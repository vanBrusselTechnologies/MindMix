package com.vanbrusselgames.mindmix.games.sudoku.data

import com.google.firebase.Firebase
import com.google.firebase.functions.FirebaseFunctionsException
import com.google.firebase.functions.functions
import com.vanbrusselgames.mindmix.core.data.DataManager
import com.vanbrusselgames.mindmix.core.games.GameLoader
import com.vanbrusselgames.mindmix.core.logging.Logger
import com.vanbrusselgames.mindmix.core.model.SceneRegistry
import com.vanbrusselgames.mindmix.core.utils.constants.Difficulty
import com.vanbrusselgames.mindmix.core.utils.encode.Decode
import com.vanbrusselgames.mindmix.core.utils.encode.Encode
import com.vanbrusselgames.mindmix.games.sudoku.model.LoadedPuzzle
import com.vanbrusselgames.mindmix.games.sudoku.model.PuzzleType
import com.vanbrusselgames.mindmix.games.sudoku.model.SudokuData
import com.vanbrusselgames.mindmix.games.sudoku.model.SudokuProgress
import com.vanbrusselgames.mindmix.games.sudoku.model.SudokuPuzzleCell
import com.vanbrusselgames.mindmix.games.sudoku.model.SudokuSavedProgress
import com.vanbrusselgames.mindmix.games.sudoku.viewmodel.SudokuViewModel
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.channels.awaitClose
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.callbackFlow
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import kotlinx.serialization.json.Json
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class SudokuRepository @Inject constructor(
    private val gameLoader: GameLoader, private val dataManager: DataManager
) {
    val size = SudokuViewModel.SIZE
    val newPuzzles = Difficulty.entries.associateWith { mutableListOf<String>() }
    val pages = Difficulty.entries.associateWith { 1 }.toMutableMap()
    private val _puzzleProgress = mutableListOf<SudokuProgress>()
    private val _stringProgress = mutableListOf<SudokuSavedProgress>()
    private var _loadedFromFiles = false
    private val jsonParser = Json { ignoreUnknownKeys = true }

    private fun getFileName(gameMode: PuzzleType, difficulty: Difficulty): String {
        return gameLoader.getFileName(
            SceneRegistry.Sudoku.gameId, gameMode.ordinal, difficulty.name
        )
    }

    fun requestNewPuzzleFlow(difficulty: Difficulty, gameMode: PuzzleType): Flow<SudokuProgress> =
        callbackFlow {
            if (!_loadedFromFiles) {
                _loadedFromFiles = true
                Difficulty.entries.forEach {
                    loadNewPuzzlesFromFile(it, gameMode)
                }
            }
            requestPuzzle(difficulty, gameMode) {
                trySend(it)
                close()
            }
            awaitClose { /* no-op cleanup */ }
        }

    private fun getAmountCachedPuzzles(difficulty: Difficulty): Int {
        return newPuzzles[difficulty]?.size ?: 0
    }

    private fun getCachedPuzzle(difficulty: Difficulty, gameMode: PuzzleType): SudokuProgress? {
        val newPuzzleEncodedClues = newPuzzles[difficulty]?.removeFirstOrNull()
        if (newPuzzleEncodedClues == null) return null
        gameLoader.removeFromFile(getFileName(gameMode, difficulty), newPuzzleEncodedClues)

        val clues = Decode.base94toIntList(newPuzzleEncodedClues.trim(), size * size)
        val puzzle = LoadedPuzzle(difficulty, clues)
        val progress = puzzle.toSudokuProgress()
        val index = _puzzleProgress.indexOfFirst { it.difficulty == difficulty }
        if (index == -1) _puzzleProgress.add(progress) else _puzzleProgress[index] = progress
        val stringProgress = stringifyEmptyProgress(progress, newPuzzleEncodedClues)
        updateStringProgressAndSave(stringProgress)
        return progress
    }

    private fun downloadNewPuzzles(
        difficulty: Difficulty, gameMode: PuzzleType, onCacheAvailable: () -> Unit
    ) {
        val cachedAmount = getAmountCachedPuzzles(difficulty)
        if (cachedAmount >= 5) return onCacheAvailable()
        if (cachedAmount >= 1) onCacheAvailable()
        val page = pages[difficulty]!!
        fetchFromFirebase(difficulty, gameMode, page, 100, { encodedClueList ->
            pages[difficulty] = page + 1
            val encodedClues = encodedClueList.shuffled().toMutableList()
            newPuzzles[difficulty]?.addAll(encodedClues)
            gameLoader.appendToFile(getFileName(gameMode, difficulty), encodedClues)
            if (cachedAmount == 0) onCacheAvailable()
        })
    }

    private fun requestPuzzle(
        difficulty: Difficulty, gameMode: PuzzleType, callback: (SudokuProgress) -> Unit
    ) {
        downloadNewPuzzles(difficulty, gameMode) {
            Logger.d("[sudoku] Cached Puzzle available")
            callback(getCachedPuzzle(difficulty, gameMode)!!)
        }
    }

    private fun loadNewPuzzlesFromFile(difficulty: Difficulty, gameMode: PuzzleType) {
        val content = gameLoader.readFile(getFileName(gameMode, difficulty))
        newPuzzles[difficulty]?.addAll(content)
    }

    private fun fetchFromFirebase(
        difficulty: Difficulty,
        gameMode: PuzzleType,
        page: Int,
        perPage: Int = 100,
        onSuccess: (List<String>) -> Unit,
        onError: (Exception) -> Unit? = {}
    ) {
        val data = mapOf(
            "gameId" to SceneRegistry.Sudoku.gameId,
            "gameModeId" to gameMode.ordinal,
            "difficulty" to difficulty.name.lowercase(),
            "page" to page,
            "per_page" to perPage
        )

        Firebase.functions.getHttpsCallable("puzzles").call(data).continueWith { task ->
            val result = task.result?.data as? List<*> ?: return@continueWith
            Logger.d("[sudoku] Downloaded ${result.size} puzzles")
            onSuccess(result.filterIsInstance<String>())
        }.addOnFailureListener { e ->
            if (e is FirebaseFunctionsException) {
                Logger.e("[sudoku] Firebase error: ${e.code}, ${e.details}")
            }
            Logger.e("[sudoku] Failed to fetch puzzles", e)
            onError(e)
        }
    }

    suspend fun getPuzzleProgress(difficulty: Difficulty): SudokuProgress? {
        if (_puzzleProgress.isNotEmpty()) return _puzzleProgress.firstOrNull { it.difficulty == difficulty }

        val json = dataManager.getSavedDataForScene(SceneRegistry.Sudoku)
        if (json.trim() == "") return null
        val data = jsonParser.decodeFromString<SudokuData>(json)
        for (kvp in data.page) pages[kvp.key] = kvp.value
        if (data.progress.isEmpty()) return null

        val savedProgress = withContext(Dispatchers.Default) {
            data.progress.filter { it.clues != "" }.map { progress ->
                SudokuProgress(
                    Decode.base94toIntList(progress.clues, size * size),
                    Decode.base94toIntList(progress.input, size * size),
                    progress.inputNotes.map { notes ->
                        val decodedNotes = Decode.base94toBooleanList(notes, SudokuViewModel.SIZE)
                        List(SudokuViewModel.SIZE) { if (decodedNotes[it]) it + 1 else 0 }
                    },
                    progress.difficulty
                )
            }
        }
        _puzzleProgress.clear()
        _puzzleProgress.addAll(savedProgress)
        _stringProgress.clear()
        _stringProgress.addAll(data.progress)
        return savedProgress.firstOrNull { it.difficulty == difficulty }
    }

    fun setPuzzleProgressForDifficulty(difficulty: Difficulty, cells: Array<SudokuPuzzleCell>) {
        val index = _puzzleProgress.indexOfFirst { it.difficulty == difficulty }
        val clues = cells.map { if (it.isClue.value) it.value.intValue else 0 }
        val input = cells.map { it.value.intValue }
        val inputNotes = cells.map { c ->
            c.notes.mapIndexed { i, n -> if (n) i + 1 else 0 }.toMutableList()
        }
        val progress = SudokuProgress(clues, input, inputNotes, difficulty)
        if (index == -1) _puzzleProgress.add(progress) else {
            val input1 = _puzzleProgress[index].input.joinToString("")
            val input2 = progress.input.joinToString("")
            val notes1 =
                _puzzleProgress[index].inputNotes.joinToString("") { it.joinToString { "" } }
            val notes2 = progress.inputNotes.joinToString("") { it.joinToString { "" } }
            if (input1 == input2 && notes1 == notes2) return
            _puzzleProgress[index] = progress
        }
        CoroutineScope(Dispatchers.IO).launch {
            val stringProgress = stringifyProgress(progress)
            updateStringProgressAndSave(stringProgress)
        }
    }

    private fun updateStringProgressAndSave(progress: SudokuSavedProgress) {
        CoroutineScope(Dispatchers.IO).launch {
            val index = _stringProgress.indexOfFirst { it.difficulty == progress.difficulty }
            if (index == -1) _stringProgress.add(progress) else _stringProgress[index] = progress
            val data = Json.encodeToString(SudokuData(_stringProgress, pages.toMap()))
            dataManager.saveScene(SceneRegistry.Sudoku, data)
        }
    }

    private fun stringifyProgress(progress: SudokuProgress): SudokuSavedProgress {
        return SudokuSavedProgress(
            Encode.base94IntCollection(progress.clues),
            Encode.base94IntCollection(progress.input),
            progress.inputNotes.map { notes ->
                val booleans = notes.map { it != 0 }
                Encode.base94BooleanCollection(booleans)
            },
            progress.difficulty
        )
    }

    private fun stringifyEmptyProgress(
        progress: SudokuProgress, encodedClues: String
    ): SudokuSavedProgress {
        val encodedNotes = Encode.base94BooleanCollection(progress.inputNotes.first().map { false })
        return SudokuSavedProgress(
            encodedClues,
            encodedClues,
            progress.inputNotes.map { encodedNotes },
            progress.difficulty
        )
    }

    fun removeProgressForDifficulty(difficulty: Difficulty) {
        _puzzleProgress.removeAll { it.difficulty == difficulty }
        _stringProgress.removeAll { it.difficulty == difficulty }
        val data = Json.encodeToString(SudokuData(_stringProgress, pages.toMap()))
        dataManager.saveScene(SceneRegistry.Sudoku, data)
    }
}
