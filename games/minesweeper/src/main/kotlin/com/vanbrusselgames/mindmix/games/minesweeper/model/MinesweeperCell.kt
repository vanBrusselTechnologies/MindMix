package com.vanbrusselgames.mindmix.games.minesweeper.model

import androidx.compose.runtime.mutableStateOf
import androidx.compose.ui.graphics.Color

data class MinesweeperCell(val id: Int) {
    var isMine = false
    var pressed = false
    var mineCount = 0

    val background = mutableStateOf(Color.White)

    var state = CellState.Empty
        set(value) {
            field = value
            mutableCellState.value = value
        }
    val mutableCellState = mutableStateOf(state)

    fun reset() {
        state = CellState.Empty
        pressed = false
        background.value = Color.White
    }
}