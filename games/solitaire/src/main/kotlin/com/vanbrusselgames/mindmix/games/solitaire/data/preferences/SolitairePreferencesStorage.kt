package com.vanbrusselgames.mindmix.games.solitaire.data.preferences

import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.core.edit

suspend fun DataStore<Preferences>.saveSolitairePreferences(prefs: SolitairePreferences) {
    edit { preferences ->
        preferences[SolitairePreferences.PREF_KEY_CARD_TYPE] = prefs.cardVisualType
    }
}