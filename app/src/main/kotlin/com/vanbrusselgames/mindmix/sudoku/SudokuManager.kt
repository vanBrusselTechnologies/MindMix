package com.vanbrusselgames.mindmix.sudoku

import androidx.compose.runtime.mutableStateOf
import com.google.firebase.analytics.FirebaseAnalytics
import com.vanbrusselgames.mindmix.BaseLayout
import com.vanbrusselgames.mindmix.Logger
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json

class SudokuManager {
    companion object Instance {
        private const val GAME_NAME = "Sudoku"

        var inputMode: InputMode = InputMode.Normal
        private lateinit var puzzle: SudokuPuzzle
        var cells: Array<SudokuPuzzleCell> = arrayOf()
        var selectedCellIndex: Int = -1

        var checkConflictingCells = true
        var autoEditNotes = true
        var finished = false
        val sudokuFinished = mutableStateOf(finished)

        enum class PuzzleType {
            Classic
        }

        enum class InputMode {
            Normal, Note
        }

        fun loadFromFile(data: SudokuData) {
            if (data.finished) {
                startNewGame()
                return
            }
            val cellList = mutableListOf<SudokuPuzzleCell>()
            var clueId = 0
            for (i in data.input.indices) {
                val isClue = data.clues[clueId] == i
                if (isClue) clueId++
                val notes = Array(9) { false }
                for (i2 in data.inputNotes[i]) {
                    notes[i2] = true
                }
                cellList.add(SudokuPuzzleCell(i, isClue, data.input[i], notes))
            }
            cells = cellList.toTypedArray()

            val clues = cells.map { c -> if (c.isClue) c.value else 0 }.toIntArray()
            try {
                puzzle = SudokuPuzzle(SudokuPuzzle.getSolution(clues))
            } catch (e: IllegalArgumentException) {
                Logger.e(e.toString())
                Logger.e("Saved Sudoku puzzle is not valid, loading new puzzle")
                loadPuzzle()
                return
            }

            if (checkConflictingCells) {
                cells.forEach { c -> checkConflictingCell(c.id) }
            }
        }

        fun saveToFile(): String {
            val clues = cells.filter { c -> c.isClue }.map { c -> c.id }
            val input = cells.map { c -> c.value }
            val notes = cells.map { c ->
                c.notes.mapIndexed { i, n -> if (n) i else -1 }.filter { n -> n != -1 }
            }
            return Json.encodeToString(SudokuData(clues, input, notes, finished))
        }

        fun loadPuzzle() {
            if (finished) reset()
            if (cells.isNotEmpty()) return
            Logger.logEvent(FirebaseAnalytics.Event.LEVEL_START) {
                param(FirebaseAnalytics.Param.LEVEL_NAME, GAME_NAME)
            }
            val size = 9
            puzzle = createPuzzle(size = size)
            val clues = SudokuPuzzle.createClues(puzzle, 40)
            cells = Array(size * size) { SudokuPuzzleCell(it, clues[it] != 0, clues[it]) }

        }

        private fun reset() {
            finished = false
            sudokuFinished.value = finished
            BaseLayout.disableTopRowButtons.value = false
            cells = arrayOf()
        }

        fun startNewGame() {
            reset()
            loadPuzzle()
        }

        private fun createPuzzle(
            type: PuzzleType = PuzzleType.Classic, size: Int = 9
        ): SudokuPuzzle {
            return if (type == PuzzleType.Classic) SudokuPuzzle.randomGrid(size)
            else SudokuPuzzle.randomGrid(0)
        }

        fun autoChangeNotes(index: Int) {
            if (index >= 81) return
            if (cells[index].value == 0) return
            val indices: IntArray = puzzle.peers(index)
            val number: Int = cells[index].value
            for (n: Int in indices) {
                if (index == n) continue
                if (!cells[n].hasNote(number)) continue
                cells[n].setNote(number)
            }
        }

        fun checkConflictingCell(index: Int = 0, isSecondary: Boolean = false) {
            if (index >= 81 || index < 0 || cells[index].isClue) return
            val indices: IntArray = puzzle.peers(index)
            var isConflicting = false
            for (n: Int in indices) {
                if (index == n) continue
                when (true) {
                    (cells[index].value == 0) -> {
                        if (!isSecondary && cells[n].isIncorrect) {
                            checkConflictingCell(n, true)
                        }
                        if (cells[n].value == 0) continue
                        if (!cells[index].hasNote(cells[n].value)) continue
                        cells[index].isIncorrect = true
                        isConflicting = true
                    }

                    (cells[index].value == cells[n].value) -> {
                        cells[index].isIncorrect = true
                        if (!isSecondary) checkConflictingCell(n, true)
                        isConflicting = true
                    }

                    (!isSecondary && cells[n].isIncorrect) -> {
                        checkConflictingCell(n, true)
                    }

                    else -> {}
                }
            }
            if (!isConflicting) cells[index].isIncorrect = false
        }

        fun checkFinished() {
            finished = isFinished()
            sudokuFinished.value = finished
            if (finished) {
                BaseLayout.disableTopRowButtons.value = finished
                Logger.logEvent(FirebaseAnalytics.Event.LEVEL_END) {
                    param(FirebaseAnalytics.Param.LEVEL_NAME, GAME_NAME)
                    param(FirebaseAnalytics.Param.SUCCESS, 1)
                }
            }
        }

        private fun isFinished(): Boolean {
            try {
                if (cells.any { c -> c.value == 0 }) return false
                val input = cells.map { c -> c.value }.toIntArray()
                return SudokuPuzzle.getSolution(input).contentEquals(input)
            } catch (_: Exception) {
                return false
            }
        }
    }
}