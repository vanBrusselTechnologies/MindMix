package com.vanbrusselgames.mindmix.games.game2048

import androidx.compose.runtime.mutableLongStateOf
import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.toMutableStateList
import androidx.compose.ui.geometry.Offset
import androidx.compose.ui.util.fastAny
import androidx.compose.ui.util.fastFilter
import androidx.compose.ui.util.fastForEach
import androidx.compose.ui.util.fastRoundToInt
import androidx.datastore.preferences.core.Preferences
import androidx.navigation.NavController
import com.google.firebase.analytics.FirebaseAnalytics
import com.vanbrusselgames.mindmix.core.common.BaseGameViewModel
import com.vanbrusselgames.mindmix.core.logging.Logger
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.feature.gamefinished.navigation.navigateToGameFinished
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json
import kotlin.collections.chunked
import kotlin.collections.indexOfFirst
import kotlin.math.abs
import kotlin.math.log2
import kotlin.math.pow
import kotlin.random.Random

class Game2048ViewModel : BaseGameViewModel() {
    override val nameResId = Game2048.NAME_RES_ID
    override val descResId = R.string.game_2048_desc

    val gridSize = mutableStateOf(GridSize2048.FOUR)
    var sideSize = gridSize.value.getSize()
        private set
    private var cellCount: Int = sideSize * sideSize
    private val startNumbers = 3
    val cellList = mutableStateListOf<GridCell2048>()
    private var newId = cellCount

    var isStuck = false
    var delayedDialog = false

    var score = mutableLongStateOf(0L)

    var savedProgress = Array(GridSize2048.entries.size) {
        Game2048Progress(listOf(), -1, -1, -1, GridSize2048.entries[it], false)
    }

    fun getTarget(): Long = when (gridSize.value) {
        GridSize2048.THREE -> 256
        GridSize2048.FOUR -> 2048
        GridSize2048.FIVE -> 8192
        GridSize2048.SIX -> 32768
        GridSize2048.SEVEN -> 131072
    }

    fun getHighestTileValue() = cellList.map { it.value }.max()

    fun setSize(value: GridSize2048) {
        saveAndLoadProgress(gridSize.value, value)
        gridSize.value = value
    }

    private fun saveAndLoadProgress(prevSize: GridSize2048, size: GridSize2048) {
        if (prevSize === size) return
        saveCurrentProgress(prevSize)

        setProgress(size)
        loadPuzzle()
    }

    private fun saveCurrentProgress(size: GridSize2048 = gridSize.value) {
        val p = savedProgress.first { it.size == size }
        p.cellValues = cellList.map { c -> c.value }
        p.currentScore = score.longValue
        p.isStuck = isStuck
    }

    private fun saveProgress(progress: Game2048Progress) {
        val p = savedProgress.first { it.size == progress.size }
        p.cellValues = progress.cellValues
        p.currentScore = progress.currentScore
        p.isStuck = progress.isStuck
        p.moves = progress.moves
        p.bestScore = progress.bestScore
    }

    private fun setProgress(size: GridSize2048) {
        reset()
        gridSize.value = size
        sideSize = size.getSize()
        cellCount = sideSize * sideSize
        val progress = savedProgress.find { it.size == size }!!
        if (progress.cellValues.isEmpty() || progress.cellValues.all { it == 0L }) return startNewGame()

        cellList.addAll(List(cellCount) {
            GridCell2048(newId++, progress.cellValues[it])
        }.toMutableStateList())
        score.longValue = progress.currentScore
        isStuck = progress.isStuck
        //moves = progress.moves
    }

    fun loadFromFile(data: Game2048Data) {
        gridSize.value = data.size
        for (progress in data.progress) {
            if (progress.cellValues.all { it == 0L }) continue
            saveProgress(progress)
        }
        setProgress(data.size)
    }

    fun saveToFile(): String {
        if (isStuck) {
            val index = savedProgress.indexOfFirst { it.size == gridSize.value }
            savedProgress[index] = Game2048Progress(
                listOf(), -1, -1, savedProgress[index].bestScore, gridSize.value, false
            )
        } else {
            saveCurrentProgress()
        }
        return Json.encodeToString(Game2048Data(gridSize.value, savedProgress.asList()))
    }

