package com.vanbrusselgames.mindmix.games.game2048.model

import kotlinx.serialization.Serializable

@Serializable
data class Game2048Record(
    val size: GridSize2048 = GridSize2048.FOUR,
    val bestScore: Long = -1,
    val minMovesToTarget: Int = -1,
)