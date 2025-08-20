package com.vanbrusselgames.mindmix.preferences

import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.core.edit

suspend fun DataStore<Preferences>.saveUserPreferences(prefs: UserPreferences) {
    edit { preferences ->
        preferences[UserPreferences.PREF_KEY_THEME] = prefs.theme
    }
}