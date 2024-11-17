package com.vanbrusselgames.mindmix.games.minesweeper.navigation

import androidx.navigation.NavController
import androidx.navigation.NavGraphBuilder
import androidx.navigation.NavOptions
import androidx.navigation.compose.composable
import androidx.navigation.navArgument
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.games.minesweeper.GameUI
import com.vanbrusselgames.mindmix.games.minesweeper.GameViewModel
import kotlinx.serialization.Serializable

@Serializable
object MinesweeperRoute {
    const val NAV_ROUTE = "game/minesweeper?mode={mode}"
    val NAV_ARGUMENTS = listOf(navArgument("mode") { defaultValue = "0" })
}

fun NavController.navigateToMinesweeper(navOptions: NavOptions? = null) {
    SceneManager.currentScene = SceneManager.Scene.MINESWEEPER
    navigate(route = MinesweeperRoute, navOptions)
}

fun NavGraphBuilder.minesweeper(
    navController: NavController, viewModel: GameViewModel
) {
    composable<MinesweeperRoute> {
        viewModel.loadPuzzle()
        //it.arguments?.getString("mode")
        GameUI(viewModel, navController)
    }
}