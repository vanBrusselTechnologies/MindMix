package com.vanbrusselgames.mindmix.games.solitaire.data.preferences

import androidx.datastore.preferences.core.intPreferencesKey
import com.vanbrusselgames.mindmix.games.solitaire.model.CardVisualType

data class SolitairePreferences(val cardVisualType: Int = CardVisualType.SIMPLE.ordinal) {
    companion object {
        val PREF_KEY_CARD_TYPE = intPreferencesKey("solitaire_cardType")
    }
}