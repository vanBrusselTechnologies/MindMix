package com.vanbrusselgames.mindmix.games.sudoku

import kotlinx.serialization.Serializable

@Serializable
data class SudokuData(
    val clues: List<Int>,
    val input: List<Int>,
    val inputNotes: List<List<Int>>,
    val finished: Boolean,
    val difficulty: Difficulty = SudokuManager.difficulty
)