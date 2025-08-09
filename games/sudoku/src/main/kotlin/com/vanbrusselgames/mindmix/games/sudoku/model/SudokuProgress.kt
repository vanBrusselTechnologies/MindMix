package com.vanbrusselgames.mindmix.games.sudoku.model

import com.vanbrusselgames.mindmix.core.utils.constants.Difficulty
import kotlinx.serialization.Serializable

@Serializable
data class SudokuProgress(
    val clues: List<Int> = listOf(),
    val input: List<Int> = listOf(),
    val inputNotes: List<List<Int>> = listOf(),
    val difficulty: Difficulty = Difficulty.MEDIUM
)