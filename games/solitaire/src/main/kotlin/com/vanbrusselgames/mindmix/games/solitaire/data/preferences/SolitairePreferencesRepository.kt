package com.vanbrusselgames.mindmix.games.solitaire.data.preferences

import android.content.Context
import com.vanbrusselgames.mindmix.core.data.dataStore
import dagger.hilt.android.qualifiers.ApplicationContext
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.map
import javax.inject.Inject

class SolitairePreferencesRepository @Inject constructor(@ApplicationContext ctx: Context) {
    private val dataStore = ctx.dataStore

    fun getPreferences(): Flow<SolitairePreferences> {
        return dataStore.data.map { it.toSolitairePreferences() }
    }

    suspend fun savePreferences(prefs: SolitairePreferences) {
        dataStore.saveSolitairePreferences(prefs)
    }
}