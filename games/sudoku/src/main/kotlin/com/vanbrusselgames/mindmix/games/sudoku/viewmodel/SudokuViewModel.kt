package com.vanbrusselgames.mindmix.games.sudoku.viewmodel

import android.app.Activity
import androidx.compose.runtime.MutableState
import androidx.compose.runtime.mutableStateOf
import androidx.lifecycle.viewModelScope
import androidx.navigation.NavController
import com.google.firebase.analytics.FirebaseAnalytics
import com.vanbrusselgames.mindmix.core.advertisement.AdManager
import com.vanbrusselgames.mindmix.core.common.BaseGameViewModel
import com.vanbrusselgames.mindmix.core.logging.Logger
import com.vanbrusselgames.mindmix.core.model.SceneRegistry
import com.vanbrusselgames.mindmix.core.utils.constants.Difficulty
import com.vanbrusselgames.mindmix.games.sudoku.data.SudokuRepository
import com.vanbrusselgames.mindmix.games.sudoku.data.preferences.SudokuPreferences
import com.vanbrusselgames.mindmix.games.sudoku.data.preferences.SudokuPreferencesRepository
import com.vanbrusselgames.mindmix.games.sudoku.helpers.SudokuPuzzle
import com.vanbrusselgames.mindmix.games.sudoku.model.FinishedGame
import com.vanbrusselgames.mindmix.games.sudoku.model.InputMode
import com.vanbrusselgames.mindmix.games.sudoku.model.PuzzleType
import com.vanbrusselgames.mindmix.games.sudoku.model.SudokuProgress
import com.vanbrusselgames.mindmix.games.sudoku.model.SudokuPuzzleCell
import com.vanbrusselgames.mindmix.games.sudoku.model.rewardForDifficulty
import com.vanbrusselgames.mindmix.games.sudoku.navigation.navigateToSudokuGameFinished
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.flow.firstOrNull
import kotlinx.coroutines.flow.flowOn
import kotlinx.coroutines.flow.onStart
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import javax.inject.Inject

