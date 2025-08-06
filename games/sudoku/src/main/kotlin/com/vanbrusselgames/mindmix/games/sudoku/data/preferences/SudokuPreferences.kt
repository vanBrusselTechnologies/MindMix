package com.vanbrusselgames.mindmix.games.sudoku.data.preferences

import androidx.datastore.preferences.core.booleanPreferencesKey
import androidx.datastore.preferences.core.intPreferencesKey

data class SudokuPreferences(
    val autoEditNotes: Boolean = false,
    val checkConflictingCells: Boolean = true,
    val difficulty: Int
) {
    companion object {
        val PREF_KEY_AUTO_EDIT_NOTES = booleanPreferencesKey("sudoku_autoEditNotes")
        val PREF_KEY_CHECK_CONFLICTING_CELLS = booleanPreferencesKey("sudoku_checkConflictingCells")
        val PREF_KEY_DIFFICULTY = intPreferencesKey("sudoku_difficulty")
    }
}