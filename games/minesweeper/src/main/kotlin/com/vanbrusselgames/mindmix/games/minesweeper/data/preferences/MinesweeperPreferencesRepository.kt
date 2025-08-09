package com.vanbrusselgames.mindmix.games.minesweeper.data.preferences

import android.content.Context
import com.vanbrusselgames.mindmix.core.data.dataStore
import dagger.hilt.android.qualifiers.ApplicationContext
import kotlinx.coroutines.flow.first
import javax.inject.Inject

class MinesweeperPreferencesRepository @Inject constructor(
    @ApplicationContext private val context: Context
) {
    private val dataStore = context.dataStore

    suspend fun getPreferences(): MinesweeperPreferences {
        return dataStore.data.first().toMinesweeperPreferences()
    }

    suspend fun savePreferences(prefs: MinesweeperPreferences) {
        dataStore.saveMinesweeperPreferences(prefs)
    }
}