package com.vanbrusselgames.mindmix.games.solitaire

import kotlinx.serialization.Serializable

@Serializable
data class SolitaireData(
    val cardStacks: List<List<Int>>,
    val finished: Boolean,
    val millis: Long,
    val penaltyMillis: Long = 0L,
    val fastestMillis: Long = -1L,
    val moves: Int = 0
)