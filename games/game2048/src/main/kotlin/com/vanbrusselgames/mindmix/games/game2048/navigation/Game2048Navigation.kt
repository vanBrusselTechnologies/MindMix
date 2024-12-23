package com.vanbrusselgames.mindmix.games.game2048.navigation

import androidx.compose.material3.SnackbarHostState
import androidx.compose.runtime.remember
import androidx.navigation.NamedNavArgument
import androidx.navigation.NavController
import androidx.navigation.NavGraphBuilder
import androidx.navigation.NavOptions
import androidx.navigation.compose.composable
import com.vanbrusselgames.mindmix.core.logging.Logger
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.games.game2048.Game2048ViewModel
import com.vanbrusselgames.mindmix.games.game2048.GameUI
import kotlinx.serialization.Serializable

@Serializable
object Game2048Route {
    const val NAV_ROUTE = "game/2048"
    val NAV_ARGUMENTS = emptyList<NamedNavArgument>()
}

fun NavController.navigateTo2048(navOptions: NavOptions? = null) {
    Logger.d("Navigate to: 2048")
    SceneManager.currentScene = SceneManager.Scene.GAME2048
    navigate(Game2048Route, navOptions)
}

fun NavGraphBuilder.game2048(
    navController: NavController, viewModel: Game2048ViewModel, snackbarHostState: SnackbarHostState
) {
    composable<Game2048Route> {
        remember { viewModel.loadPuzzle();0 }
        GameUI(viewModel, navController, snackbarHostState)
    }
}