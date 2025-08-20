package com.vanbrusselgames.mindmix.core.data.preferences

import androidx.datastore.preferences.core.intPreferencesKey

data class UserPreferences(val theme: Int) {
    companion object {
        val PREF_KEY_THEME = intPreferencesKey("menu_theme")
    }
}