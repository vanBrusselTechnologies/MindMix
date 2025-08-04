package com.vanbrusselgames.mindmix.games.sudoku.viewmodel

import androidx.compose.runtime.mutableStateOf
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.common.BaseGameViewModel
import com.vanbrusselgames.mindmix.core.utils.constants.Difficulty
import com.vanbrusselgames.mindmix.games.sudoku.R
import com.vanbrusselgames.mindmix.games.sudoku.model.InputMode
import com.vanbrusselgames.mindmix.games.sudoku.model.Sudoku
import com.vanbrusselgames.mindmix.games.sudoku.model.SudokuPuzzleCell
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow

class MockSudokuViewModel : BaseGameViewModel(), ISudokuViewModel {
    companion object Instance {
        const val SIZE = 9
    }

    override val nameResId = Sudoku.Companion.NAME_RES_ID
    override val descResId = R.string.sudoku_desc

    override val cells = Array(SIZE * SIZE) { SudokuPuzzleCell(it, false, 0, SIZE) }

    override val autoEditNotes = mutableStateOf(false)
    override val checkConflictingCells = mutableStateOf(false)
    override val difficulty = mutableStateOf(Difficulty.MEDIUM)
    override val inputMode = mutableStateOf(InputMode.Normal)
    override val preferencesLoaded = MutableStateFlow(false).asStateFlow()
    override val puzzleLoaded = MutableStateFlow(false).asStateFlow()

    private var finished = false

    override fun startNewGame() {
        startPuzzle()
    }

    override fun startPuzzle() {
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

    override fun setDifficulty(value: Difficulty) {
        difficulty.value = value
    }
}