package com.vanbrusselgames.mindmix.games.sudoku.viewmodel

import android.app.Activity
import androidx.compose.runtime.MutableState
import androidx.compose.runtime.mutableStateOf
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.common.BaseGameViewModel
import com.vanbrusselgames.mindmix.core.utils.constants.Difficulty
import com.vanbrusselgames.mindmix.games.sudoku.model.FinishedGame
import com.vanbrusselgames.mindmix.games.sudoku.model.InputMode
import com.vanbrusselgames.mindmix.games.sudoku.model.SudokuPuzzleCell
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow

class MockSudokuViewModel : BaseGameViewModel(), ISudokuViewModel {
    companion object {
        const val SIZE = 9
    }

    override val cells = Array(SIZE * SIZE) { SudokuPuzzleCell(it, false, 0, SIZE) }

    override val finishedGame = mutableStateOf(FinishedGame())
    override val autoEditNotes = mutableStateOf(false)
    override val checkConflictingCells = mutableStateOf(false)
    override val difficulty = mutableStateOf(Difficulty.MEDIUM)
    override val inputMode = mutableStateOf(InputMode.Normal)
    override val preferencesLoaded = MutableStateFlow(false).asStateFlow()
    override val puzzleLoaded = MutableStateFlow(false).asStateFlow()

    private var finished = false

    override fun startNewGame() {
    }

    override fun setSelectedCell(cellId: Int) {
        cells.forEach { it.isSelected.value = it.id == cellId }
    }

    override fun changeInputMode() {
        inputMode.value = if (inputMode.value == InputMode.Normal) InputMode.Note
        else InputMode.Normal
    }

    override fun onClickNumPadCell(numPadCellIndex: Int, navController: NavController) {
        if (finished) return
        val cell = cells.find { it.isSelected.value }
        if (cell == null) return

        if (inputMode.value == InputMode.Normal) {
            cell.value.intValue = numPadCellIndex + 1
            if (finished) cells.forEach { it.isSelected.value = false }
        } else {
            cell.setNote(numPadCellIndex + 1)
            cell.value.intValue = 0
        }
    }

    override fun onClickUpdateAutoEditNotes() {
        autoEditNotes.value = !autoEditNotes.value
    }

    override fun onClickUpdateCheckConflictingCells() {
        checkConflictingCells.value = !checkConflictingCells.value
    }

    override fun forceSave() {
    }

    override fun checkAdLoaded(activity: Activity, adLoaded: MutableState<Boolean>) {
    }

    override fun showAd(
        activity: Activity, adLoaded: MutableState<Boolean>, onAdWatched: (Int) -> Unit
    ) {
    }

    override fun setDifficulty(value: Difficulty) {
        difficulty.value = value
    }
}