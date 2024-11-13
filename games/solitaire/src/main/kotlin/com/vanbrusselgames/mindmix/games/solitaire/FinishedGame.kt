package com.vanbrusselgames.mindmix.games.solitaire

object FinishedGame {
    var titleResId: Int = 0
    var textResId: Int = 0
    var reward: Int = 0
    var usedMillis: Long = 0
    var penaltyMillis: Long = 0
    var lastRecordMillis: Long = -1
    var isNewRecord = false
    var moves: Int = 0
}