package com.vanbrusselgames.mindmix.games.game2048

import kotlinx.serialization.Serializable

@Serializable
data class Game2048Progress(
    var cellValues: List<Long> = listOf(),
    var moves: Int = -1,
    var currentScore: Long = -1,
    var bestScore: Long = -1,
    val size: GridSize2048 = GridSize2048.FOUR,
    var isStuck: Boolean = false
)