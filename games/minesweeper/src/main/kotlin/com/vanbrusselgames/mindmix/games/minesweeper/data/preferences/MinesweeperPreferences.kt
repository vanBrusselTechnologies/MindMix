package com.vanbrusselgames.mindmix.games.minesweeper.data.preferences

import androidx.datastore.preferences.core.booleanPreferencesKey
import androidx.datastore.preferences.core.intPreferencesKey
import com.vanbrusselgames.mindmix.core.utils.constants.Difficulty

data class MinesweeperPreferences(
    val autoFlag: Boolean = false,
    val difficulty: Int = Difficulty.MEDIUM.ordinal,
    val safeStart: Boolean = true,
) {
    companion object {
        val PREF_KEY_AUTO_FLAG = booleanPreferencesKey("minesweeper_autoFlag")
        val PREF_KEY_DIFFICULTY = intPreferencesKey("minesweeper_difficulty")
        val PREF_KEY_SAFE_START = booleanPreferencesKey("minesweeper_safeStart")
    }
}