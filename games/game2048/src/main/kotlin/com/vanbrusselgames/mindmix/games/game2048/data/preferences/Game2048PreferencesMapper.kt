package com.vanbrusselgames.mindmix.games.game2048.data.preferences

import androidx.datastore.preferences.core.Preferences

fun Preferences.toGame2048Preferences(): Game2048Preferences {
    return Game2048Preferences(
        size = this[Game2048Preferences.PREF_KEY_SIZE] ?: 1
    )
}