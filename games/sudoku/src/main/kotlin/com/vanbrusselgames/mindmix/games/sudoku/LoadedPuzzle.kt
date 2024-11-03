package com.vanbrusselgames.mindmix.games.sudoku

import com.vanbrusselgames.mindmix.core.utils.constants.Difficulty

data class LoadedPuzzle(val size: Int, val difficulty: Difficulty, val clues: List<Int>)