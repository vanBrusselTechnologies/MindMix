package com.vanbrusselgames.mindmix.games.game2048

class Game2048(val viewModel: GameViewModel = GameViewModel()) {
    companion object {
        const val GAME_ID: Int = 3
        const val GAME_NAME = "2048"

        val NAME_RES_ID: Int = R.string.game_2048_name
        val IMAGE_RES_ID: Int = R.drawable.game_icon_2048_4x4
    }
}