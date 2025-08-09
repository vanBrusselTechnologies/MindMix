package com.vanbrusselgames.mindmix.games.minesweeper.data.preferences

import androidx.datastore.preferences.core.Preferences
import com.vanbrusselgames.mindmix.core.utils.constants.Difficulty

fun Preferences.toMinesweeperPreferences(): MinesweeperPreferences {
    return MinesweeperPreferences(
        autoFlag = this[MinesweeperPreferences.PREF_KEY_AUTO_FLAG] ?: false,
        difficulty = this[MinesweeperPreferences.PREF_KEY_DIFFICULTY] ?: Difficulty.MEDIUM.ordinal,
        safeStart = this[MinesweeperPreferences.PREF_KEY_SAFE_START] ?: false,
    )
}