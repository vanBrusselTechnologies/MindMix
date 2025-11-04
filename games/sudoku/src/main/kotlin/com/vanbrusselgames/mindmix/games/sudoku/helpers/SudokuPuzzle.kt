package com.vanbrusselgames.mindmix.games.sudoku.helpers

import androidx.collection.mutableIntListOf
import kotlin.math.sqrt
import kotlin.random.Random

class SudokuPuzzle private constructor(private val n: Int) {
    private val boxSize = sqrt(n.toFloat()).toInt()
    private var cells = Array(n * n) { IntArray(n) }

    init {
        val a1 = IntArray(n) { 1 + it }
        var i = 0
        while (i < n * n) {
            System.arraycopy(a1, 0, cells[i], 0, n)
            i++
        }
    }

    constructor(input: IntArray) : this(n = sqrt(input.size.toFloat()).toInt()) {
        for (i: Int in input.indices) {
            if (input[i] !in 1..n) continue
            val puzzle = placeValue(i, input[i])
                ?: throw IllegalArgumentException("This puzzle is unsolvable!")
            cells = puzzle.cells
        }
    }

    private fun clone(): SudokuPuzzle {
        val clone = SudokuPuzzle(n)
        var i = cells.size - 1
        while (i >= 0) {
            clone.cells[i] = IntArray(cells[i].size)
            System.arraycopy(cells[i], 0, clone.cells[i], 0, cells[i].size)
            i--
        }

        return clone
    }

    private fun isPeer(c1: Int, c2: Int): Boolean {
        return (c1 / n == c2 / n //Cells in same row
                || c1 % n == c2 % n //Cells in same column
                || (c1 / (n * boxSize) == c2 / (n * boxSize) && c1 % n / boxSize == c2 % n / boxSize)) //Cells in same box
                && c1 != c2 //Cell is not peer to itself
    }

    companion object {
        //Dictionary to memoize lists of peers
        //The key is a pair of <int, int>, where the first value is the length of the puzzle (typically 9) and the second value is the cell index
        private val SavedPeers = mutableMapOf<Pair<Int, Int>, IntArray>()

        private fun findSingularizedCells(
            puzzle1: SudokuPuzzle, puzzle2: SudokuPuzzle, cellIndex: Int
        ): MutableList<Int> {
            val result = mutableListOf<Int>()
            val peers = puzzle1.peers(cellIndex)
            for (i: Int in peers) {
                if (puzzle1.cells[i].size > 1 && puzzle2.cells[i].size == 1) result.add(i)
            }
            return result
        }

        private fun multiSolve(
            input: SudokuPuzzle, maximumSolutions: Int = -1
        ): List<SudokuPuzzle> {
            val solutions = mutableListOf<SudokuPuzzle>()
            input.solve { p ->
                solutions.add(p)
                return@solve solutions.size < maximumSolutions || maximumSolutions == -1
            }
            return solutions
        }

        fun getSolution(clues: IntArray): IntArray {
            val solution = IntArray(clues.size)
            val puzzle = SudokuPuzzle(clues).solve()
                ?: throw IllegalArgumentException("Can't create solution from an unsolvable puzzle!")
            for (i: Int in puzzle.cells.indices) {
                val cell: IntArray = puzzle.cells[i]
                if (cell.isEmpty()) throw IllegalArgumentException("Can't create solution from an unsolvable puzzle!")
                solution[i] = cell[0]
            }
            return solution
        }

        fun getSolution(sudokuPuzzle: SudokuPuzzle): IntArray {
            val solution = IntArray(sudokuPuzzle.n * sudokuPuzzle.n)
            val puzzle = sudokuPuzzle.solve()
                ?: throw IllegalArgumentException("Can't create solution from an unsolvable puzzle!")
            for (i: Int in puzzle.cells.indices) {
                val cell: IntArray = puzzle.cells[i]
                if (cell.isEmpty()) throw IllegalArgumentException("Can't create solution from an unsolvable puzzle!")
                solution[i] = cell[0]
            }
            return solution
        }

        fun randomGrid(size: Int): SudokuPuzzle {
            var puzzle = SudokuPuzzle(size)
            while (true) {
                val unsolvedCellIndexesList = mutableListOf<Int>()
                val candidateCount = puzzle.cells.size
                var i = 0
                while (i < candidateCount) {
                    if (puzzle.cells[i].size >= 2) unsolvedCellIndexesList.add(i)
                    i++
                }
                val unsolvedCellIndexes: IntArray = unsolvedCellIndexesList.toIntArray()
                val cellIndex: Int =
                    unsolvedCellIndexes[Random.nextInt(unsolvedCellIndexes.size)]
                val candidateValue: Int =
                    puzzle.cells[cellIndex][Random.nextInt(puzzle.cells[cellIndex].size)]
                val workingPuzzle = puzzle.placeValue(cellIndex, candidateValue)
                if (workingPuzzle != null) {
                    val solutions = multiSolve(workingPuzzle, 2)
                    when (solutions.size) {
                        0 -> continue
                        1 -> return solutions[0]
                        else -> puzzle = workingPuzzle
                    }
                }
            }
        }

        fun createClues(input: SudokuPuzzle, maxClues: Int = 0): IntArray {
            //Get a unique solution for the input puzzle, in case it wasn't already
            val puzzle = input.solve()
                ?: throw IllegalArgumentException("Can't create clues from an unsolvable puzzle!")

            //This is the list of clues we work on.  It can be reconstituted into a new puzzle object by way of a constructor
            val size = puzzle.cells.size
            var clues = IntArray(size)
            var clueI = 0
            while (clueI < size) {
                clues[clueI] = puzzle.cells[clueI][0]
                clueI++
            }
            val testedClues = mutableIntListOf()
            while (true) {
                if (testedClues.size == size) return clues
                //Pick a random cell to blank
                val clueCell: Int = Random.nextInt(clues.size)

                if (testedClues.contains(clueCell)) continue
                if (clues[clueCell] == 0) {
                    testedClues.add(clueCell)
                    continue
                }
                val workingClues: IntArray = clues.copyOf()
                workingClues[clueCell] = 0
                if (multiSolve(SudokuPuzzle(workingClues), 2).size > 1) {
                    if (maxClues == 0) return clues
                    testedClues.add(clueCell)
                    continue
                }

                clues = workingClues
                if (clues.filter { it != 0 }.size <= maxClues) return clues
            }
        }

        fun peers(cell: Int, size: Int): IntArray {
            val n = sqrt(size.toFloat()).toInt()
            val boxSize = sqrt(n.toFloat()).toInt()
            val key = Pair(n, cell)
            if (SavedPeers.containsKey(key)) return SavedPeers[key]!!
            val items = mutableListOf<Int>()
            var i = 0

            while (i < n * n) {
                if (isPeer(cell, i, n, boxSize)) items.add(i)
                i++
            }
            SavedPeers[key] = items.toIntArray()
            return SavedPeers[key]!!
        }

        private fun isPeer(c1: Int, c2: Int, n: Int, boxSize: Int): Boolean {
            return (c1 / n == c2 / n //Cells in same row
                    || c1 % n == c2 % n //Cells in same column
                    || (c1 / (n * boxSize) == c2 / (n * boxSize) && c1 % n / boxSize == c2 % n / boxSize)) //Cells in same box
                    && c1 != c2 //Cell is not peer to itself
        }
    }

