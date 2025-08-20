package com.vanbrusselgames.mindmix.preferences

import androidx.datastore.preferences.core.Preferences

fun Preferences.toUserPreferences(): UserPreferences {
    return UserPreferences(theme = this[UserPreferences.PREF_KEY_THEME] ?: 0)
}