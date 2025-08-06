package com.vanbrusselgames.mindmix.games.game2048.data.preferences

import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.core.edit

suspend fun DataStore<Preferences>.saveGame2048Preferences(prefs: Game2048Preferences) {
    edit { preferences ->
        preferences[Game2048Preferences.PREF_KEY_SIZE] = prefs.size
    }
}