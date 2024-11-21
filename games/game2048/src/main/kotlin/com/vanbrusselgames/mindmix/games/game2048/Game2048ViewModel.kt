package com.vanbrusselgames.mindmix.games.game2048

import androidx.compose.runtime.mutableLongStateOf
import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.toMutableStateList
import androidx.compose.ui.util.fastAny
import androidx.compose.ui.util.fastFilter
import androidx.compose.ui.util.fastForEach
import androidx.compose.ui.util.fastRoundToInt
import androidx.datastore.preferences.core.Preferences
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.common.BaseGameViewModel
import com.vanbrusselgames.mindmix.feature.gamefinished.navigation.navigateToGameFinished
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch
import kotlin.collections.chunked
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

    fun getTarget(): Long = when (gridSize.value) {
        GridSize2048.THREE -> 256
        GridSize2048.FOUR -> 2048
        GridSize2048.FIVE -> 8192
        GridSize2048.SIX -> 32768
        GridSize2048.SEVEN -> 131072
    }

    fun getHighestTileValue() = cellList.map { it.value }.max()

    init {
        // TODO: Load progress
        if (cellList.isEmpty() || cellList.all { it.value == 0L }) {
            startNewGame()
        }
    }

    override fun onLoadPreferences(preferences: Preferences) {

    }

    private fun reset() {
        cellList.clear()

        isStuck = false
        delayedDialog = false
        score.longValue = 0L
    }

    override fun startNewGame() {
        reset()

        cellList.addAll(List(cellCount) { GridCell2048(newId++, 0) }.toMutableStateList())
        cellList.shuffle()
        for (i in 0 until startNumbers) {
            cellList[i].value = 2
        }
        cellList.sortBy { it.id }
    }

    fun setSize(value: GridSize2048) {
        saveAndLoadProgress(gridSize.value, value)
        gridSize.value = value
    }

    private fun saveAndLoadProgress(prevSize: GridSize2048, size: GridSize2048) {
        // todo: Load Actual progress for size, instead of creating new puzzle.

        //if (currentPuzzle != null) setCurrentProgress(prevDifficulty)

        //val progress = savedProgress.find { it.size == size }!!
        //if (progress.cellList.isEmpty()) return
        //currentPuzzle = LoadedPuzzle(SIZE, difficulty, progress.clues)

        if (prevSize === size) return
        sideSize = size.getSize()
        cellCount = sideSize * sideSize

        cellList.clear()
        cellList.addAll(List(cellCount) { GridCell2048(newId++, 0) }.toMutableStateList())

        cellList.shuffle()
        for (i in 0 until startNumbers) {
            cellList[i].value = 2
        }
        cellList.sortBy { it.id }
        score.longValue = 0L
    }

    fun swipeUp(navController: NavController) {
        val lastValues = cellList.map { it.value }
        val columns = getColumns()
        combineEqualCells(navController, columns)

        val tempCellList = mutableListOf<GridCell2048>()
        getColumns().forEachIndexed { cId, column ->
            val cells = column.filter { it.value != 0L }.toMutableList()
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
            val cells = column.filter { it.value != 0L }.toMutableList()
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
            val cells = row.filter { it.value != 0L }.toMutableList()
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
            val cells = row.filter { it.value != 0L }.toMutableList()
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

    private fun combineEqualCells(navController: NavController, cells: List<List<GridCell2048>>) {
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
        cell.value = if (Random.nextFloat() < 0.9) 2 else 4
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
        getRows().fastForEach {
            var value = 0L
            for (c in it) {
                if (c.value == value) return true
                value = c.value
            }
        }
        getColumns().fastForEach {
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
        // TODO : Localize CORRECT title / text
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