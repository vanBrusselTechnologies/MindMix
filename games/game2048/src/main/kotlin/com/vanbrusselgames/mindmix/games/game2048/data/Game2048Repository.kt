package com.vanbrusselgames.mindmix.games.game2048.data

import com.vanbrusselgames.mindmix.core.data.DataManager
import com.vanbrusselgames.mindmix.core.model.SceneRegistry
import com.vanbrusselgames.mindmix.games.game2048.model.Game2048Data
import com.vanbrusselgames.mindmix.games.game2048.model.Game2048Progress
import com.vanbrusselgames.mindmix.games.game2048.model.Game2048Record
import com.vanbrusselgames.mindmix.games.game2048.model.GridSize2048
import kotlinx.serialization.json.Json
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class Game2048Repository @Inject constructor(private val dataManager: DataManager) {
    private val _puzzleProgress = mutableListOf<Game2048Progress>()
    private val _puzzleRecords = mutableListOf<Game2048Record>()
    private val jsonParser = Json { ignoreUnknownKeys = true }

    fun createNewPuzzle(size: GridSize2048): Game2048Progress {
        val sideSize = size.getSize()
        val cellCount = sideSize * sideSize
        val list = MutableList(cellCount) { 0L }
        val randomIndices = (0 until cellCount).shuffled().take(3)
        for (index in randomIndices) {
            list[index] = 2L
        }
        val progress = Game2048Progress(size, list, 0, 0)
        val index = _puzzleProgress.indexOfFirst { it.size == size }
        if (index == -1) _puzzleProgress.add(progress) else _puzzleProgress[index] = progress
        return progress
    }

    fun getPuzzleProgress(size: GridSize2048): Game2048Progress? {
        if (_puzzleProgress.isNotEmpty()) return _puzzleProgress.firstOrNull { it.size == size }

        val json = dataManager.getSavedDataForScene(SceneRegistry.Game2048)
        if (json.trim() == "") return null
        val data = jsonParser.decodeFromString<Game2048Data>(json)

        _puzzleProgress.clear()
        _puzzleProgress.addAll(data.progress)
        _puzzleRecords.clear()
        _puzzleRecords.addAll(data.records)
        return data.progress.firstOrNull { it.size == size }
    }

    fun setPuzzleProgressForSize(
        size: GridSize2048, cellValues: List<Long>, moves: Int, score: Long
    ) {
        val index = _puzzleProgress.indexOfFirst { it.size == size }
        val progress = Game2048Progress(
            size,
            cellValues,
            moves,
            score,
        )
        if (index == -1) _puzzleProgress.add(progress) else {
            val input1 = _puzzleProgress[index].cellValues.joinToString("")
            val input2 = progress.cellValues.joinToString("")
            if (input1 == input2) return
            _puzzleProgress[index] = progress
        }

        saveProgress()
    }

    private fun saveProgress() {
        val data = Json.encodeToString(Game2048Data(_puzzleProgress, _puzzleRecords))
        dataManager.saveScene(SceneRegistry.Game2048, data)
    }

    fun removeProgressForSize(size: GridSize2048) {
        _puzzleProgress.removeAll { it.size == size }
        val data = Json.encodeToString(Game2048Data(_puzzleProgress, _puzzleRecords))
        dataManager.saveScene(SceneRegistry.Game2048, data)
    }
}
