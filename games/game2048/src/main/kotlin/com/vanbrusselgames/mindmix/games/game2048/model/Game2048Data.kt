package com.vanbrusselgames.mindmix.games.game2048.model

import kotlinx.serialization.Serializable

@Serializable
data class Game2048Data(
    val progress: List<Game2048Progress> = listOf(), val records: List<Game2048Record> = listOf()
)