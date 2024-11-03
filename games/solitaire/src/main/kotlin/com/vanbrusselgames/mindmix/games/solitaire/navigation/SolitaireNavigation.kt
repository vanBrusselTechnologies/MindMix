package com.vanbrusselgames.mindmix.games.solitaire.navigation

import androidx.navigation.NavController
import androidx.navigation.NavGraphBuilder
import androidx.navigation.NavOptions
import androidx.navigation.compose.composable
import androidx.navigation.navArgument
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.games.solitaire.GameUI
import com.vanbrusselgames.mindmix.games.solitaire.GameViewModel
import kotlinx.serialization.Serializable

@Serializable
object SolitaireRoute {
    const val NAV_ROUTE = "game/solitaire?mode={mode}"
    val NAV_ARGUMENTS = listOf(navArgument("mode") { defaultValue = "0" })
}

fun NavController.navigateToSolitaire(navOptions: NavOptions? = null) {
    SceneManager.currentScene = SceneManager.Scene.SOLITAIRE
    navigate(route = SolitaireRoute, navOptions)
}

fun NavGraphBuilder.solitaire(
    navController: NavController, viewModel: GameViewModel
) {
    viewModel.loadPuzzle()
    composable<SolitaireRoute> {
        //it.arguments?.getString("mode")
        GameUI(viewModel, navController)
    }
}