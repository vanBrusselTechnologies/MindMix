package com.vanbrusselgames.mindmix.games.solitaire.model

import kotlinx.serialization.Serializable

@Serializable
data class SolitaireData(
    val progress: List<SolitaireProgress> = listOf(),
    val records: List<SolitaireRecord> = listOf()
)