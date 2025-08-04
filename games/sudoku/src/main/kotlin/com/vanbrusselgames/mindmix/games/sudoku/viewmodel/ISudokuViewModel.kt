package com.vanbrusselgames.mindmix.games.sudoku.viewmodel

import androidx.compose.runtime.State
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.common.IBaseGameViewModel
import com.vanbrusselgames.mindmix.core.utils.constants.Difficulty
import com.vanbrusselgames.mindmix.games.sudoku.model.InputMode
import com.vanbrusselgames.mindmix.games.sudoku.model.SudokuPuzzleCell
import kotlinx.coroutines.flow.StateFlow

interface ISudokuViewModel : IBaseGameViewModel {
    val cells: Array<SudokuPuzzleCell>

    val autoEditNotes: State<Boolean>
    val checkConflictingCells: State<Boolean>
    val difficulty: State<Difficulty>
    val inputMode: State<InputMode>
    val puzzleLoaded: StateFlow<Boolean>
    val preferencesLoaded: StateFlow<Boolean>

    fun startPuzzle()
    fun setSelectedCell(cellId: Int)
    fun changeInputMode()
    fun onClickNumPadCell(numPadCellIndex: Int, navController: NavController)

    fun onClickUpdateAutoEditNotes()
    fun onClickUpdateCheckConflictingCells()
    fun setDifficulty(value: Difficulty)
}