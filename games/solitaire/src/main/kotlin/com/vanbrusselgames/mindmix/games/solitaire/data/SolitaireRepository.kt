package com.vanbrusselgames.mindmix.games.solitaire.data

import com.vanbrusselgames.mindmix.core.common.GameTimer
import com.vanbrusselgames.mindmix.core.data.DataManager
import com.vanbrusselgames.mindmix.core.model.SceneRegistry
import com.vanbrusselgames.mindmix.games.solitaire.model.PlayingCard
import com.vanbrusselgames.mindmix.games.solitaire.model.SolitaireData
import com.vanbrusselgames.mindmix.games.solitaire.model.SolitaireProgress
import com.vanbrusselgames.mindmix.games.solitaire.model.SolitaireRecord
import kotlinx.serialization.json.Json
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class SolitaireRepository @Inject constructor(private val dataManager: DataManager) {
    private val _puzzleProgress = mutableListOf<SolitaireProgress>()
    private val _puzzleRecords = mutableListOf<SolitaireRecord>()
    private val jsonParser = Json { ignoreUnknownKeys = true }

    fun createNewPuzzle(): SolitaireProgress {
        val stacks = MutableList(14) { listOf<Int>() }
        val cardIndices = (0 until 52).shuffled()
        var j = 0
        for (i in 0 until 7) {
            stacks[7 + i] = cardIndices.subList(j, j + i + 1)
                .mapIndexed { index, id -> if (index == i) id else -(id + 1) }
            j += i + 1
        }
        stacks[6] = cardIndices.subList(j, cardIndices.size).map { -(it + 1) }

        val progress = SolitaireProgress(stacks, 0L, 0L, 0)
        val index = _puzzleProgress.indexOfFirst { true }
        if (index == -1) _puzzleProgress.add(progress) else _puzzleProgress[index] = progress
        return progress
    }

    fun getPuzzleProgress(): SolitaireProgress? {
        if (_puzzleProgress.isNotEmpty()) return _puzzleProgress.firstOrNull { true }

        val json = dataManager.getSavedDataForScene(SceneRegistry.Solitaire)
        if (json.trim() == "") return null
        val data = jsonParser.decodeFromString<SolitaireData>(json)

        _puzzleProgress.clear()
        _puzzleProgress.addAll(data.progress)
        _puzzleRecords.clear()
        _puzzleRecords.addAll(data.records)
        return data.progress.firstOrNull { true }
    }

    fun setPuzzleProgress(
        cardStacks: Array<MutableList<PlayingCard>>, moves: Int, timer: GameTimer
    ) {
        val stacks = MutableList(14) { listOf<Int>() }
        for (i in cardStacks.indices) {
            stacks[i] = cardStacks[i].map { if (it.frontVisible.value) it.id else -1 * it.id - 1 }
        }
        val progress = SolitaireProgress(stacks, timer.currentMillis, timer.addedMillis, moves)
        val index = _puzzleProgress.indexOfFirst { true }
        if (index == -1) _puzzleProgress.add(progress) else _puzzleProgress[index] = progress
        saveProgress()
    }

    fun getPuzzleRecord(): SolitaireRecord? {
        if (_puzzleRecords.isNotEmpty()) return _puzzleRecords.firstOrNull { true }

        val json = dataManager.getSavedDataForScene(SceneRegistry.Solitaire)
        if (json.trim() == "") return null
        val data = jsonParser.decodeFromString<SolitaireData>(json)

        _puzzleProgress.clear()
        _puzzleProgress.addAll(data.progress)
        _puzzleRecords.clear()
        _puzzleRecords.addAll(data.records)
        return data.records.firstOrNull { true }
    }

    fun setPuzzleRecord(fastestMillis: Long) {
        val record = SolitaireRecord(fastestMillis)
        val index = _puzzleRecords.indexOfFirst { true }
        if (index == -1) _puzzleRecords.add(record) else _puzzleRecords[index] = record
        saveProgress()
    }

    private fun saveProgress() {
        val data = Json.encodeToString(SolitaireData(_puzzleProgress, _puzzleRecords))
        dataManager.saveScene(SceneRegistry.Solitaire, data)
    }

    fun removeProgress() {
        _puzzleProgress.removeAll { true }
        saveProgress()
    }

    fun forceSave() {
        dataManager.save(true)
    }
}
