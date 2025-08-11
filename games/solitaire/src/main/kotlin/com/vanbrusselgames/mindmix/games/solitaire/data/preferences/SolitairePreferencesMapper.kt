package com.vanbrusselgames.mindmix.games.solitaire.data.preferences

import androidx.datastore.preferences.core.Preferences
import com.vanbrusselgames.mindmix.games.solitaire.model.CardVisualType

fun Preferences.toSolitairePreferences(): SolitairePreferences {
    return SolitairePreferences(
        cardVisualType = this[SolitairePreferences.PREF_KEY_CARD_TYPE]
            ?: CardVisualType.SIMPLE.ordinal,
    )
}