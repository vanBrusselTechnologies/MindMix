package com.vanbrusselgames.mindmix.games.minesweeper

data class Minesweeper(val viewModel: MinesweeperViewModel = MinesweeperViewModel()) {
    companion object {
        const val GAME_ID: Int = 2
        const val GAME_NAME = "Minesweeper"

        val NAME_RES_ID: Int = R.string.minesweeper_name
        val IMAGE_RES_ID: Int = R.drawable.game_icon_minesweeper
    }
}