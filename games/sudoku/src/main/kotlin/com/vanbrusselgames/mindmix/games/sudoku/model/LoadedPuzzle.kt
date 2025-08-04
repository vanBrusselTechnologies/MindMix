package com.vanbrusselgames.mindmix.games.sudoku.model

import com.vanbrusselgames.mindmix.core.utils.constants.Difficulty

data class LoadedPuzzle(val difficulty: Difficulty, val clues: List<Int>) {
    fun toSudokuProgress(): SudokuProgress {
        return SudokuProgress(clues, clues, clues.map { listOf() }, difficulty)
    }
}