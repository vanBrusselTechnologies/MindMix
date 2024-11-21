package com.vanbrusselgames.mindmix.games.sudoku

class Sudoku(val viewModel: SudokuViewModel = SudokuViewModel()) {
    companion object {
        const val GAME_ID: Int = 0
        const val GAME_NAME = "Sudoku"

        val NAME_RES_ID: Int = R.string.sudoku_name
        val IMAGE_RES_ID: Int = R.drawable.game_icon_sudoku
    }
}