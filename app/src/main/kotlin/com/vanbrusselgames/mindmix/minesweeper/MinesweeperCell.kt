package com.vanbrusselgames.mindmix.minesweeper

import androidx.compose.runtime.mutableStateOf

data class MinesweeperCell(val id: Int, val isMine: Boolean){
    private var _state = State.Empty
    val mutableCellState = mutableStateOf(_state)
    var state
        get() = _state
        set(value) {
            _state = value
            mutableCellState.value = value
        }
    val mineCount = getCellMineCount()

    enum class State{
        Bomb,
        Flag,
        Number,
        Empty
    }

    private fun getCellMineCount(): Int {
        val shortSide: Int = minOf(MinesweeperManager.sizeX, MinesweeperManager.sizeY)
        if (isMine) return 99
        var mineCount = 0
        var i = -1
        while (i <= 1) {
            var j = -2
            while (j < 1) {
                j++
                val mineIndex = id + i + j * shortSide
                if (mineIndex < 0 || mineIndex >= MinesweeperManager.cellCount) continue
                if (mineIndex % shortSide == 0 && i == 1 || id % shortSide == 0 && i == -1) continue
                mineCount += if (MinesweeperData.Mines[mineIndex]) 1 else 0
            }
            i++
        }
        return mineCount
    }
}
