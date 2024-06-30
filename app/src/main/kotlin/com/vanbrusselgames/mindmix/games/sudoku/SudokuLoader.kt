package com.vanbrusselgames.mindmix.games.sudoku

import androidx.compose.runtime.mutableStateOf
import com.google.android.gms.tasks.Task
import com.google.android.gms.tasks.Tasks
import com.google.common.math.IntMath.sqrt
import com.google.firebase.functions.FirebaseFunctionsException
import com.vanbrusselgames.mindmix.Logger
import com.vanbrusselgames.mindmix.MainActivity
import com.vanbrusselgames.mindmix.games.GameLoader
import com.vanbrusselgames.mindmix.utils.constants.Difficulty
import com.vanbrusselgames.mindmix.utils.encode.Decode
import com.vanbrusselgames.mindmix.utils.encode.Encode
import kotlinx.coroutines.tasks.await
import java.math.RoundingMode

class SudokuLoader {
    companion object {
        private var requesting = false

        val puzzleLoaded = mutableStateOf(false)
        private val loadedPuzzles = mapOf(
            Difficulty.EASY to mutableListOf<LoadedPuzzle>(),
            Difficulty.MEDIUM to mutableListOf(),
            Difficulty.HARD to mutableListOf(),
            Difficulty.EXPERT to mutableListOf(),
            Difficulty.MASTER to mutableListOf()
        )

        val pages = mutableMapOf(
            Difficulty.EASY to 1,
            Difficulty.MEDIUM to 1,
            Difficulty.HARD to 1,
            Difficulty.EXPERT to 1,
            Difficulty.MASTER to 1
        )

        private var requestingCallback: ((LoadedPuzzle) -> Unit)? = null
        private var requestingDifficulty: Difficulty? = null

        suspend fun requestPuzzles(gameMode: PuzzleType, loadFromFile: Boolean = false) {
            try {
                Logger.i("Start loading Sudoku Puzzles")
                if (loadFromFile) {
                    for (diff in Difficulty.entries) {
                        loadFromFile(gameMode, diff)
                    }
                }
                for (diff in Difficulty.entries) {
                    if (loadedPuzzles[diff]!!.size > 5) continue
                    requestPuzzles(gameMode, diff, pages[diff]!!).await()
                }
            } catch (_: Exception) {
            }
        }

        private fun loadFromFile(gameMode: PuzzleType, difficulty: Difficulty) {
            val fileName = GameLoader.getFileName(GAME_ID, gameMode.ordinal, difficulty.name)
            val content = GameLoader.readFile(fileName)
            if (content.isEmpty()) return
            val size = SudokuManager.SIZE * SudokuManager.SIZE
            val puzzles = content.map { p ->
                val clues = Decode.base94toIntList(p.trim(), size)
                LoadedPuzzle(sqrt(size, RoundingMode.FLOOR), difficulty, clues)
            }.shuffled()

            Logger.i("Succesfully loaded ${puzzles.size} puzzles from file")
            loadedPuzzles[difficulty]!!.addAll(puzzles)

            var overrideProgress = false
            val p = puzzles.first()
            if (requestingCallback != null && requestingDifficulty == difficulty) {
                overrideProgress = true
                requestingCallback?.let { it(p) }
                requestingCallback = null
            }


            setProgress(
                SudokuProgress(
                    p.clues, p.clues, List(p.clues.size) { listOf() }, difficulty
                ), overrideProgress
            )
        }

        private fun requestPuzzles(
            gameMode: PuzzleType, difficulty: Difficulty, page: Int, perPage: Int = 100
        ): Task<Unit> {
            if (requesting) return Tasks.forResult(Unit)
            requesting = true

            Logger.i("Requesting Sudoku puzzles")
            val size = SudokuManager.SIZE * SudokuManager.SIZE
            val data = hashMapOf(
                "gameId" to GAME_ID,
                "gameModeId" to gameMode.ordinal,
                "difficulty" to difficulty.name.lowercase(),
                "page" to page,
                "per_page" to perPage
            )
            return MainActivity.functions.getHttpsCallable("puzzles").call(data)
                .continueWith { task ->
                    val encodedPuzzles = (task.result?.data as List<*>).filterIsInstance<String>()
                    val puzzles = encodedPuzzles.map { p ->
                        val clues = Decode.base94toIntList(p.trim(), size)
                        LoadedPuzzle(sqrt(size, RoundingMode.FLOOR), difficulty, clues)
                    }.shuffled()

                    Logger.i("Succesfully loaded ${puzzles.size} puzzles")
                    loadedPuzzles[difficulty]!!.addAll(puzzles)

                    var overrideProgress = false
                    val p = puzzles.first()
                    if (requestingCallback != null && requestingDifficulty == difficulty) {
                        overrideProgress = true
                        requestingCallback?.let { it(p) }
                        requestingCallback = null
                    }


                    setProgress(
                        SudokuProgress(
                            p.clues, p.clues, List(p.clues.size) { listOf() }, difficulty
                        ), overrideProgress
                    )

                    val fileName =
                        GameLoader.getFileName(GAME_ID, gameMode.ordinal, difficulty.name)
                    GameLoader.appendToFile(fileName, encodedPuzzles)

                    requesting = false
                }.addOnCompleteListener { task ->
                    requesting = false
                    if (!task.isSuccessful) {
                        val e = task.exception
                        if (e is FirebaseFunctionsException) {
                            val code = e.code
                            val details = e.details
                            Logger.w("$code, $details")
                        }
                        Logger.w("Failed to load puzzles", e!!)
                    }
                }
        }

        fun requestPuzzle(diff: Difficulty, callback: (LoadedPuzzle) -> Unit) {
            try {
                val p = loadedPuzzles[diff]!!.first()
                setProgress(
                    SudokuProgress(p.clues, p.clues, List(p.clues.size) { listOf() }, diff), true
                )
                callback(p)
            } catch (e: NoSuchElementException) {
                requestingDifficulty = diff
                requestingCallback = callback
            }
        }

        fun removePuzzle(p: LoadedPuzzle?) {
            if (p == null) return
            val index = loadedPuzzles[p.difficulty]!!.indexOfFirst {
                it.clues.joinToString("") == p.clues.joinToString("")
            }
            if (index != -1) {
                loadedPuzzles[p.difficulty]!!.removeAt(index)
                if (loadedPuzzles[p.difficulty]!!.size <= 50) {
                    val page = pages[p.difficulty]!!
                    requestPuzzles(PuzzleType.Classic, p.difficulty, page)
                    pages[p.difficulty] = page + 1
                }
                puzzleLoaded.value = false

                val puzzle = Encode.base94(p.clues)

                val fileName = GameLoader.getFileName(
                    GAME_ID, SudokuManager.gameMode.ordinal, p.difficulty.name
                )
                GameLoader.removeFromFile(fileName, puzzle)
            }
        }

        fun loadFromFile(data: SudokuData) {
            SudokuManager.setDifficulty(data.difficulty)
            for (kvp in data.page) pages[kvp.key] = kvp.value
            val progressList = data.progress
            val size = SudokuManager.SIZE * SudokuManager.SIZE
            val savedProgress = progressList.map { progress ->
                SudokuProgress(
                    Decode.base94toIntList(progress.clues, size),
                    Decode.base94toIntList(progress.input, size),
                    progress.inputNotes.map { notes ->
                        val decodedNotes = Decode.base94toBooleanList(notes, SudokuManager.SIZE)
                        List(SudokuManager.SIZE) { i -> if (decodedNotes[i]) i + 1 else 0 }
                    },
                    progress.difficulty
                )
            }
            for (progress in savedProgress) {
                setProgress(progress, true)
            }
        }

        private fun setProgress(progress: SudokuProgress<List<Int>>, override: Boolean) {
            if(progress.clues.all { it == 0 }) return
            val index =
                SudokuManager.savedProgress.indexOfFirst { it.difficulty == progress.difficulty }
            if (index != -1) {
                if (!override && SudokuManager.savedProgress[index].clues.isNotEmpty()) return
                SudokuManager.savedProgress[index] = progress
                if (progress.clues.isEmpty()) return
                val size = sqrt(progress.input.size, RoundingMode.FLOOR)
                val p = LoadedPuzzle(size, progress.difficulty, progress.clues)

                val duplicateIndex = loadedPuzzles[p.difficulty]!!.indexOfFirst {
                    it.clues.joinToString("") == p.clues.joinToString("")
                }
                if (duplicateIndex != -1) loadedPuzzles[p.difficulty]?.removeAt(duplicateIndex)

                loadedPuzzles[progress.difficulty]!!.add(0, p)
                if (SudokuManager.difficulty.value == progress.difficulty) {
                    SudokuManager.currentPuzzle = p

                    for (i in progress.input.indices) {
                        val cell = SudokuManager.cells[i]
                        cell.value.intValue = progress.input[i]
                        for (n in 0 until 9) {
                            val note = n + 1
                            if (progress.inputNotes[i].contains(note) != cell.hasNote(note)) {
                                cell.setNote(note)
                            }
                        }
                    }

                    if (SudokuManager.checkConflictingCells.value) {
                        SudokuManager.cells.forEach { c ->
                            SudokuManager.checkConflictingCell(c)
                        }
                    } else SudokuManager.cells.forEach { it.isIncorrect.value = false }

                    puzzleLoaded.value = true
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
