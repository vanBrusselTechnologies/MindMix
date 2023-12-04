package com.vanbrusselgames.mindmix.solitaire

import kotlinx.serialization.Serializable

@Serializable
data class SolitaireData(val cardStacks: List<List<Int>>, val finished: Boolean)