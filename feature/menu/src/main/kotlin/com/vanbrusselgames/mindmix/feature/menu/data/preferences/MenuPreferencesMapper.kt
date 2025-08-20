package com.vanbrusselgames.mindmix.feature.menu.data.preferences

import androidx.datastore.preferences.core.Preferences

fun Preferences.toMenuPreferences(): MenuPreferences {
    return MenuPreferences(selectedGame = this[MenuPreferences.PREF_KEY_SELECTED_GAME] ?: 4)
}