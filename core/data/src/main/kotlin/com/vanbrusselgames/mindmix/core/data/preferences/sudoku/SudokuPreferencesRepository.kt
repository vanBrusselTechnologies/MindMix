package com.vanbrusselgames.mindmix.core.data.preferences.sudoku

import android.content.Context
import com.vanbrusselgames.mindmix.core.data.dataStore
import dagger.hilt.android.qualifiers.ApplicationContext
import kotlinx.coroutines.flow.first
import javax.inject.Inject

class SudokuPreferencesRepository @Inject constructor(
    @ApplicationContext private val context: Context
) {
    private val dataStore = context.dataStore

    suspend fun getPreferences(): SudokuPreferences {
        return dataStore.data.first().toSudokuPreferences()
    }

    suspend fun savePreferences(prefs: SudokuPreferences) {
        dataStore.saveSudokuPreferences(prefs)
    }
}