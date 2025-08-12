package com.vanbrusselgames.mindmix.games.minesweeper.model

data class FinishedGame(
    val successType: SuccessType = SuccessType.GAME_OVER,
    val reward: Int = 0,
)