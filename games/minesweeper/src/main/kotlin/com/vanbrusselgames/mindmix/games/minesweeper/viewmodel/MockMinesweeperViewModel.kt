package com.vanbrusselgames.mindmix.games.minesweeper.viewmodel

import androidx.collection.MutableIntList
import androidx.collection.mutableIntListOf
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.ui.geometry.Offset
import androidx.navigation.NavController
import com.google.firebase.analytics.FirebaseAnalytics
import com.vanbrusselgames.mindmix.core.common.viewmodel.BaseGameViewModel
import com.vanbrusselgames.mindmix.core.logging.Logger
import com.vanbrusselgames.mindmix.core.model.SceneRegistry
import com.vanbrusselgames.mindmix.core.utils.constants.Difficulty
import com.vanbrusselgames.mindmix.games.minesweeper.model.CellState
import com.vanbrusselgames.mindmix.games.minesweeper.model.FinishedGame
import com.vanbrusselgames.mindmix.games.minesweeper.model.InputMode
import com.vanbrusselgames.mindmix.games.minesweeper.model.MinesweeperCell
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlin.math.floor
import kotlin.math.min
import kotlin.math.roundToInt

class MockMinesweeperViewModel : BaseGameViewModel(), IMinesweeperViewModel {
    override val finishedGame = mutableStateOf(FinishedGame())
    override val autoFlag = mutableStateOf(false)
    override val difficulty = mutableStateOf(Difficulty.MEDIUM)
    override val inputMode = mutableStateOf(InputMode.Normal)
    override val safeStart = mutableStateOf(false)

    override var sizeX = 16
    override var sizeY = 22
    val cellCount
        get() = sizeX * sizeY
    override val cells = Array(cellCount) { MinesweeperCell(it) }
    val mines = BooleanArray(cellCount) { false }
    override val minesLeft = mutableIntStateOf(-1)

    override val preferencesLoaded = MutableStateFlow(false).asStateFlow()
    override val puzzleLoaded = MutableStateFlow(false).asStateFlow()

    override var finished = false

    private fun reset() {
        finished = false
        mines.fill(false)
        cells.forEach { it.reset() }
    }

    override fun startNewGame() {
        reset()
        startPuzzle()
        Logger.logEvent(FirebaseAnalytics.Event.LEVEL_START) {
            param(FirebaseAnalytics.Param.LEVEL_NAME, SceneRegistry.Minesweeper.name)
        }
    }

    private fun startPuzzle() {
        Logger.d("[minesweeper] startPuzzle")
        if (finished) reset()
    }

    override fun changeInputMode() {
        inputMode.value =
            if (inputMode.value == InputMode.Flag) InputMode.Normal else InputMode.Flag
    }

    override fun onSelectCell(offset: Offset, cellSize: Float, navController: NavController) {
        if (finished) return
        val column = floor(offset.x / cellSize)
        val row = floor(offset.y / cellSize)
        val cellIndex = if (sizeX < sizeY) {
            (column + row * sizeX).roundToInt()
        } else {
            (sizeY * (column + 1) - (row + 1)).roundToInt()
        }
        val cell = cells[cellIndex]

        cell.pressed = true
        when (cell.state) {
            CellState.Empty -> {
                if (inputMode.value == InputMode.Flag) {
                    cell.state = CellState.Flag
                    calculateMinesLeft()
                } else {
                    if (cell.isMine) return showAllMines()
                    cell.state = CellState.Number
                    if (cell.mineCount == 0) findOtherSafeCells(cell)
                    if (autoFlag.value) autoFlag()
                    checkFinished()
                }
            }

            CellState.Flag -> {
                if (inputMode.value == InputMode.Flag) {
                    cell.state = CellState.Empty
                    calculateMinesLeft()
                }
            }

            else -> {}
        }
    }

    private fun findOtherSafeCells(cell: MinesweeperCell) {
        val index = cell.id
        var i = -1
        while (i <= 1) {
            var j = -2
            while (j < 1) {
                j++
                val mineIndex = if (sizeX < sizeY) index + i + j * sizeX else index + sizeY * i + j
                if (mineIndex !in 0..<cellCount) continue
                if (sizeX < sizeY) {
                    if (mineIndex % sizeX == 0 && i == 1 || index % sizeX == 0 && i == -1) continue
                } else {
                    if (mineIndex % sizeY == 0 && j == 1 || index % sizeY == 0 && j == -1) continue
                }
                val nextCell = cells[mineIndex]
                if (nextCell.state != CellState.Empty) continue
                nextCell.state = CellState.Number
                if (nextCell.mineCount == 0) findOtherSafeCells(nextCell)
            }
            i++
        }
    }

    private fun showAllMines() {
        if (finished) return
        finished = true
        Logger.logEvent(FirebaseAnalytics.Event.LEVEL_END) {
            param(FirebaseAnalytics.Param.LEVEL_NAME, SceneRegistry.Minesweeper.name)
            param(FirebaseAnalytics.Param.SUCCESS, 0)
        }
        cells.forEach {
            if (it.isMine && it.state == CellState.Empty) it.state = CellState.Bomb
        }
    }

    private fun calculateMinesLeft() {
        var count = 0
        cells.forEach { c ->
            if (c.isMine) count++
            if (c.state == CellState.Flag) count--
        }
        minesLeft.intValue = count
    }

    private fun checkFinished() {
        finished = isFinished()
        if (finished) {
            Logger.logEvent(FirebaseAnalytics.Event.LEVEL_END) {
                param(FirebaseAnalytics.Param.LEVEL_NAME, SceneRegistry.Minesweeper.name)
                param(FirebaseAnalytics.Param.SUCCESS, 1)
            }
        }
    }

    private fun isFinished(): Boolean {
        for (cell in cells) {
            if (!cell.isMine && cell.state != CellState.Number) return false
            if (cell.state == CellState.Bomb) return false
        }
        return true
    }

    override fun onClickUpdateAutoFlag() {
        val enabled = !autoFlag.value
        autoFlag.value = enabled
        if (enabled) autoFlag()
    }

    private fun autoFlag() {
        var autoPlacedFlag = false
        for (c in cells) {
            if (!c.isMine || c.state != CellState.Empty) continue
            if (!canBeAutoFlagged(c.id, mutableIntListOf(c.id))) continue
            c.state = CellState.Flag
            autoPlacedFlag = true
        }
        if (autoPlacedFlag) calculateMinesLeft()
    }

    private fun canBeAutoFlagged(mineIndex: Int, safeIndexes: MutableIntList): Boolean {
        val size = min(sizeX, sizeY)
        for (i in 0 until 3) {
            for (j in 0 until 3) {
                val index = mineIndex + i - 1 + (j - 1) * size
                if (index == mineIndex) continue
                if (index < 0 || index >= cells.size) continue
                if (safeIndexes.contains(index)) continue
                if ((index % size == 0 && i == 2) || (mineIndex % size == 0 && i == 0)) continue
                if (cells[index].state == CellState.Number) continue
                if (!cells[index].isMine) return false
                safeIndexes.add(index)
                if (!canBeAutoFlagged(index, safeIndexes)) return false
            }
        }

        return true
    }

    override fun onClickUpdateSafeStart() {
        val enabled = !safeStart.value
        safeStart.value = enabled
    }

    override fun setDifficulty(value: Difficulty) {
        difficulty.value = value
    }
}