    override fun onLoadPreferences(preferences: Preferences) {
    }

    private fun reset() {
        cellList.clear()

        isStuck = false
        delayedDialog = false
        score.longValue = 0L
    }

    /**
     * Loads a puzzle for the game.
     * If no playable puzzle is currently loaded it will start a new game by initializing a fresh puzzle.
     * If a puzzle is already loaded, it will continue with the current state.
     */
    fun loadPuzzle() {
        if (isStuck || cellList.all { it.value == 0L }) startNewGame()
        // otherwise game is already loaded using LoadFromFile()
    }

    override fun startNewGame() {
        reset()
        Logger.logEvent(FirebaseAnalytics.Event.LEVEL_START) {
            param(FirebaseAnalytics.Param.LEVEL_NAME, Game2048.GAME_NAME)
        }

        cellList.addAll(List(cellCount) { GridCell2048(newId++, 0) }.toMutableStateList())
        cellList.shuffle()
        for (i in 0 until startNumbers) {
            cellList[i].value = 2
        }
        cellList.sortBy { it.id }
    }

    fun handleDragGestures(navController: NavController, totalDragOffset: Offset) {
        if (isStuck || delayedDialog || SceneManager.dialogActiveState.value) return
        val x = totalDragOffset.x
        val y = totalDragOffset.y
        if (abs(x) < threshold && abs(y) < threshold) return
        if (abs(x) > abs(y)) { // Horizontal
            if (x > 0) swipeRight(navController) else swipeLeft(navController)
        } else { // Vertical
            if (y > 0) swipeDown(navController) else swipeUp(navController)
        }
    }

    fun swipeUp(navController: NavController) {
        val lastValues = cellList.map { it.value }
        val columns = getColumns()
        combineEqualCells(navController, columns)

        val tempCellList = mutableListOf<GridCell2048>()
        getColumns().forEachIndexed { cId, column ->
            val cells = column.sortedBy { if (it.value == 0L) 1 else 0 }.toMutableList()
            //.filter { it.value != 0L }.toMutableList()
            while (cells.size < sideSize) {
                cells.add(GridCell2048(newId++, 0))
            }
            tempCellList.addAll(cells)
        }

        for (i in 0 until cellCount) cellList[i] = tempCellList[i]
        if (lastValues == cellList.map { it.value }) return
        tryAddCell(navController)
    }

    fun swipeDown(navController: NavController) {
        val lastValues = cellList.map { it.value }
        val columns = getColumns().map { it.reversed() }
        combineEqualCells(navController, columns)

        val tempCellList = mutableListOf<GridCell2048>()
        getColumns().forEachIndexed { cId, column ->
            val cells = column.sortedBy { if (it.value == 0L) 0 else 1 }.toMutableList()
            //.filter { it.value != 0L }.toMutableList()
            while (cells.size < sideSize) {
                cells.add(0, GridCell2048(newId++, 0))
            }
            tempCellList.addAll(cells)
        }

        for (i in 0 until cellCount) cellList[i] = tempCellList[i]
        if (lastValues == cellList.map { it.value }) return
        tryAddCell(navController)
    }

    fun swipeLeft(navController: NavController) {
        val lastValues = cellList.map { it.value }
        val rows = getRows()
        combineEqualCells(navController, rows)

        val newRows = getRows().mapIndexed { rId, row ->
            val cells = row.sortedBy { if (it.value == 0L) 1 else 0 }.toMutableList()
            //.filter { it.value != 0L }.toMutableList()
            while (cells.size < sideSize) {
                cells.add(GridCell2048(newId++, 0))
            }
            cells
        }

        var i = 0
        for (cId in 0 until sideSize) {
            for (rId in 0 until sideSize) {
                cellList[i] = newRows[rId][cId]
                i++
            }
        }

        if (lastValues == cellList.map { it.value }) return
        tryAddCell(navController)
    }

