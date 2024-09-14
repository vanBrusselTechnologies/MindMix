package com.vanbrusselgames.mindmix.games.game2048

import com.vanbrusselgames.mindmix.R

data class Game2048(val viewModel: GameViewModel = GameViewModel()){
    companion object {
        val GAME_ID: Int = 3
        val NAME_RES_ID: Int = R.string.game2048_name
        val IMAGE_RES_ID: Int = R.drawable.game_icon_2048_4x4
    }
}