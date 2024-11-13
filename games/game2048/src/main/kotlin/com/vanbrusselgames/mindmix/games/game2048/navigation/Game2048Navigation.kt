package com.vanbrusselgames.mindmix.games.game2048.navigation

import androidx.navigation.NamedNavArgument
import androidx.navigation.NavController
import androidx.navigation.NavGraphBuilder
import androidx.navigation.NavOptions
import androidx.navigation.compose.composable
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.games.game2048.GameUI
import com.vanbrusselgames.mindmix.games.game2048.GameViewModel
import kotlinx.serialization.Serializable

@Serializable
object Game2048Route {
    const val NAV_ROUTE = "game/2048"
    val NAV_ARGUMENTS = emptyList<NamedNavArgument>()
}

fun NavController.navigateTo2048(navOptions: NavOptions? = null) {
    SceneManager.currentScene = SceneManager.Scene.GAME2048
    navigate(route = Game2048Route, navOptions)
}

fun NavGraphBuilder.game2048(
    navController: NavController, viewModel: GameViewModel
) {
    composable<Game2048Route> {
        GameUI(viewModel, navController)
    }
}