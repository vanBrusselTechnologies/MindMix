package com.vanbrusselgames.mindmix.games.game2048.model

import kotlinx.serialization.Serializable

@Serializable
data class Game2048Progress(
    val size: GridSize2048 = GridSize2048.FOUR,
    val cellValues: List<Long> = listOf(),
    val moves: Int = -1,
    val bestScore: Long = -1,
    val score: Long = -1
)