    fun peers(cell: Int): IntArray {
        val key = Pair(n, cell)
        if (SavedPeers.containsKey(key)) return SavedPeers[key]!!
        val items = mutableListOf<Int>()
        var i = 0
        while (i < n * n) {
            if (isPeer(cell, i)) items.add(i)
            i++
        }
        SavedPeers[key] = items.toIntArray()
        return SavedPeers[key]!!
    }

    private fun applyConstraints(cellIndex: Int, value: Int): SudokuPuzzle? {
        val puzzle = clone()

        //Standard Sudoku constraint logic: Set this cell to one and only one candidate, and remove this value from the candidate list of all its peers
        puzzle.cells[cellIndex] = intArrayOf(value)

        val peers = puzzle.peers(cellIndex)
        for (peerIndex in peers) {
            val newPeers = puzzle.cells[peerIndex].filter { it != value }
            if (newPeers.isEmpty()) return null
            puzzle.cells[peerIndex] = newPeers.toIntArray()
        }

        return puzzle
    }

    private fun placeValue(cellIndex: Int, value: Int): SudokuPuzzle? {
        if (!cells[cellIndex].contains(value)) return null
        var puzzle = applyConstraints(cellIndex, value) ?: return null
        for (i in findSingularizedCells(this, puzzle, cellIndex)) {
            val cellValue = if (puzzle.cells[i].size == 1) puzzle.cells[i][0] else return null
            puzzle = puzzle.placeValue(i, cellValue) ?: return null
        }
        return puzzle
    }

    private fun findWorkingCell(): Int {
        var i = 0
        val list = mutableListOf<IntArray>()
        while (i < cells.size) {
            if (cells[i].size >= 2) list.add(cells[i])
            i++
        }
        var minCandidates = n + 1
        var minItem: IntArray = list[0]
        for (item in list) {
            val size = item.size
            if (size == 2) return cells.indexOf(item)
            if (size < minCandidates) {
                minCandidates = size
                minItem = item
            }
        }
        return cells.indexOf(minItem)
    }

    private fun solve(solutionFunc: ((SudokuPuzzle) -> Boolean)? = null): SudokuPuzzle? {
        var i = 0
        var singleOptionCells = 0
        while (i < cells.size) {
            if (cells[i].size == 1) singleOptionCells++
            i++
        }
        if (singleOptionCells == cells.size) return if (solutionFunc != null && solutionFunc(this)) null else this

        val activeCell: Int = findWorkingCell()
        for (guess in cells[activeCell]) {
            var puzzle: SudokuPuzzle? = placeValue(activeCell, guess) ?: continue
            puzzle = puzzle?.solve(solutionFunc)
            if (puzzle != null) return puzzle
        }
        return null
    }

    override fun toString(): String {
        val sb = StringBuilder()

        for (t in cells) {
            if (t.size == 1) sb.append(t[0])
            else sb.append('.')
        }
        val output = sb.toString()
        sb.clear()
        return output
    }
}