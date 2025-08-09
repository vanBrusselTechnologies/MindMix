package com.vanbrusselgames.mindmix.games.minesweeper.data.preferences

import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.core.edit

suspend fun DataStore<Preferences>.saveMinesweeperPreferences(prefs: MinesweeperPreferences) {
    edit { preferences ->
        preferences[MinesweeperPreferences.PREF_KEY_AUTO_FLAG] = prefs.autoFlag
        preferences[MinesweeperPreferences.PREF_KEY_DIFFICULTY] = prefs.difficulty
        preferences[MinesweeperPreferences.PREF_KEY_SAFE_START] = prefs.safeStart
    }
}