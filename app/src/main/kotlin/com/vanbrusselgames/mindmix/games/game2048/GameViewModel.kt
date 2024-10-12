package com.vanbrusselgames.mindmix.games.game2048

import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.toMutableStateList
import androidx.compose.ui.util.fastAny
import androidx.compose.ui.util.fastFilter
import androidx.compose.ui.util.fastForEach
import androidx.lifecycle.ViewModel
import kotlin.random.Random

class GameViewModel : ViewModel() {
    val sideSize = 4
    private val cellCount: Int = sideSize * sideSize
    private val startNumbers = 3
    val cellList = List(cellCount) { GridCell2048(it, 0) }.toMutableStateList()
    private var newId = cellCount

    val isStuck = mutableStateOf(false)

    init {
        if(cellList.any{it.value == 0L}) {
            cellList.shuffle()
            for (i in 0 until startNumbers) {
                cellList[i].value = 2
            }
            cellList.sortBy { it.id }
        }
    }

    fun swipeUp() {
        val columns = getColumns()
        combineEqualCells(columns)

        val tempCellList = mutableListOf<GridCell2048>()
        getColumns().forEachIndexed { cId, column ->
            val cells = column.filter { it.value != 0L }.toMutableList()
            while (cells.size < sideSize) {
                cells.add(GridCell2048(newId++, 0))
            }
            tempCellList.addAll(cells)
        }

        for (i in 0 until cellCount) {
            cellList[i] = tempCellList[i]
        }

        tryAddCell()
    }

    fun swipeDown() {
        val columns = getColumns().map { it.reversed() }
        combineEqualCells(columns)

        val tempCellList = mutableListOf<GridCell2048>()
        getColumns().forEachIndexed { cId, column ->
            val cells = column.filter { it.value != 0L }.toMutableList()
            while (cells.size < sideSize) {
                cells.add(0, GridCell2048(newId++, 0))
            }
            tempCellList.addAll(cells)
        }

        for (i in 0 until cellCount) {
            cellList[i] = tempCellList[i]
        }

        tryAddCell()
    }

    fun swipeLeft() {
        val rows = getRows()
        combineEqualCells(rows)

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

        tryAddCell()
    }

    fun swipeRight() {
        val rows = getRows().map { it.reversed() }
        combineEqualCells(rows)

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

        tryAddCell()
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

    private fun combineEqualCells(cells: List<List<GridCell2048>>) {
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
                c.value = v * 2
                cell.value = 0
                v = 0
            }
        }
    }

    private fun tryAddCell() {
        val options = cellList.fastFilter { c -> c.value == 0L }
        if (options.isEmpty()) {
            isStuck.value = !canMove()
            return
        }
        val cell = options.random()
        cell.value = if (Random.nextFloat() < 0.9) 2 else 4
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
}