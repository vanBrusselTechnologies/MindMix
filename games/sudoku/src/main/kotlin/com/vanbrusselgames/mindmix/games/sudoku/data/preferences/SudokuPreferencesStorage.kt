package com.vanbrusselgames.mindmix.games.sudoku.data.preferences

import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.core.edit

suspend fun DataStore<Preferences>.saveSudokuPreferences(prefs: SudokuPreferences) {
    edit { preferences ->
        preferences[SudokuPreferences.PREF_KEY_AUTO_EDIT_NOTES] = prefs.autoEditNotes
        preferences[SudokuPreferences.PREF_KEY_CHECK_CONFLICTING_CELLS] =
            prefs.checkConflictingCells
        preferences[SudokuPreferences.PREF_KEY_DIFFICULTY] = prefs.difficulty
    }
}