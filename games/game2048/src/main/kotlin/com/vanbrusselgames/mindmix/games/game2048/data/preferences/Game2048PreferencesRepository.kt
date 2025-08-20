package com.vanbrusselgames.mindmix.games.game2048.data.preferences

import android.content.Context
import com.vanbrusselgames.mindmix.core.data.dataStore
import dagger.hilt.android.qualifiers.ApplicationContext
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.map
import javax.inject.Inject

class Game2048PreferencesRepository @Inject constructor(@ApplicationContext ctx: Context) {
    private val dataStore = ctx.dataStore

    fun getPreferences(): Flow<Game2048Preferences> {
        return dataStore.data.map { it.toGame2048Preferences() }
    }

    suspend fun savePreferences(prefs: Game2048Preferences) {
        dataStore.saveGame2048Preferences(prefs)
    }
}