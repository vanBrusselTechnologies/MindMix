package com.vanbrusselgames.mindmix.games.sudoku.data

import androidx.datastore.preferences.core.booleanPreferencesKey

internal val PREF_KEY_CHECK_CONFLICTING_CELLS = booleanPreferencesKey("sudoku_checkConflictingCells")
internal val PREF_KEY_AUTO_EDIT_NOTES = booleanPreferencesKey("sudoku_autoEditNotes")