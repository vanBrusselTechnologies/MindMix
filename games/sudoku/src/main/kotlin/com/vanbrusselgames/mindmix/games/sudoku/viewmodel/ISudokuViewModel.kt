package com.vanbrusselgames.mindmix.games.sudoku.viewmodel

import android.app.Activity
import androidx.compose.runtime.MutableState
import androidx.compose.runtime.State
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.common.IBaseGameViewModel
import com.vanbrusselgames.mindmix.core.utils.constants.Difficulty
import com.vanbrusselgames.mindmix.games.sudoku.model.FinishedGame
import com.vanbrusselgames.mindmix.games.sudoku.model.InputMode
import com.vanbrusselgames.mindmix.games.sudoku.model.SudokuPuzzleCell
import kotlinx.coroutines.flow.StateFlow

interface ISudokuViewModel : IBaseGameViewModel {
    val cells: Array<SudokuPuzzleCell>

    val finishedGame: State<FinishedGame>
    val autoEditNotes: State<Boolean>
    val checkConflictingCells: State<Boolean>
    val difficulty: State<Difficulty>
    val inputMode: State<InputMode>
    val puzzleLoaded: StateFlow<Boolean>
    val preferencesLoaded: StateFlow<Boolean>

    fun setSelectedCell(cellId: Int)
    fun changeInputMode()
    fun onClickNumPadCell(numPadCellIndex: Int, navController: NavController)

    fun forceSave()
    fun checkAdLoaded(activity: Activity, adLoaded: MutableState<Boolean>)
    fun showAd(activity: Activity, adLoaded: MutableState<Boolean>, onAdWatched: (Int) -> Unit)

    fun onClickUpdateAutoEditNotes()
    fun onClickUpdateCheckConflictingCells()
    fun setDifficulty(value: Difficulty)
}