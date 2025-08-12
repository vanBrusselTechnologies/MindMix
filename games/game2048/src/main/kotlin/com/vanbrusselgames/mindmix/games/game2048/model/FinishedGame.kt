package com.vanbrusselgames.mindmix.games.game2048.model

data class FinishedGame(
    val successType: SuccessType = SuccessType.GAME_OVER,
    val reward: Int = 0,
    val highestTileValue: Long = 0,
    val targetTile: Long = 0,
    val score: Long = 0,
)