package com.vanbrusselgames.mindmix.games.minesweeper.viewmodel

import androidx.compose.runtime.MutableIntState
import androidx.compose.runtime.State
import androidx.compose.ui.geometry.Offset
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.common.IBaseGameViewModel
import com.vanbrusselgames.mindmix.core.utils.constants.Difficulty
import com.vanbrusselgames.mindmix.games.minesweeper.model.InputMode
import com.vanbrusselgames.mindmix.games.minesweeper.model.MinesweeperCell
import kotlinx.coroutines.flow.StateFlow

interface IMinesweeperViewModel : IBaseGameViewModel {
    val autoFlag: State<Boolean>
    val difficulty: State<Difficulty>
    val inputMode: State<InputMode>
    val minesLeft: MutableIntState
    val safeStart: State<Boolean>
    val puzzleLoaded: StateFlow<Boolean>
    val preferencesLoaded: StateFlow<Boolean>

    val cells: Array<MinesweeperCell>
    var finished: Boolean
    var sizeX: Int
    var sizeY: Int

    fun onSelectCell(offset: Offset, cellSize: Float, navController: NavController)
    fun changeInputMode()

    fun onClickUpdateAutoFlag()
    fun onClickUpdateSafeStart()
    fun setDifficulty(value: Difficulty)
}