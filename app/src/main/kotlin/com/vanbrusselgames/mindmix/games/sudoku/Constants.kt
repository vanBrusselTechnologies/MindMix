package com.vanbrusselgames.mindmix.games.sudoku

import com.vanbrusselgames.mindmix.utils.constants.Difficulty

enum class InputMode {
    Normal, Note
}

enum class PuzzleType {
    Classic
}

const val GAME_NAME = "Sudoku"

const val GAME_ID = 0

val enabledDifficulties = listOf(Difficulty.EASY, Difficulty.MEDIUM, Difficulty.HARD, Difficulty.EXPERT, Difficulty.MASTER)