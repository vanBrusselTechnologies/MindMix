package com.vanbrusselgames.mindmix.games.sudoku.model

import com.vanbrusselgames.mindmix.core.utils.constants.Difficulty
import kotlinx.serialization.Serializable

@Serializable
data class SudokuSavedProgress(
    val clues: String, var input: String, var inputNotes: List<String>, val difficulty: Difficulty
)