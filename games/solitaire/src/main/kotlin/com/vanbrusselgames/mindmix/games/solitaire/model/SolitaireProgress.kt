package com.vanbrusselgames.mindmix.games.solitaire.model

import kotlinx.serialization.Serializable

@Serializable
data class SolitaireProgress(
    val cardStacks: List<List<Int>> = listOf(),
    val millis: Long = 0L,
    val penaltyMillis: Long = 0L,
    val moves: Int = 0
)