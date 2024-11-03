package com.vanbrusselgames.mindmix.games.minesweeper

import androidx.compose.runtime.mutableStateOf
import androidx.compose.ui.graphics.Color

data class MinesweeperCell(val viewModel: GameViewModel, val id: Int) {
    var isMine = false
    var pressed = false

    var state = State.Empty
        set(value) {
            field = value
            mutableCellState.value = value
        }
    val mutableCellState = mutableStateOf(state)

    var mineCount: Int = 0
        private set

    val background = mutableStateOf(Color.White)

    enum class State { Bomb, Flag, Number, Empty }

    fun getCellMineCount() {
        if (isMine) {
            this.mineCount = 99
            return
        }
        val shortSide: Int = minOf(viewModel.sizeX, viewModel.sizeY)
        var mineCount = 0
        var i = -1
        while (i <= 1) {
            var j = -2
            while (j < 1) {
                j++
                val mineIndex = id + i + j * shortSide
                if (mineIndex < 0 || mineIndex >= viewModel.cellCount) continue
                if (mineIndex % shortSide == 0 && i == 1 || id % shortSide == 0 && i == -1) continue
                mineCount += if (viewModel.mines[mineIndex]) 1 else 0
            }
            i++
        }
        this.mineCount = mineCount
    }
}
