package com.vanbrusselgames.mindmix.games.game2048.data.preferences

import androidx.datastore.preferences.core.intPreferencesKey

data class Game2048Preferences(val size: Int) {
    companion object {
        val PREF_KEY_SIZE = intPreferencesKey("game2048_size")
    }
}