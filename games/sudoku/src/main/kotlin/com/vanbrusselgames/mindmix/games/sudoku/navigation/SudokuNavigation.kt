package com.vanbrusselgames.mindmix.games.sudoku.navigation

import androidx.navigation.NavController
import androidx.navigation.NavGraphBuilder
import androidx.navigation.NavOptions
import androidx.navigation.compose.composable
import androidx.navigation.navArgument
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.games.sudoku.GameUI
import com.vanbrusselgames.mindmix.games.sudoku.GameViewModel
import kotlinx.serialization.Serializable

@Serializable
object SudokuRoute {
    const val NAV_ROUTE = "game/sudoku?mode={mode}"
    val NAV_ARGUMENTS = listOf(navArgument("mode") { defaultValue = "0" })
}

fun NavController.navigateToSudoku(navOptions: NavOptions? = null) {
    SceneManager.currentScene = SceneManager.Scene.SUDOKU
    navigate(route = SudokuRoute, navOptions)
}

fun NavGraphBuilder.sudoku(
    navController: NavController, viewModel: GameViewModel
) {
    composable<SudokuRoute> {
        viewModel.startPuzzle()
        //it.arguments?.getString("mode")
        GameUI(viewModel, navController)
    }
}