    fun swipeRight(navController: NavController) {
        val lastValues = cellList.map { it.value }
        val rows = getRows().map { it.reversed() }
        combineEqualCells(navController, rows)

        val newRows = getRows().mapIndexed { rId, row ->
            val cells = row.sortedBy { if (it.value == 0L) 0 else 1 }.toMutableList()
            //.filter { it.value != 0L }.toMutableList()
            while (cells.size < sideSize) {
                cells.add(0, GridCell2048(newId++, 0))
            }
            cells
        }

        var i = 0
        for (cId in 0 until sideSize) {
            for (rId in 0 until sideSize) {
                cellList[i] = newRows[rId][cId]
                i++
            }
        }

        if (lastValues == cellList.map { it.value }) return
        tryAddCell(navController)
    }

    private fun getColumns(): List<List<GridCell2048>> {
        return cellList.chunked(sideSize)
    }

    private fun getRows(): List<List<GridCell2048>> {
        val rows = List(sideSize) { mutableListOf<GridCell2048>() }
        getColumns().forEach { c ->
            c.forEachIndexed { rId, r ->
                rows[rId].add(r)
            }
        }
        return rows
    }

    private fun combineEqualCells(
        navController: NavController, cells: List<List<GridCell2048>>
    ) {
        var reachedTarget = false
        var points = 0L
        for (r in cells) {
            var v = 0L
            var cId = -1
            for (c in r) {
                if (c.value == 0L) continue
                if (v == 0L || v != c.value) {
                    v = c.value
                    cId = c.id
                    continue
                }
                val cell = r.find { it.id == cId }
                if (cell == null) continue
                if (v * 2 == getTarget() && getHighestTileValue() == v) reachedTarget = true
                c.value = v * 2
                cell.value = 0
                points += v * 2
                v = 0
            }
        }
        addScore(points)
        if (reachedTarget) CoroutineScope(Dispatchers.Main).launch {
            delayedDialog = true
            delay(500)
            onGameFinished(navController, true)
            delayedDialog = false
        }
    }

    private fun tryAddCell(navController: NavController) {
        val options = cellList.fastFilter { c -> c.value == 0L }
        if (options.isEmpty()) {
            checkStuck(navController)
            return
        }
        val cell = options.random()
        cellList[cellList.indexOf(cell)] =
            GridCell2048(newId++, if (Random.nextFloat() < 0.9) 2 else 4)
        checkStuck(navController)
    }

    private fun checkStuck(navController: NavController) {
        isStuck = !canMove()
        if (isStuck) CoroutineScope(Dispatchers.Main).launch {
            delayedDialog = true
            delay(1000)
            onGameFinished(navController, getHighestTileValue() >= getTarget())
            delayedDialog = false
        }
    }

    private fun canMove(): Boolean {
        if (cellList.fastAny { c -> c.value == 0L }) return true
        val rows = getRows()
        rows.fastForEach {
            var value = 0L
            for (c in it) {
                if (c.value == value) return true
                value = c.value
            }
        }
        val columns = getColumns()
        columns.fastForEach {
            var value = 0L
            for (c in it) {
                if (c.value == value) return true
                value = c.value
            }
        }
        return false
    }

    private fun addScore(points: Long) {
        score.longValue += points
    }

    private fun onGameFinished(navController: NavController, reachedTarget: Boolean) {
        saveCurrentProgress()
        FinishedGame.titleResId =
            if (!isStuck && reachedTarget) R.string.game_2048_reach_target_title else if (isStuck && !reachedTarget) R.string.game_2048_game_over_title else Game2048.NAME_RES_ID
        FinishedGame.textResId =
            if (!isStuck && reachedTarget) R.string.game_2048_reach_target_text else if (isStuck && !reachedTarget) R.string.game_2048_game_over_text else R.string.game_2048_success
        FinishedGame.score = score.longValue
        FinishedGame.highestTileValue = getHighestTileValue()
        FinishedGame.targetTile = getTarget()
        FinishedGame.isStuck = isStuck
        FinishedGame.reward = if (reachedTarget && isStuck) 10 + getBonusReward() else 0
        navController.navigateToGameFinished()
    }

    private fun getBonusReward(): Int {
        return 1.5.pow((log2(getHighestTileValue().toDouble()) - log2(getTarget().toDouble())))
            .fastRoundToInt()
    }
}