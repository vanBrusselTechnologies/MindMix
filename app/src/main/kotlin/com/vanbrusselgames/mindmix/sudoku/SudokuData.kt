package com.vanbrusselgames.mindmix.sudoku

import kotlinx.serialization.Serializable

@Serializable
data class SudokuData(
    val clues: List<Int>,
    val input: List<Int>,
    val inputNotes: List<List<Int>>,
    val finished: Boolean
)
//var CheckConflictingCells = true      Preference -> DataStore
//var AutoEditNotes = true              Preference -> DataStore