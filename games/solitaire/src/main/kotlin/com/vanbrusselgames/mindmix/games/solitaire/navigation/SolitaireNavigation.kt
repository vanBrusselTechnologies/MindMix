package com.vanbrusselgames.mindmix.games.solitaire.navigation

import androidx.compose.material3.SnackbarHostState
import androidx.compose.runtime.rememberCoroutineScope
import androidx.navigation.NavController
import androidx.navigation.NavGraphBuilder
import androidx.navigation.NavOptions
import androidx.navigation.compose.composable
import androidx.navigation.navArgument
import com.vanbrusselgames.mindmix.core.logging.Logger
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.games.solitaire.GameUI
import com.vanbrusselgames.mindmix.games.solitaire.SolitaireViewModel
import kotlinx.serialization.Serializable

@Serializable
object SolitaireRoute {
    const val NAV_ROUTE = "game/solitaire?mode={mode}"
    val NAV_ARGUMENTS = listOf(navArgument("mode") { defaultValue = "0" })
}

fun NavController.navigateToSolitaire(navOptions: NavOptions? = null) {
    Logger.d("Navigate to: Solitaire")
    SceneManager.currentScene = SceneManager.Scene.SOLITAIRE
    navigate(SolitaireRoute, navOptions)
}

fun NavGraphBuilder.solitaire(
    navController: NavController,
    viewModel: SolitaireViewModel,
    snackbarHostState: SnackbarHostState
) {
    composable<SolitaireRoute> {
        val coroutineScope = rememberCoroutineScope()
        viewModel.setCoroutineScope(coroutineScope)
        viewModel.loadPuzzle()
        //it.arguments?.getString("mode")
        GameUI(viewModel, navController, snackbarHostState)
    }
}