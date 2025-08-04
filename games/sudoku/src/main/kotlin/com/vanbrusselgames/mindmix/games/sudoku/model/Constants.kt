package com.vanbrusselgames.mindmix.games.sudoku.model

import com.vanbrusselgames.mindmix.core.utils.constants.Difficulty

enum class InputMode {
    Normal, Note
}

enum class PuzzleType {
    Classic
}

val enabledDifficulties = listOf(
    Difficulty.EASY, Difficulty.MEDIUM, Difficulty.HARD, Difficulty.EXPERT, Difficulty.MASTER
)

val rewardForDifficulty = mapOf(
    Difficulty.EASY to 3,
    Difficulty.MEDIUM to 7,
    Difficulty.HARD to 12,
    Difficulty.EXPERT to 18,
    Difficulty.MASTER to 25
)