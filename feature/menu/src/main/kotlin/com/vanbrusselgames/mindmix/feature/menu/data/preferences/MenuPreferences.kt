package com.vanbrusselgames.mindmix.feature.menu.data.preferences

import androidx.datastore.preferences.core.intPreferencesKey

data class MenuPreferences(val selectedGame: Int) {
    companion object {
        val PREF_KEY_SELECTED_GAME = intPreferencesKey("menu_selectedGame")
    }
}