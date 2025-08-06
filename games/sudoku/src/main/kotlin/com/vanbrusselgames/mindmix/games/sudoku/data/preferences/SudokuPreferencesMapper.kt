package com.vanbrusselgames.mindmix.games.sudoku.data.preferences

import androidx.datastore.preferences.core.Preferences

fun Preferences.toSudokuPreferences(): SudokuPreferences {
    return SudokuPreferences(
        autoEditNotes = this[SudokuPreferences.PREF_KEY_AUTO_EDIT_NOTES] ?: false,
        checkConflictingCells = this[SudokuPreferences.PREF_KEY_CHECK_CONFLICTING_CELLS] ?: true,
        difficulty = this[SudokuPreferences.PREF_KEY_DIFFICULTY] ?: 1
    )
}