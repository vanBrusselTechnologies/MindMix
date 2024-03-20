package com.vanbrusselgames.mindmix.games.sudoku

import androidx.compose.runtime.mutableStateOf
import com.google.firebase.analytics.FirebaseAnalytics
import com.vanbrusselgames.mindmix.BaseLayout
import com.vanbrusselgames.mindmix.Logger
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json

class SudokuManager {
    companion object Instance {
        var inputMode: InputMode = InputMode.Normal
        var loadedPuzzle: LoadedPuzzle? = null
        var cells: Array<SudokuPuzzleCell> = arrayOf()
        var selectedCellIndex: Int = -1

        var checkConflictingCells = false
        var autoEditNotes = false
        var finished = false
        val sudokuFinished = mutableStateOf(finished)
        var difficulty: Difficulty = Difficulty.EASY

        fun saveToFile(): String {
            val clues = cells.filter { c -> c.isClue }.map { c -> c.id }
            val input = cells.map { c -> c.value }
            val notes = cells.map { c ->
                c.notes.mapIndexed { i, n -> if (n) i else -1 }.filter { n -> n != -1 }
            }
            return Json.encodeToString(SudokuData(clues, input, notes, finished))
        }

        fun startPuzzle() {
            if (finished) reset()
            if (loadedPuzzle != null) return
            SudokuLoader.requestPuzzle(difficulty) { p ->
                loadedPuzzle = p
                if(cells.size == p.size * p.size){
                    cells.forEach {
                        it.isClue = p.clues[it.id] != 0
                        it.value = p.clues[it.id]
                    }
                }
                else {
                    cells = Array(p.size * p.size) { SudokuPuzzleCell(it, p.clues[it] != 0, p.clues[it], p.size) }
                }
                if (checkConflictingCells) {
                    cells.forEach { c -> checkConflictingCell(c.id) }
                }
                SudokuLoader.puzzleLoaded.value = true
            }
        }

        private fun reset() {
            finished = false
            sudokuFinished.value = finished
            BaseLayout.disableTopRowButtons.value = false
            SudokuLoader.removePuzzle(loadedPuzzle)
            cells.forEach {
                it.reset()
            }
            loadedPuzzle = null
            selectedCellIndex = -1
        }

        fun startNewGame() {
            reset()
            startPuzzle()
            Logger.logEvent(FirebaseAnalytics.Event.LEVEL_START) {
                param(FirebaseAnalytics.Param.LEVEL_NAME, GAME_NAME)
            }
        }

        fun autoChangeNotes(index: Int) {
            if (loadedPuzzle == null) return
            if (index >= cells.size) return
            if (cells[index].value == 0) return
            val indices: IntArray = SudokuPuzzle.peers(index, cells.size)
            val number: Int = cells[index].value
            for (n: Int in indices) {
                if (index == n) continue
                if (!cells[n].hasNote(number)) continue
                cells[n].setNote(number)
            }
        }

        fun checkConflictingCell(index: Int = 0, isSecondary: Boolean = false) {
            if (loadedPuzzle == null) return
            if (index >= cells.size || index < 0 || cells[index].isClue) return
            val indices: IntArray = SudokuPuzzle.peers(index, cells.size)
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