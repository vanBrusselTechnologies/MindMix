package com.vanbrusselgames.mindmix.games.game2048.data.preferences

import android.content.Context
import com.vanbrusselgames.mindmix.core.data.dataStore
import dagger.hilt.android.qualifiers.ApplicationContext
import kotlinx.coroutines.flow.first
import javax.inject.Inject

class Game2048PreferencesRepository @Inject constructor(
    @ApplicationContext private val context: Context
) {
    private val dataStore = context.dataStore

    suspend fun getPreferences(): Game2048Preferences {
        return dataStore.data.first().toGame2048Preferences()
    }

    suspend fun savePreferences(prefs: Game2048Preferences) {
        dataStore.saveGame2048Preferences(prefs)
    }
}