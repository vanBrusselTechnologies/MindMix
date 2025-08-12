package com.vanbrusselgames.mindmix.games.solitaire.model

data class FinishedGame(
    val reward: Int = 0,
    val usedMillis: Long = 0,
    val penaltyMillis: Long = 0,
    val lastRecordMillis: Long = -1,
    val isNewRecord: Boolean = false,
    val moves: Int = 0,
)