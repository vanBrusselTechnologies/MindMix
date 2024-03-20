package com.vanbrusselgames.mindmix.games.sudoku

enum class Difficulty {
    EASY, MEDIUM, HARD, EXPERT
}

enum class InputMode {
    Normal, Note
}

enum class PuzzleType {
    Classic
}

const val GAME_NAME = "Sudoku"

val CluesPerDifficulty: Map<Difficulty, Int> = mapOf(
    Difficulty.EASY to 50,
    Difficulty.MEDIUM to 30,
    Difficulty.HARD to 17,
    Difficulty.EXPERT to 22,
    //4 to 17
)