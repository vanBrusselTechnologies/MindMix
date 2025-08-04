package com.vanbrusselgames.mindmix.games.sudoku.model

import com.vanbrusselgames.mindmix.core.utils.constants.Difficulty
import kotlinx.serialization.Serializable

@Serializable
data class SudokuData(
    val progress: List<SudokuSavedProgress> = listOf(),
    val page: Map<Difficulty, Int> = mutableMapOf()
)