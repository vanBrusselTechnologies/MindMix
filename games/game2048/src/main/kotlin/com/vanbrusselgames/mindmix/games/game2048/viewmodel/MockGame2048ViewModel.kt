package com.vanbrusselgames.mindmix.games.game2048.viewmodel

import androidx.compose.runtime.mutableLongStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.snapshots.SnapshotStateList
import androidx.compose.ui.geometry.Offset
import androidx.compose.ui.util.fastAny
import androidx.compose.ui.util.fastFilter
import androidx.compose.ui.util.fastForEach
import androidx.compose.ui.util.fastRoundToInt
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.common.viewmodel.BaseGameViewModel
import com.vanbrusselgames.mindmix.games.game2048.model.FinishedGame
import com.vanbrusselgames.mindmix.games.game2048.model.GridCell2048
import com.vanbrusselgames.mindmix.games.game2048.model.GridSize2048
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import kotlin.math.abs
import kotlin.math.log2
import kotlin.math.pow
import kotlin.random.Random

class MockGame2048ViewModel : BaseGameViewModel(), IGame2048ViewModel {
    override val finishedGame = mutableStateOf(FinishedGame())
    override val gridSize = mutableStateOf(GridSize2048.FOUR)
    override val cellList =
        SnapshotStateList(gridSize.value.getMaxCellCount()) { GridCell2048(it, 0) }
    override val score = mutableLongStateOf(0L)

    override val preferencesLoaded = MutableStateFlow(false).asStateFlow()
    override val puzzleLoaded = MutableStateFlow(false).asStateFlow()

    override var finished = false

    private val threshold = 100

    private var cellCount = gridSize.value.getMaxCellCount()
    private var sideSize = gridSize.value.getSize()
    private var newId = cellCount

    private fun getTarget(): Long = when (gridSize.value) {
        GridSize2048.THREE -> 256
        GridSize2048.FOUR -> 2048
        GridSize2048.FIVE -> 8192
        GridSize2048.SIX -> 32768
        GridSize2048.SEVEN -> 131072
    }

    private fun getHighestTileValue() = cellList.maxOf { it.value }

    override fun startNewGame() {}

    override fun handleDragGestures(navController: NavController, totalDragOffset: Offset) {
        if (finished) return
        val x = totalDragOffset.x
        val y = totalDragOffset.y
        if (abs(x) < threshold && abs(y) < threshold) return
        if (abs(x) > abs(y)) { // Horizontal
            if (x > 0) swipeRight(navController) else swipeLeft(navController)
        } else { // Vertical
            if (y > 0) swipeDown(navController) else swipeUp(navController)
        }
    }

    private fun swipeUp(navController: NavController) {
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

    private fun swipeDown(navController: NavController) {
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

    private fun swipeLeft(navController: NavController) {
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

    private fun swipeRight(navController: NavController) {
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
            finished = true
        }
    }

    private fun tryAddCell(navController: NavController) {
        val options = cellList.fastFilter { it.value == 0L }
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
        if (!canMove()) CoroutineScope(Dispatchers.Main).launch {
            finished = true
        }
    }

    private fun canMove(): Boolean {
        if (cellList.fastAny { it.value == 0L }) return true
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

    private fun getBonusReward(): Int {
        return 1.5.pow((log2(getHighestTileValue().toDouble()) - log2(getTarget().toDouble())))
            .fastRoundToInt()
    }

    override fun continueGame() {
        finished = false
    }

    override fun setSize(value: GridSize2048) {
        gridSize.value = value
        sideSize = value.getSize()
        cellCount = value.getMaxCellCount()
    }
}