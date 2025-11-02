package com.vanbrusselgames.mindmix.games.minesweeper.data

import com.vanbrusselgames.mindmix.core.data.DataManager
import com.vanbrusselgames.mindmix.core.model.SceneRegistry
import com.vanbrusselgames.mindmix.core.utils.constants.Difficulty
import com.vanbrusselgames.mindmix.games.minesweeper.model.CellState
import com.vanbrusselgames.mindmix.games.minesweeper.model.MinesweeperCell
import com.vanbrusselgames.mindmix.games.minesweeper.model.MinesweeperData
import com.vanbrusselgames.mindmix.games.minesweeper.model.MinesweeperProgress
import kotlinx.serialization.json.Json
import javax.inject.Inject
import javax.inject.Singleton
import kotlin.math.pow

@Singleton
class MinesweeperRepository @Inject constructor(private val dataManager: DataManager) {
    private val _puzzleProgress = mutableListOf<MinesweeperProgress>()
    private val jsonParser = Json { ignoreUnknownKeys = true }

    fun createNewPuzzle(difficulty: Difficulty, cellCount: Int): MinesweeperProgress {
        val mineCount = 25 + (10f * 1.75f.pow(difficulty.ordinal)).toInt()
        val mineIndices = (0 until cellCount).shuffled().take(mineCount)
        val mines = MutableList(cellCount) { CellState.Empty.ordinal }
        val progress = MinesweeperProgress(difficulty, mines, mineIndices)
        val index = _puzzleProgress.indexOfFirst { it.difficulty == difficulty }
        if (index == -1) _puzzleProgress.add(progress) else _puzzleProgress[index] = progress
        return progress
    }

    fun getPuzzleProgress(difficulty: Difficulty): MinesweeperProgress? {
        if (_puzzleProgress.isNotEmpty()) return _puzzleProgress.firstOrNull { it.difficulty == difficulty }

        val json = dataManager.getSavedDataForScene(SceneRegistry.Minesweeper)
        if (json.trim() == "") return null
        val data = jsonParser.decodeFromString<MinesweeperData>(json)

        _puzzleProgress.clear()
        _puzzleProgress.addAll(data.progress)
        return data.progress.firstOrNull { it.difficulty == difficulty }
    }

    fun setPuzzleProgressForDifficulty(difficulty: Difficulty, cells: Array<MinesweeperCell>) {
        val index = _puzzleProgress.indexOfFirst { it.difficulty == difficulty }
        val mines = cells.filter { it.isMine }.map { it.id }
        val input = cells.map { it.state.ordinal }
        val progress = MinesweeperProgress(difficulty, input, mines)
        if (index == -1) _puzzleProgress.add(progress) else {
            val input1 = _puzzleProgress[index].input.joinToString("")
            val input2 = progress.input.joinToString("")
            if (input1 == input2) return
            _puzzleProgress[index] = progress
        }

        saveProgress()
    }

    private fun saveProgress() {
        val data = Json.encodeToString(MinesweeperData(_puzzleProgress))
        dataManager.saveScene(SceneRegistry.Minesweeper, data)
    }

    fun removeProgressForDifficulty(difficulty: Difficulty) {
        _puzzleProgress.removeAll { it.difficulty == difficulty }
        saveProgress()
    }
}
