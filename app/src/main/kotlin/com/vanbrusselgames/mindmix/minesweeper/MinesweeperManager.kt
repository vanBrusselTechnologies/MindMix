package com.vanbrusselgames.mindmix.minesweeper

import androidx.compose.runtime.mutableStateOf
import kotlin.math.pow
import kotlin.random.Random

class MinesweeperManager {
    companion object Instance {
        var inputMode: InputMode = InputMode.Normal
        val minesweeperFinished = mutableStateOf(false)
        var sizeX = 16
        var sizeY = 22
        val cellCount
            get() = sizeX * sizeY
        var cells = arrayOf<MinesweeperCell>()

        enum class PuzzleType {
            Classic
        }

        enum class InputMode {
            Normal, Flag
        }

        fun loadPuzzle() {
            if (MinesweeperData.Mines.isEmpty()) {
                val difficulty = 0
                MinesweeperData.Mines = Array(cellCount) { false }
                MinesweeperData.Input = IntArray(cellCount) { 0 }
                val mineCount = 25 + (10f * 1.75f.pow(difficulty)).toInt()
                var i = 0
                while (i < mineCount) {
                    val rand: Int = Random.nextInt(cellCount)
                    val isMine = MinesweeperData.Mines[rand]
                    if (!isMine) {
                        MinesweeperData.Mines[rand] = true
                        i++
                    }
                }
                cells = Array(cellCount) { MinesweeperCell(it, MinesweeperData.Mines[it]) }
            } else {/*
                for (int i = 0; i < chars.Length; i++)
                {
                    char ch = chars[i];
                    switch (ch)
                    {
                        case '2':
                            _minesweeperInput[i] = 2;
                            InstantiateFlag(i);
                            break;
                        case '1':
                            _minesweeperInput[i] = 1;
                            InstantiateButton(i);
                            break;
                    }
                }
                */
            }
        }

        fun findOtherSafeCells(cell: MinesweeperCell, tmp: MutableList<Int>) {
            val index = cell.id
            var i = -1
            while (i <= 1) {
                var j = -2
                while (j < 1) {
                    j++
                    val mineIndex =
                        if (sizeX < sizeY) index + i + j * sizeX else index + sizeY * i + j
                    if (mineIndex < 0 || mineIndex >= cellCount) continue
                    if (sizeX < sizeY) {
                        if (mineIndex % sizeX == 0 && i == 1 || index % sizeX == 0 && i == -1) continue
                    } else {
                        if (mineIndex % sizeY == 0 && j == 1 || index % sizeY == 0 && j == -1) continue
                    }
                    val nextCell = cells[mineIndex]
                    if (nextCell.state != MinesweeperCell.State.Empty) continue
                    //if (MinesweeperData.Input[mineIndex] == 1) continue
                    //MinesweeperData.Input[mineIndex] = 1
                    nextCell.state = MinesweeperCell.State.Number
                    tmp.add(mineIndex)
                    if (nextCell.mineCount == 0) findOtherSafeCells(nextCell, tmp)
                }
                i++
            }
        }

        fun showAllMines() {
            if (MinesweeperData.GameOver) return
            MinesweeperData.GameOver = true
            var i = -1
            while (i < cellCount - 1) {
                i++
                if (!cells[i].isMine || cells[i].state != MinesweeperCell.State.Empty/*MinesweeperData.Input[i] != 0*/) continue
                cells[i].state = MinesweeperCell.State.Bomb
                //MinesweeperData.Input[i] = 1
            }
            minesweeperFinished.value = true
        }

        fun checkFinished() {
            MinesweeperData.Finised = isFinished()
            minesweeperFinished.value = MinesweeperData.Finised
        }

        private fun isFinished(): Boolean {
            for (cell in cells) {
                if (!cell.isMine && cell.state != MinesweeperCell.State.Number) return false
                if (cell.state == MinesweeperCell.State.Bomb) return false
            }/*var i = 0
            while (i < cellCount) {
                if (!MinesweeperData.Mines[i] && MinesweeperData.Input[i] != 1) return false
                if (MinesweeperData.Mines[i] && MinesweeperData.Input[i] == 1) return false
                i++
            }*/
            return true
        }

        fun changeInputMode(): InputMode {
            inputMode = if (inputMode == InputMode.Flag) InputMode.Normal else InputMode.Flag
            return inputMode
        }
    }
}