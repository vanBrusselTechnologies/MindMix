package com.vanbrusselgames.mindmix.feature.menu.data.preferences

import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.core.edit

suspend fun DataStore<Preferences>.saveMenuPreferences(prefs: MenuPreferences) {
    edit { preferences ->
        preferences[MenuPreferences.PREF_KEY_SELECTED_GAME] = prefs.selectedGame
    }
}