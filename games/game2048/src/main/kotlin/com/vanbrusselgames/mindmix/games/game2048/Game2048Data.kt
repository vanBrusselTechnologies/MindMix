package com.vanbrusselgames.mindmix.games.game2048

import kotlinx.serialization.Serializable

@Serializable
data class Game2048Data(
    val size: GridSize2048 = GridSize2048.FOUR,
    val progress: List<Game2048Progress> = listOf()
)