package com.vanbrusselgames.mindmix.games.game2048

import androidx.navigation.NamedNavArgument
import com.vanbrusselgames.mindmix.R

data class Game2048(val viewModel: GameViewModel = GameViewModel()) {
    companion object {
        const val GAME_ID: Int = 3

        const val NAV_ROUTE = "game/2048"
        val NAV_ARGUMENTS = emptyList<NamedNavArgument>()

        val NAME_RES_ID: Int = R.string.game2048_name
        val IMAGE_RES_ID: Int = R.drawable.game_icon_2048_4x4
    }
}