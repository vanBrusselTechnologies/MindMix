package com.vanbrusselgames.mindmix.games.minesweeper.viewmodel

import androidx.collection.MutableIntList
import androidx.collection.mutableIntListOf
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.ui.geometry.Offset
import androidx.lifecycle.viewModelScope
import androidx.navigation.NavController
import com.google.firebase.analytics.FirebaseAnalytics
import com.vanbrusselgames.mindmix.core.common.BaseGameViewModel
import com.vanbrusselgames.mindmix.core.logging.Logger
import com.vanbrusselgames.mindmix.core.model.SceneRegistry
import com.vanbrusselgames.mindmix.core.utils.constants.Difficulty
import com.vanbrusselgames.mindmix.feature.gamefinished.navigation.navigateToGameFinished
import com.vanbrusselgames.mindmix.games.minesweeper.R
import com.vanbrusselgames.mindmix.games.minesweeper.data.MinesweeperRepository
import com.vanbrusselgames.mindmix.games.minesweeper.data.preferences.MinesweeperPreferences
import com.vanbrusselgames.mindmix.games.minesweeper.data.preferences.MinesweeperPreferencesRepository
import com.vanbrusselgames.mindmix.games.minesweeper.model.CellState
import com.vanbrusselgames.mindmix.games.minesweeper.model.FinishedGame
import com.vanbrusselgames.mindmix.games.minesweeper.model.InputMode
import com.vanbrusselgames.mindmix.games.minesweeper.model.Minesweeper
import com.vanbrusselgames.mindmix.games.minesweeper.model.MinesweeperCell
import com.vanbrusselgames.mindmix.games.minesweeper.model.MinesweeperProgress
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.flow.onStart
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import javax.inject.Inject
import kotlin.math.floor
import kotlin.math.min
import kotlin.math.roundToInt

