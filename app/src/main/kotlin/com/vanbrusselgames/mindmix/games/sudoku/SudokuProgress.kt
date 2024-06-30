package com.vanbrusselgames.mindmix.games.sudoku

import com.vanbrusselgames.mindmix.utils.constants.Difficulty
import kotlinx.serialization.Serializable

@Serializable
data class SudokuProgress<T>(
    val clues: T,
    var input: T,
    var inputNotes: List<T>,
    val difficulty: Difficulty
)