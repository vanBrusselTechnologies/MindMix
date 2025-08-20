package com.vanbrusselgames.mindmix.games.minesweeper.data.preferences

import android.content.Context
import com.vanbrusselgames.mindmix.core.data.dataStore
import dagger.hilt.android.qualifiers.ApplicationContext
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.map
import javax.inject.Inject

class MinesweeperPreferencesRepository @Inject constructor(@ApplicationContext ctx: Context) {
    private val dataStore = ctx.dataStore

    fun getPreferences(): Flow<MinesweeperPreferences> {
        return dataStore.data.map { it.toMinesweeperPreferences() }
    }

    suspend fun savePreferences(prefs: MinesweeperPreferences) {
        dataStore.saveMinesweeperPreferences(prefs)
    }
}