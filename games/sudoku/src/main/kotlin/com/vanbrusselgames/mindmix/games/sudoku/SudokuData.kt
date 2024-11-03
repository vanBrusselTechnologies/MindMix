package com.vanbrusselgames.mindmix.games.sudoku

import com.vanbrusselgames.mindmix.core.utils.constants.Difficulty
import kotlinx.serialization.Serializable

@Serializable
data class SudokuData(
    val difficulty: Difficulty = Difficulty.MEDIUM,
    val progress: List<SudokuProgress<String>> = listOf(),
    val page: Map<Difficulty, Int> = mutableMapOf()
)