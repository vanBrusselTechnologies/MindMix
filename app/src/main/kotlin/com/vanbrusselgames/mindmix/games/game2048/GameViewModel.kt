package com.vanbrusselgames.mindmix.games.game2048

import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.toMutableStateList
import androidx.lifecycle.ViewModel
import kotlin.random.Random

class GameViewModel : ViewModel() {
    val sideSize = 4
    private val cellCount: Int = sideSize * sideSize
    private val startNumbers = 3
    val cellList =
        List(cellCount) { GridCell2048(it, 0, it % sideSize, it / sideSize) }.toMutableStateList()
    private var newId = cellCount

    val isStuck = mutableStateOf(false)

    init {
        cellList.shuffle()
        for (i in 0 until startNumbers) {
            cellList[i].value = 2
        }
        cellList.sortBy { it.id }
    }

    fun swipeUp() {
        val columns = getColumns()
        combineEqualCells(columns)

        val tempCellList = mutableListOf<GridCell2048>()
        getColumns().forEachIndexed { cId, column ->
            val cells = column.filter { it.value != 0 }.mapIndexed { i, cell ->
                cell.rowIndex = i
                cell
            }.toMutableList()
            while (cells.size < sideSize) {
                cells.add(GridCell2048(newId++, 0, cId, cells.size))
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
            val cells = column.filter { it.value != 0 }.toMutableList()
            while (cells.size < sideSize) {
                cells.add(0, GridCell2048(newId++, 0, cId, cells.size))
            }
            cells.forEachIndexed { i, cell -> cell.rowIndex = i }
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
            val cells = row.filter { it.value != 0 }.mapIndexed { i, cell ->
                cell.columnIndex = i
                cell
            }.toMutableList()
            while (cells.size < sideSize) {
                cells.add(GridCell2048(newId++, 0, cells.size, rId))
            }
            cells
        }

        var i = 0
        for (cId in 0 until sideSize) {
            for (rId in 0 until sideSize) {
                cellList[i++] = newRows[rId][cId]
            }
        }

        tryAddCell()
    }

    fun swipeRight() {
        val rows = getRows().map { it.reversed() }
        combineEqualCells(rows)

        val newRows = getRows().mapIndexed { rId, row ->
            val cells = row.filter { it.value != 0 }.toMutableList()
            while (cells.size < sideSize) {
                cells.add(0, GridCell2048(newId++, 0, cells.size, rId))
            }
            cells.forEachIndexed { i, cell -> cell.rowIndex = i }
            cells
        }

        var i = 0
        for (cId in 0 until sideSize) {
            for (rId in 0 until sideSize) {
                cellList[i++] = newRows[rId][cId]
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
            var v = 0
            var cId = -1
            for (c in r) {
                if (c.value == 0) continue
                if (v == 0 || v != c.value) {
                    v = c.value
                    cId = c.id
                    continue
                }
                r.find { it.id == cId }?.value = v * 2
                v = 0
                c.value = 0
            }
        }
    }

    private fun tryAddCell() {
        val options = cellList.filter { c -> c.value == 0 }
        if (options.isEmpty()) {
            isStuck.value = !canMove()
            return
        }
        options.random().value = 2 + if (Random.nextBoolean()) 2 else 0
    }

    private fun canMove(): Boolean {
        return cellList.any { c -> c.value == 0 } /*|| todo: check if same numbers are next to each other: They can still be merged*/
    }
}