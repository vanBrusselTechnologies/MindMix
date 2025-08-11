package com.vanbrusselgames.mindmix.games.solitaire.data.preferences

import android.content.Context
import com.vanbrusselgames.mindmix.core.data.dataStore
import dagger.hilt.android.qualifiers.ApplicationContext
import kotlinx.coroutines.flow.first
import javax.inject.Inject

class SolitairePreferencesRepository @Inject constructor(
    @ApplicationContext private val context: Context
) {
    private val dataStore = context.dataStore

    suspend fun getPreferences(): SolitairePreferences {
        return dataStore.data.first().toSolitairePreferences()
    }

    suspend fun savePreferences(prefs: SolitairePreferences) {
        dataStore.saveSolitairePreferences(prefs)
    }
}