@HiltViewModel
class MinesweeperViewModel @Inject constructor(
    private val minesweeperRepository: MinesweeperRepository,
    private val prefsRepository: MinesweeperPreferencesRepository
) : BaseGameViewModel(), IMinesweeperViewModel {
    override val nameResId = Minesweeper.NAME_RES_ID
    override val descResId = R.string.minesweeper_desc

    override val autoFlag = mutableStateOf(false)
    override val difficulty = mutableStateOf(Difficulty.MEDIUM)
    override val inputMode = mutableStateOf(InputMode.Normal)
    override val safeStart = mutableStateOf(false)

    override var sizeX = 16
    override var sizeY = 22
    override val cellCount
        get() = sizeX * sizeY
    override val cells = Array(cellCount) { MinesweeperCell(this, it) }
    override val mines = BooleanArray(cellCount) { false }
    override val minesLeft = mutableIntStateOf(-1)

    private val _preferencesLoaded = MutableStateFlow(false)
    override val preferencesLoaded = _preferencesLoaded.onStart { loadPreferences() }
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000L), false)

    private val _puzzleLoaded = MutableStateFlow(false)
    override val puzzleLoaded = _puzzleLoaded.onStart { loadData() }
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000L), false)

    override var finished = false


    private suspend fun loadPreferences() {
        applyPreferences(prefsRepository.getPreferences())
    }

    private fun applyPreferences(preferences: MinesweeperPreferences) {
        Logger.d("[minesweeper] applyPreferences")
        autoFlag.value = preferences.autoFlag
        difficulty.value = Difficulty.entries[preferences.difficulty]
        safeStart.value = preferences.safeStart
        _preferencesLoaded.value = true
    }

    private suspend fun loadData() {
        withContext(Dispatchers.IO) {
            preferencesLoaded.first { it }
            val savedProgress = minesweeperRepository.getPuzzleProgress(difficulty.value)
            if (savedProgress != null) onPuzzleLoaded(savedProgress)
            else onPuzzleLoaded(minesweeperRepository.createNewPuzzle(difficulty.value, cellCount))
        }
    }

    private fun reset() {
        finished = false
        mines.fill(false)
        cells.forEach { it.reset() }
        _puzzleLoaded.value = false
    }

    override fun startNewGame() {
        reset()
        minesweeperRepository.removeProgressForDifficulty(difficulty.value)
        startPuzzle()
        Logger.logEvent(FirebaseAnalytics.Event.LEVEL_START) {
            param(FirebaseAnalytics.Param.LEVEL_NAME, SceneRegistry.Minesweeper.name)
        }
    }

    private fun startPuzzle() {
        Logger.d("[minesweeper] startPuzzle")
        if (finished) reset()
        if (_puzzleLoaded.value) return
        onPuzzleLoaded(
            minesweeperRepository.getPuzzleProgress(difficulty.value)
                ?: minesweeperRepository.createNewPuzzle(difficulty.value, cellCount)
        )
    }

    private fun onPuzzleLoaded(p: MinesweeperProgress?) {
        Logger.d("[minesweeper] onPuzzleLoaded")
        if (p == null) {
            _puzzleLoaded.value = false
            return
        }
        minesLeft.intValue = p.mines.size
        for (index in p.mines) mines[index] = true
        val stateEntries = CellState.entries
        cells.mapIndexed { i, c ->
            c.reset()
            c.isMine = mines[i]
            c.state = stateEntries[p.input[i]]
            c.getCellMineCount()
        }

        if (safeStart.value && cells.all { it.state == CellState.Empty }) {
            val cell = cells.filter { !it.isMine && it.mineCount == 0 }.random()
            cell.state = CellState.Number
            findOtherSafeCells(cell)
        }

        if (autoFlag.value) autoFlag()
        calculateMinesLeft()
        minesweeperRepository.setPuzzleProgressForDifficulty(difficulty.value, cells)
        _puzzleLoaded.value = true
    }

    override fun changeInputMode() {
        inputMode.value =
            if (inputMode.value == InputMode.Flag) InputMode.Normal else InputMode.Flag
    }

    override fun onSelectCell(offset: Offset, cellSize: Float, navController: NavController) {
        if (finished) return
        val column = floor(offset.x / cellSize)
        val row = floor(offset.y / cellSize)
        val cellIndex = if (sizeX < sizeY) {
            (column + row * sizeX).roundToInt()
        } else {
            (sizeY * (column + 1) - (row + 1)).roundToInt()
        }
        val cell = cells[cellIndex]

        cell.pressed = true
        when (cell.state) {
            CellState.Empty -> {
                if (inputMode.value == InputMode.Flag) {
                    cell.state = CellState.Flag
                    calculateMinesLeft()
                } else {
                    if (cell.isMine) return showAllMines(navController)
                    cell.state = CellState.Number
                    if (cell.mineCount == 0) findOtherSafeCells(cell)
                    if (autoFlag.value) autoFlag()
                    checkFinished(navController)
                }
            }

            CellState.Flag -> {
                if (inputMode.value == InputMode.Flag) {
                    cell.state = CellState.Empty
                    calculateMinesLeft()
                }
            }

            else -> return
        }
        minesweeperRepository.setPuzzleProgressForDifficulty(difficulty.value, cells)
    }

    private fun findOtherSafeCells(cell: MinesweeperCell) {
        val index = cell.id
        var i = -1
        while (i <= 1) {
            var j = -2
            while (j < 1) {
                j++
                val mineIndex = if (sizeX < sizeY) index + i + j * sizeX else index + sizeY * i + j
                if (mineIndex < 0 || mineIndex >= cellCount) continue
                if (sizeX < sizeY) {
                    if (mineIndex % sizeX == 0 && i == 1 || index % sizeX == 0 && i == -1) continue
                } else {
                    if (mineIndex % sizeY == 0 && j == 1 || index % sizeY == 0 && j == -1) continue
                }
                val nextCell = cells[mineIndex]
                if (nextCell.state != CellState.Empty) continue
                nextCell.state = CellState.Number
                if (nextCell.mineCount == 0) findOtherSafeCells(nextCell)
            }
            i++
        }
    }

    private fun showAllMines(navController: NavController) {
        if (finished) return
        finished = true
        Logger.logEvent(FirebaseAnalytics.Event.LEVEL_END) {
            param(FirebaseAnalytics.Param.LEVEL_NAME, SceneRegistry.Minesweeper.name)
            param(FirebaseAnalytics.Param.SUCCESS, 0)
        }
        cells.forEach {
            if (it.isMine && it.state == CellState.Empty) it.state = CellState.Bomb
        }
        onGameFinished(navController, false)
    }

    private fun calculateMinesLeft() {
        var count = 0
        cells.forEach { c ->
            if (c.isMine) count++
            if (c.state == CellState.Flag) count--
        }
        minesLeft.intValue = count
    }

    private fun checkFinished(navController: NavController) {
        finished = isFinished()
        if (finished) {
            Logger.logEvent(FirebaseAnalytics.Event.LEVEL_END) {
                param(FirebaseAnalytics.Param.LEVEL_NAME, SceneRegistry.Minesweeper.name)
                param(FirebaseAnalytics.Param.SUCCESS, 1)
            }
            onGameFinished(navController, true)
        }
    }

    private fun isFinished(): Boolean {
        for (cell in cells) {
            if (!cell.isMine && cell.state != CellState.Number) return false
            if (cell.state == CellState.Bomb) return false
        }
        return true
    }

    private fun onGameFinished(navController: NavController, success: Boolean) {
        FinishedGame.titleResId = Minesweeper.NAME_RES_ID
        // if (failed) "Failed" else "Congrats / Smart / Well done"
        FinishedGame.textResId =
            if (!success) R.string.minesweeper_failed else R.string.minesweeper_success
        // if (failed) "A mine exploded" else """You did great and solved puzzle in ${0} seconds!!
        //     |That's Awesome!
        //     |Share with your friends and challenge them to beat your time!""".trimMargin()
        FinishedGame.reward = if (!success) 0 else 10

        navController.navigateToGameFinished()
    }

    override fun onClickUpdateAutoFlag() {
        val enabled = !autoFlag.value
        autoFlag.value = enabled
        if (enabled) autoFlag()

        viewModelScope.launch {
            prefsRepository.savePreferences(
                MinesweeperPreferences(enabled, difficulty.value.ordinal, safeStart.value)
            )
        }
    }

    private fun autoFlag() {
        var autoPlacedFlag = false
        for (c in cells) {
            if (!c.isMine || c.state != CellState.Empty) continue
            if (!canBeAutoFlagged(c.id, mutableIntListOf(c.id))) continue
            c.state = CellState.Flag
            autoPlacedFlag = true
        }
        if (autoPlacedFlag) calculateMinesLeft()
    }

    private fun canBeAutoFlagged(mineIndex: Int, safeIndexes: MutableIntList): Boolean {
        val size = min(sizeX, sizeY)
        for (i in 0 until 3) {
            for (j in 0 until 3) {
                val index = mineIndex + i - 1 + (j - 1) * size
                if (index == mineIndex) continue
                if (index < 0 || index >= cells.size) continue
                if (safeIndexes.contains(index)) continue
                if ((index % size == 0 && i == 2) || (mineIndex % size == 0 && i == 0)) continue
                if (cells[index].state == CellState.Number) continue
                if (!cells[index].isMine) return false
                safeIndexes.add(index)
                if (!canBeAutoFlagged(index, safeIndexes)) return false
            }
        }

        return true
    }

    override fun onClickUpdateSafeStart() {
        val enabled = !safeStart.value
        safeStart.value = enabled
        viewModelScope.launch {
            prefsRepository.savePreferences(
                MinesweeperPreferences(autoFlag.value, difficulty.value.ordinal, enabled)
            )
        }
    }

    override fun setDifficulty(value: Difficulty) {
        saveAndLoadProgress(difficulty.value, value)
        difficulty.value = value

        viewModelScope.launch {
            prefsRepository.savePreferences(
                MinesweeperPreferences(
                    autoFlag.value, value.ordinal, safeStart.value
                )
            )
        }
    }

    private fun saveAndLoadProgress(prevDifficulty: Difficulty, difficulty: Difficulty) {
        Logger.d("[minesweeper] saveAndLoadProgress")
        if (prevDifficulty == difficulty) return
        if (_puzzleLoaded.value && !finished) {
            minesweeperRepository.setPuzzleProgressForDifficulty(prevDifficulty, cells)
        }

        reset()
        onPuzzleLoaded(
            minesweeperRepository.getPuzzleProgress(difficulty)
                ?: minesweeperRepository.createNewPuzzle(difficulty, cellCount)
        )
    }
}