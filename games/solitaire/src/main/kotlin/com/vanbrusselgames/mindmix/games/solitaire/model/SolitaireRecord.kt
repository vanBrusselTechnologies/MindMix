package com.vanbrusselgames.mindmix.games.solitaire.model

import kotlinx.serialization.Serializable

@Serializable
data class SolitaireRecord(val fastestMillis: Long = -1L)