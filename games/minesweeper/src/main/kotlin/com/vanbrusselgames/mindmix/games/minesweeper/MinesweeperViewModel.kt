package com.vanbrusselgames.mindmix.games.minesweeper

import androidx.collection.MutableIntList
import androidx.collection.mutableIntListOf
import androidx.compose.runtime.MutableState
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.ui.graphics.Color
import androidx.datastore.preferences.core.Preferences
import androidx.navigation.NavController
import com.google.firebase.analytics.FirebaseAnalytics
import com.vanbrusselgames.mindmix.core.common.BaseGameViewModel
import com.vanbrusselgames.mindmix.core.logging.Logger
import com.vanbrusselgames.mindmix.feature.gamefinished.navigation.navigateToGameFinished
import com.vanbrusselgames.mindmix.games.minesweeper.data.PREF_KEY_AUTO_FLAG
import com.vanbrusselgames.mindmix.games.minesweeper.data.PREF_KEY_SAFE_START
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json
import kotlin.math.min
import kotlin.math.pow
import kotlin.random.Random

class MinesweeperViewModel : BaseGameViewModel() {
    override val nameResId = Minesweeper.NAME_RES_ID
    override val descResId = R.string.minesweeper_desc

    var inputMode: InputMode = InputMode.Normal
    var sizeX = 16
    var sizeY = 22

    var finished = false

    var safeStart = mutableStateOf(false)
    var autoFlag = mutableStateOf(false)

    val mines = BooleanArray(sizeX * sizeY) { false }

    val cellCount
        get() = sizeX * sizeY
    var cells = Array(cellCount) {
        MinesweeperCell(this, it)
    }

    val minesLeft = mutableIntStateOf(-1)

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
        if (autoFlag.value) autoFlag()
        calculateMinesLeft()
    }

    override fun onLoadPreferences(preferences: Preferences) {
        if (preferences[PREF_KEY_AUTO_FLAG] != null) {
            autoFlag.value = preferences[PREF_KEY_AUTO_FLAG]!!
        }
        if (preferences[PREF_KEY_SAFE_START] != null) {
            safeStart.value = preferences[PREF_KEY_SAFE_START]!!
        }
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
        if (mines.any { it }) return
        Logger.logEvent(FirebaseAnalytics.Event.LEVEL_START) {
            param(FirebaseAnalytics.Param.LEVEL_NAME, Minesweeper.GAME_NAME)
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

        if (safeStart.value) {
            while (true) {
                val randomIndex: Int = Random.nextInt(cellCount)
                val c = cells[randomIndex]
                if (c.isMine || c.mineCount != 0) continue
                c.state = MinesweeperCell.State.Number
                findOtherSafeCells(c)
                if (autoFlag.value) autoFlag()
                break
            }
        }
    }

    private fun reset() {
        finished = false
        mines.fill(false)
        cells.forEach { c ->
            c.isMine = false
            c.state = MinesweeperCell.State.Empty
            c.pressed = false
            c.background.value = Color.White
        }
    }

    override fun startNewGame() {
        reset()
        loadPuzzle()
    }

    fun findOtherSafeCells(cell: MinesweeperCell) {
        val index = cell.id
        var i = -1
        while (i <= 1) {
            var j = -2
            while (j < 1) {
                j++
                val mineIndex = if (sizeX < sizeY) index + i + j * sizeX else index + sizeY * i + j
                if (mineIndex < 0 || mineIndex >= cellCount) continue
                if (sizeX < sizeY) {
                    if (mineIndex % sizeX == 0 && i == 1 || index % sizeX == 0 && i == -1) continue
                } else {
                    if (mineIndex % sizeY == 0 && j == 1 || index % sizeY == 0 && j == -1) continue
                }
                val nextCell = cells[mineIndex]
                if (nextCell.state != MinesweeperCell.State.Empty) continue
                nextCell.state = MinesweeperCell.State.Number
                if (nextCell.mineCount == 0) findOtherSafeCells(nextCell)
            }
            i++
        }
    }

    fun showAllMines(navController: NavController) {
        if (finished) return
        finished = true
        Logger.logEvent(FirebaseAnalytics.Event.LEVEL_END) {
            param(FirebaseAnalytics.Param.LEVEL_NAME, Minesweeper.GAME_NAME)
            param(FirebaseAnalytics.Param.SUCCESS, 0)
        }
        var i = -1
        while (i < cellCount - 1) {
            i++
            if (!cells[i].isMine || cells[i].state != MinesweeperCell.State.Empty) continue
            cells[i].state = MinesweeperCell.State.Bomb
        }
        onGameFinished(navController, false)
    }

    fun calculateMinesLeft() {
        var count = 0
        cells.forEach { c ->
            if (c.isMine) count++
            if (c.state == MinesweeperCell.State.Flag) count--
        }
        minesLeft.intValue = count
    }

    fun checkFinished(navController: NavController) {
        finished = isFinished()
        if (finished) {
            Logger.logEvent(FirebaseAnalytics.Event.LEVEL_END) {
                param(FirebaseAnalytics.Param.LEVEL_NAME, Minesweeper.GAME_NAME)
                param(FirebaseAnalytics.Param.SUCCESS, 1)
            }
            onGameFinished(navController, true)
        }
    }

    private fun isFinished(): Boolean {
        for (cell in cells) {
            if (!cell.isMine && cell.state != MinesweeperCell.State.Number) return false
            if (cell.state == MinesweeperCell.State.Bomb) return false
        }
        return true
    }

    fun changeInputMode(inputModeState: MutableState<InputMode>) {
        inputMode = if (inputMode == InputMode.Flag) InputMode.Normal else InputMode.Flag
        inputModeState.value = inputMode
    }

    fun autoFlag() {
        var autoPlacedFlag = false
        for (c in cells) {
            if (!c.isMine || c.state != MinesweeperCell.State.Empty) continue
            if (!canBeAutoFlagged(c.id, mutableIntListOf(c.id))) continue
            c.state = MinesweeperCell.State.Flag
            autoPlacedFlag = true
        }
        if (autoPlacedFlag) calculateMinesLeft()
    }

    private fun canBeAutoFlagged(
        mineIndex: Int, safeIndexes: MutableIntList
    ): Boolean {
        val size = min(sizeX, sizeY)
        for (i in 0 until 3) {
            for (j in 0 until 3) {
                val index = mineIndex + i - 1 + (j - 1) * size
                if (index == mineIndex) continue
                if (index < 0 || index >= cells.size) continue
                if (safeIndexes.contains(index)) continue
                if ((index % size == 0 && i == 2) || (mineIndex % size == 0 && i == 0)) continue
                if (cells[index].state == MinesweeperCell.State.Number) continue
                if (!cells[index].isMine) return false
                safeIndexes.add(index)
                if (!canBeAutoFlagged(index, safeIndexes)) return false
            }
        }

        return true
    }

    private fun onGameFinished(navController: NavController, success: Boolean) {
        FinishedGame.titleResId = Minesweeper.NAME_RES_ID
        // if (failed) "Failed" else "Congrats / Smart / Well done"
        FinishedGame.textResId =
            if (!success) R.string.minesweeper_failed else R.string.minesweeper_success
        // if (failed) "A mine exploded" else """You did great and solved puzzle in ${0} seconds!!
        //     |That's Awesome!
        //     |Share with your friends and challenge them to beat your time!""".trimMargin()
        FinishedGame.reward = if (!success) 0 else 10

        navController.navigateToGameFinished()
    }
}