@HiltViewModel
class SudokuViewModel @Inject constructor(
    private val adManager: AdManager,
    private val sudokuRepository: SudokuRepository,
    private val prefsRepository: SudokuPreferencesRepository
) : BaseGameViewModel(), ISudokuViewModel {
    companion object Instance {
        const val SIZE = 9
    }

    override val cells = Array(SIZE * SIZE) { SudokuPuzzleCell(it, false, 0, SIZE) }

    override val finishedGame = mutableStateOf(FinishedGame())
    override val autoEditNotes = mutableStateOf(false)
    override val checkConflictingCells = mutableStateOf(false)
    override val difficulty = mutableStateOf(Difficulty.MEDIUM)
    override val inputMode = mutableStateOf(InputMode.Normal)

    private val _preferencesLoaded = MutableStateFlow(false)
    override val preferencesLoaded = _preferencesLoaded.onStart { loadPreferences() }
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000L), false)

    private val _puzzleLoaded = MutableStateFlow(false)
    override val puzzleLoaded = _puzzleLoaded.onStart { loadData() }
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000L), false)

    private var finished = false
    private var gameMode = PuzzleType.Classic
    private var saveJob: Job? = null

    private suspend fun loadPreferences() {
        applyPreferences(prefsRepository.getPreferences().first())
    }

    private suspend fun applyPreferences(preferences: SudokuPreferences) {
        Logger.d("[sudoku] applyPreferences")
        autoEditNotes.value = preferences.autoEditNotes
        checkConflictingCells.value = preferences.checkConflictingCells
        difficulty.value = Difficulty.entries[preferences.difficulty]
        _preferencesLoaded.emit(true)
    }

    private suspend fun loadData() {
        withContext(Dispatchers.IO) {
            preferencesLoaded.first { it }
            val savedProgress = sudokuRepository.getPuzzleProgress(difficulty.value)
            if (savedProgress != null) onPuzzleLoaded(savedProgress)
            else onPuzzleLoaded(
                sudokuRepository.requestNewPuzzleFlow(difficulty.value, gameMode)
                    .flowOn(Dispatchers.IO).firstOrNull()
            )
        }
    }

    override fun startNewGame() {
        Logger.d("[sudoku] startNewGame")
        reset()
        sudokuRepository.removeProgressForDifficulty(difficulty.value)
        startPuzzle()
        Logger.logEvent(FirebaseAnalytics.Event.LEVEL_START) {
            param(FirebaseAnalytics.Param.LEVEL_NAME, SceneRegistry.Sudoku.name)
        }
    }

    private fun startPuzzle() {
        Logger.d("[sudoku] startPuzzle")
        if (finished) reset()
        if (_puzzleLoaded.value) return
        viewModelScope.launch(Dispatchers.IO) {
            onPuzzleLoaded(
                sudokuRepository.getPuzzleProgress(difficulty.value)
                    ?: sudokuRepository.requestNewPuzzleFlow(difficulty.value, gameMode)
                        .firstOrNull()
            )
        }
    }

    private fun onPuzzleLoaded(p: SudokuProgress?) {
        Logger.d("[sudoku] onPuzzleLoaded")
        if (p == null || cells.size != p.input.size) {
            _puzzleLoaded.value = false
            return
        }
        Logger.d(
            "[sudoku] Puzzle Loaded: ${p.difficulty}:\n${p.clues.joinToString("")}\n${
                p.input.joinToString("")
            }"
        )
        cells.forEach {
            it.reset()
            it.isClue.value = p.clues[it.id] != 0
            it.value.intValue = p.input[it.id]
            for (j in 0 until SIZE) {
                val note = j + 1
                if (p.inputNotes[it.id].contains(note) != it.hasNote(note)) {
                    it.setNote(note)
                }
            }
        }
        if (checkConflictingCells.value) cells.forEach { checkConflictingCell(it, true) }
        else cells.forEach { it.isIncorrect.value = false }
        _puzzleLoaded.value = true
    }

    private fun reset() {
        finished = false
        cells.forEach {
            it.reset()
        }
        _puzzleLoaded.value = false
    }

    override fun setSelectedCell(cellId: Int) {
        cells.forEach { it.isSelected.value = it.id == cellId }
    }

    override fun changeInputMode() {
        if (finished) return
        inputMode.value = if (inputMode.value == InputMode.Normal) InputMode.Note
        else InputMode.Normal
    }

    override fun onClickNumPadCell(numPadCellIndex: Int, navController: NavController) {
        if (finished) return
        val cell = cells.find { it.isSelected.value }
        if (cell == null) return

        if (inputMode.value == InputMode.Normal) {
            cell.value.intValue = numPadCellIndex + 1
            checkFinished(navController)
            if (finished) {
                cells.forEach { it.isSelected.value = false }
            } else if (autoEditNotes.value) autoChangeNotes(cell)
        } else {
            cell.setNote(numPadCellIndex + 1)
            cell.value.intValue = 0
        }
        if (checkConflictingCells.value) checkConflictingCell(cell, false)

        saveJob?.cancel()
        saveJob = CoroutineScope(Dispatchers.IO).launch {
            delay(2000)
            if (finished) return@launch
            sudokuRepository.setPuzzleProgressForDifficulty(difficulty.value, cells)
        }
    }

    private fun checkFinished(navController: NavController) {
        finished = isFinished()
        if (finished) {
            Logger.logEvent(FirebaseAnalytics.Event.LEVEL_END) {
                param(FirebaseAnalytics.Param.LEVEL_NAME, SceneRegistry.Sudoku.name)
                param(FirebaseAnalytics.Param.SUCCESS, 1)
            }
            sudokuRepository.removeProgressForDifficulty(difficulty.value)
            onGameFinished(navController)
        }
    }

    private fun isFinished(): Boolean {
        try {
            if (cells.any { it.value.intValue == 0 }) return false
            val input = cells.map { it.value.intValue }.toIntArray()
            return SudokuPuzzle.getSolution(input).contentEquals(input)
        } catch (_: Exception) {
            return false
        }
    }

    private fun onGameFinished(navController: NavController) {
        val reward = rewardForDifficulty[difficulty.value]!!
        finishedGame.value = FinishedGame(reward)
        navController.navigateToSudokuGameFinished()
    }

    override fun forceSave() {
        sudokuRepository.forceSave()
    }

    override fun checkAdLoaded(activity: Activity, adLoaded: MutableState<Boolean>) {
        adManager.checkAdLoaded(activity, adLoaded)
    }

    override fun showAd(
        activity: Activity, adLoaded: MutableState<Boolean>, onAdWatched: (Int) -> Unit
    ) {
        adManager.showAd(activity, adLoaded, onAdWatched)
    }

    override fun onClickUpdateAutoEditNotes() {
        val enabled = !autoEditNotes.value
        autoEditNotes.value = enabled
        viewModelScope.launch {
            prefsRepository.savePreferences(
                SudokuPreferences(
                    enabled, checkConflictingCells.value, difficulty.value.ordinal
                )
            )
        }
    }

    private fun autoChangeNotes(cell: SudokuPuzzleCell) {
        if (!_puzzleLoaded.value) return
        val index = cell.id
        val number: Int = cell.value.intValue
        if (number == 0) return
        val indices: IntArray = SudokuPuzzle.peers(index, cells.size)
        for (n: Int in indices) {
            if (index == n) continue
            if (!cells[n].hasNote(number)) continue
            cells[n].setNote(number)
        }
    }

    override fun onClickUpdateCheckConflictingCells() {
        val enabled = !checkConflictingCells.value
        checkConflictingCells.value = enabled
        if (enabled) {
            cells.forEach { checkConflictingCell(it, true) }
        } else cells.forEach { it.isIncorrect.value = false }
        viewModelScope.launch {
            prefsRepository.savePreferences(
                SudokuPreferences(autoEditNotes.value, enabled, difficulty.value.ordinal)
            )
        }
    }

    private fun checkConflictingCell(cell: SudokuPuzzleCell, isSecondary: Boolean) {
        if (cell.isClue.value) return
        val index = cell.id
        val indices: IntArray = SudokuPuzzle.peers(index, cells.size)
        var isConflicting = false
        val v = cell.value.intValue
        for (n: Int in indices) {
            if (isSecondary && isConflicting) break
            if (index == n) continue
            val cellN = cells[n]
            val cellNValue = cellN.value.intValue
            when (true) {
                (v == 0) -> {
                    if (!isSecondary && cellN.isIncorrect.value) {
                        checkConflictingCell(cellN, true)
                    }
                    if (cellNValue == 0) continue
                    if (!cell.hasNote(cellNValue)) continue
                    isConflicting = true
                }

                (v == cellNValue) -> {
                    isConflicting = true
                    if (!isSecondary) checkConflictingCell(cellN, true)
                }

                (!isSecondary && cellN.isIncorrect.value || cellNValue == 0 && cellN.hasNote(v)) -> {
                    checkConflictingCell(cellN, true)
                }

                else -> {}
            }
        }
        cell.isIncorrect.value = isConflicting
    }

    override fun setDifficulty(value: Difficulty) {
        saveAndLoadProgress(difficulty.value, value)
        difficulty.value = value

        viewModelScope.launch {
            prefsRepository.savePreferences(
                SudokuPreferences(
                    autoEditNotes.value, checkConflictingCells.value, value.ordinal
                )
            )
        }
    }

    private fun saveAndLoadProgress(prevDifficulty: Difficulty, difficulty: Difficulty) {
        Logger.d("[sudoku] saveAndLoadProgress")
        if (prevDifficulty == difficulty) return
        if (_puzzleLoaded.value && !finished) {
            sudokuRepository.setPuzzleProgressForDifficulty(prevDifficulty, cells)
        }

        reset()
        viewModelScope.launch(Dispatchers.IO) {
            onPuzzleLoaded(
                sudokuRepository.getPuzzleProgress(difficulty)
                    ?: sudokuRepository.requestNewPuzzleFlow(difficulty, gameMode).firstOrNull()
            )
        }
    }
}