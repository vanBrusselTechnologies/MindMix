package com.vanbrusselgames.mindmix.games.sudoku

import androidx.compose.runtime.mutableStateOf
import com.google.firebase.analytics.FirebaseAnalytics
import com.vanbrusselgames.mindmix.BaseLayout
import com.vanbrusselgames.mindmix.Logger
import com.vanbrusselgames.mindmix.R
import com.vanbrusselgames.mindmix.games.GameFinished
import com.vanbrusselgames.mindmix.utils.constants.Difficulty
import com.vanbrusselgames.mindmix.utils.encode.Encode
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json

class SudokuManager {
    companion object Instance {
        const val SIZE = 9

        var gameMode = PuzzleType.Classic

        var currentPuzzle: LoadedPuzzle? = null
            set(p) {
                field = p
                if (p != null) onPuzzleLoaded(p)
            }

        val cells = Array(SIZE * SIZE) { SudokuPuzzleCell(it, false, 0, SIZE) }

        var checkConflictingCells = mutableStateOf(false)
        var autoEditNotes = mutableStateOf(false)

        var finished = false
        val difficulty = mutableStateOf(Difficulty.MEDIUM)

        fun setDifficulty(value: Difficulty) {
            saveAndLoadProgress(difficulty.value, value)
            difficulty.value = value
        }

        var savedProgress = Array(Difficulty.entries.size) {
            SudokuProgress<List<Int>>(listOf(), mutableListOf(), listOf(), Difficulty.entries[it])
        }

        private fun saveAndLoadProgress(prevDifficulty: Difficulty, difficulty: Difficulty) {
            if (currentPuzzle != null) setCurrentProgress(prevDifficulty)

            val progress = savedProgress.find { it.difficulty == difficulty }!!
            if (progress.clues.isEmpty()) return
            currentPuzzle = LoadedPuzzle(SIZE, difficulty, progress.clues)
            cells.forEachIndexed { i, cell ->
                with(cell) {
                    value.intValue = progress.input[i]
                    for (j in 0 until SIZE) {
                        val note = j + 1
                        if (progress.inputNotes[i].contains(note) != cell.hasNote(note)) {
                            cell.setNote(note)
                        }
                    }
                    isClue.value = progress.clues[i] != 0
                }
            }
            if (checkConflictingCells.value) {
                cells.forEach { checkConflictingCell(it) }
            } else cells.forEach { it.isIncorrect.value = false }
        }

        private fun onPuzzleLoaded(p: LoadedPuzzle) {
            if (cells.size == p.size * p.size) {
                cells.forEach {
                    it.reset()
                    it.isClue.value = p.clues[it.id] != 0
                    it.value.intValue = p.clues[it.id]
                }
            } else {
                cells.forEach { it.reset() }
            }
            if (checkConflictingCells.value) cells.forEach { c -> checkConflictingCell(c) }
            else cells.forEach { it.isIncorrect.value = false }

            SudokuLoader.puzzleLoaded.value = true
        }

        private fun setCurrentProgress(difficulty: Difficulty = this.difficulty.value) {
            val p = savedProgress.first { it.difficulty == difficulty }
            p.input = cells.map { c -> c.value.intValue }
            p.inputNotes = cells.map { c ->
                c.notes.mapIndexed { i, n -> if (n) i + 1 else 0 }.toMutableList()
            }
        }

        fun saveToFile(): String {
            if (finished) {
                val index = savedProgress.indexOfFirst { it.difficulty == difficulty.value }
                savedProgress[index] =
                    SudokuProgress(listOf(), listOf(), listOf(), difficulty.value)
            } else {
                val p = savedProgress.first { it.difficulty == difficulty.value }
                p.input = cells.map { c -> c.value.intValue }
                p.inputNotes = cells.map { c ->
                    c.notes.mapIndexed { i, n -> if (n) i + 1 else 0 }.toMutableList()
                }
            }

            val progress = savedProgress.map {
                SudokuProgress(
                    Encode.base94Collection(it.clues),
                    Encode.base94Collection(it.input),
                    it.inputNotes.map { notes ->
                        val booleans = notes.map { n -> n != 0 }
                        Encode.base94Collection(booleans)
                    },
                    it.difficulty
                )
            }
            return Json.encodeToString(
                SudokuData(difficulty.value, progress, SudokuLoader.pages.toMap())
            )
        }

        fun startPuzzle() {
            if (finished) reset()
            if (currentPuzzle != null) return
            SudokuLoader.requestPuzzle(difficulty.value) { p ->
                currentPuzzle = p
            }
        }

        private fun reset() {
            finished = false
            BaseLayout.activeOverlapUI.value = false
            SudokuLoader.removePuzzle(currentPuzzle)
            cells.forEach {
                it.reset()
            }
            currentPuzzle = null
        }

        fun startNewGame() {
            reset()
            startPuzzle()
            Logger.logEvent(FirebaseAnalytics.Event.LEVEL_START) {
                param(FirebaseAnalytics.Param.LEVEL_NAME, GAME_NAME)
            }
        }

        fun autoChangeNotes(cell: SudokuPuzzleCell) {
            if (currentPuzzle == null) return
            val index = cell.id
            val number: Int = cell.value.intValue
            if (number == 0) return
            val indices: IntArray = SudokuPuzzle.peers(index, cells.size)
            for (n: Int in indices) {
                if (index == n) continue
                if (!cells[n].hasNote(number)) continue
                cells[n].setNote(number)
            }
        }

        fun checkConflictingCell(cell: SudokuPuzzleCell, isSecondary: Boolean = false) {
            if (currentPuzzle == null) return
            if (cell.isClue.value) return
            val index = cell.id
            val indices: IntArray = SudokuPuzzle.peers(index, cells.size)
            var isConflicting = false
            val v = cell.value.intValue
            for (n: Int in indices) {
                if (index == n) continue
                val cellN = cells[n]
                when (true) {
                    (v == 0) -> {
                        if (!isSecondary && cellN.isIncorrect.value) {
                            checkConflictingCell(cellN, true)
                        }
                        if (cellN.value.intValue == 0) continue
                        if (!cell.hasNote(cellN.value.intValue)) continue
                        cell.isIncorrect.value = true
                        isConflicting = true
                    }

                    (v == cellN.value.intValue) -> {
                        cell.isIncorrect.value = true
                        if (!isSecondary) checkConflictingCell(cellN, true)
                        isConflicting = true
                    }

                    (!isSecondary && cellN.isIncorrect.value || cellN.value.intValue == 0 && cellN.hasNote(
                        v
                    )) -> {
                        checkConflictingCell(cellN, true)
                    }

                    else -> {}
                }
            }
            if (!isConflicting) cell.isIncorrect.value = false
        }

        fun checkFinished() {
            finished = isFinished()
            if (finished) {
                Logger.logEvent(FirebaseAnalytics.Event.LEVEL_END) {
                    param(FirebaseAnalytics.Param.LEVEL_NAME, GAME_NAME)
                    param(FirebaseAnalytics.Param.SUCCESS, 1)
                }
                onGameFinished()
            }
        }

        private fun isFinished(): Boolean {
            try {
                if (cells.any { c -> c.value.intValue == 0 }) return false
                val input = cells.map { c -> c.value.intValue }.toIntArray()
                return SudokuPuzzle.getSolution(input).contentEquals(input)
            } catch (_: Exception) {
                return false
            }
        }

        private fun onGameFinished() {
            val titleId = R.string.sudoku_name//"Congrats / Smart / Well done"
            val descId = R.string.sudoku_success
            //"""You did great and solved puzzle in ${0} seconds!!
            //     |That's Awesome!
            //     |Share with your friends and challenge them to beat your time!""".trimMargin()
            val reward = 10
            GameFinished.onGameFinished(titleId, descId, reward)
        }
    }
}