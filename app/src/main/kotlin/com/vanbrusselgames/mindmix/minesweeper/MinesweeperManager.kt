package com.vanbrusselgames.mindmix.minesweeper

import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.ui.graphics.Color
import com.google.firebase.analytics.FirebaseAnalytics
import com.vanbrusselgames.mindmix.BaseLayout
import com.vanbrusselgames.mindmix.Logger
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json
import kotlin.math.pow
import kotlin.random.Random

class MinesweeperManager {
    companion object Instance {
        private const val GAME_NAME = "Minesweeper"
        var inputMode: InputMode = InputMode.Normal
        var sizeX = 16
        var sizeY = 22

        var finished = false
        val minesweeperFinished = mutableStateOf(false)

        val mines = BooleanArray(sizeX * sizeY) { false }

        val cellCount
            get() = sizeX * sizeY
        var cells = Array(cellCount) { MinesweeperCell(it) }

        val minesLeft = mutableIntStateOf(-1)

        private var isLoaded = false

        enum class PuzzleType {
            Classic
        }

        enum class InputMode {
            Normal, Flag
        }

        fun loadFromFile(data: MinesweeperData) {
            if (data.finished) {
                startNewGame()
                return
            }
            if (data.mines.isEmpty()) return
            minesLeft.intValue = data.mines.size
            for (index in data.mines) mines[index] = true
            val stateEntries = MinesweeperCell.State.entries
            cells.forEachIndexed { i, c ->
                c.isMine = mines[i]
                c.getCellMineCount()
                c.state = stateEntries[data.input[i]]
            }
            calculateMinesLeft()
            isLoaded = true
        }

        fun saveToFile(): String {
            val mineIndices = mutableListOf<Int>()
            val input = mutableListOf<Int>()
            cells.forEach { c ->
                if (c.isMine) mineIndices.add(c.id)
                input.add(c.state.ordinal)
            }
            return Json.encodeToString(MinesweeperData(input, mineIndices, finished))
        }

        fun loadPuzzle() {
            if (finished) reset()
            if (isLoaded) return
            Logger.logEvent(FirebaseAnalytics.Event.LEVEL_START) {
                param(FirebaseAnalytics.Param.LEVEL_NAME, GAME_NAME)
            }
            val difficulty = 1
            val mineCount = 25 + (10f * 1.75f.pow(difficulty)).toInt()
            var i = 0
            while (i < mineCount) {
                val rand: Int = Random.nextInt(cellCount)
                val isMine = mines[rand]
                if (!isMine) {
                    mines[rand] = true
                    i++
                }
            }
            cells.forEachIndexed { j, c ->
                c.isMine = mines[j]
                c.getCellMineCount()
            }
            minesLeft.intValue = mineCount
            isLoaded = true
        }

        private fun reset() {
            finished = false
            minesweeperFinished.value = finished
            BaseLayout.disableTopRowButtons.value = false
            mines.fill(false)
            cells.forEach { c ->
                c.isMine = false
                c.state = MinesweeperCell.State.Empty
                c.pressed = false
                c.background.value = Color.White
            }
            isLoaded = false
        }

        fun startNewGame() {
            reset()
            loadPuzzle()
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
                    nextCell.state = MinesweeperCell.State.Number
                    tmp.add(mineIndex)
                    if (nextCell.mineCount == 0) findOtherSafeCells(nextCell, tmp)
                }
                i++
            }
        }

        fun showAllMines() {
            if (finished) return
            finished = true
            Logger.logEvent(FirebaseAnalytics.Event.LEVEL_END) {
                param(FirebaseAnalytics.Param.LEVEL_NAME, GAME_NAME)
                param(FirebaseAnalytics.Param.SUCCESS, 0)
            }
            var i = -1
            while (i < cellCount - 1) {
                i++
                if (!cells[i].isMine || cells[i].state != MinesweeperCell.State.Empty) continue
                cells[i].state = MinesweeperCell.State.Bomb
            }
            minesweeperFinished.value = finished
            BaseLayout.disableTopRowButtons.value = true
        }

        fun calculateMinesLeft() {
            var count = 0
            cells.forEach { c ->
                if (c.isMine) count++
                if (c.state == MinesweeperCell.State.Flag) count--
            }
            minesLeft.intValue = count
        }

        fun checkFinished() {
            finished = isFinished()
            if (finished) {
                minesweeperFinished.value = finished
                BaseLayout.disableTopRowButtons.value = finished
                Logger.logEvent(FirebaseAnalytics.Event.LEVEL_END) {
                    param(FirebaseAnalytics.Param.LEVEL_NAME, GAME_NAME)
                    param(FirebaseAnalytics.Param.SUCCESS, 1)
                }
            }
        }

        private fun isFinished(): Boolean {
            for (cell in cells) {
                if (!cell.isMine && cell.state != MinesweeperCell.State.Number) return false
                if (cell.state == MinesweeperCell.State.Bomb) return false
            }
            return true
        }

        fun changeInputMode(): InputMode {
            inputMode = if (inputMode == InputMode.Flag) InputMode.Normal else InputMode.Flag
            return inputMode
        }
    }
}