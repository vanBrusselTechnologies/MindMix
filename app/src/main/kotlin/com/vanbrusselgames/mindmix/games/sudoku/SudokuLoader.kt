package com.vanbrusselgames.mindmix.games.sudoku

import androidx.compose.runtime.mutableStateOf
import com.google.common.math.IntMath.sqrt
import com.vanbrusselgames.mindmix.Logger
import kotlinx.coroutines.coroutineScope
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch
import java.math.RoundingMode

class SudokuLoader {
    companion object {
        val puzzleLoaded = mutableStateOf(false)
        private val loadedPuzzles = mutableListOf<LoadedPuzzle>()

        private var requestingCallback: ((LoadedPuzzle) -> Unit)? = null

        fun requestPuzzle(diff: Difficulty, callback: (LoadedPuzzle) -> Unit) {
            val p = loadedPuzzles.firstOrNull { it.difficulty == diff }
            if (p != null) {
                callback(p)
            } else requestingCallback = callback
        }

        fun removePuzzle(p: LoadedPuzzle?) {
            if (p == null) return
            val index = loadedPuzzles.indexOfFirst {
                it.clues.joinToString("") == p.clues.joinToString("")
            }
            if (index != -1) {
                loadedPuzzles.removeAt(index)
                puzzleLoaded.value = false
            }
        }

        fun loadFromFile(data: SudokuData) {
            if (data.finished) {
                SudokuManager.startNewGame()
                return
            }
            SudokuManager.difficulty = data.difficulty
            val size = sqrt(data.input.size, RoundingMode.FLOOR)
            if (size == 0) return
            val cellList = mutableListOf<SudokuPuzzleCell>()
            var clueId = 0
            for (i in data.input.indices) {
                val isClue = if (clueId >= data.clues.size) false else data.clues[clueId] == i
                if (isClue) clueId++
                val notes = Array(size) { false }
                for (i2 in data.inputNotes[i]) {
                    notes[i2] = true
                }
                cellList.add(SudokuPuzzleCell(i, isClue, data.input[i], size, notes))
            }
            val clues = cellList.map { c -> if (c.isClue) c.value else 0 }.toIntArray()
            try {
                val p = LoadedPuzzle(size, data.difficulty, clues)
                SudokuManager.loadedPuzzle = p
                SudokuManager.cells = cellList.toTypedArray()
                loadedPuzzles.add(0, p)
                if (SudokuManager.checkConflictingCells) {
                    SudokuManager.cells.forEach { c -> SudokuManager.checkConflictingCell(c.id) }
                }
                puzzleLoaded.value = true
            } catch (e: IllegalArgumentException) {
                Logger.e("Saved Sudoku puzzle is not valid", e)
                return
            }
        }


        suspend fun startLoading() {
            var asyncProcesses = 0
            coroutineScope {
                while (true) {
                    if (loadedPuzzles.size >= 3) {
                        delay(5000)
                        continue
                    }
                    if (asyncProcesses >= 3) continue
                    asyncProcesses += 1
                    launch(coroutineContext) {
                        try {
                            val size = 9
                            val clues = SudokuPuzzle.createClues(
                                input = createPuzzle(size = size), maxClues = 17
                            )
                            val p = LoadedPuzzle(size, Difficulty.MEDIUM, clues)
                            loadedPuzzles.add(p)
                            requestingCallback?.let { it(p) }
                            requestingCallback = null
                            asyncProcesses--
                        } catch (_: Exception) {
                            asyncProcesses--
                        }
                    }
                }
            }
        }

        private fun createPuzzle(
            type: PuzzleType = PuzzleType.Classic, size: Int = 9
        ): SudokuPuzzle {
            return if (type == PuzzleType.Classic) SudokuPuzzle.randomGrid(size)
            else SudokuPuzzle.randomGrid(0)
        }
    }
}
