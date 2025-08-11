package com.vanbrusselgames.mindmix.games.sudoku.navigation

import androidx.activity.compose.BackHandler
import androidx.compose.runtime.remember
import androidx.compose.ui.platform.LocalView
import androidx.compose.ui.window.DialogProperties
import androidx.compose.ui.window.DialogWindowProvider
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavController
import androidx.navigation.NavGraphBuilder
import androidx.navigation.NavOptions
import androidx.navigation.compose.composable
import androidx.navigation.compose.dialog
import androidx.navigation.navigation
import com.vanbrusselgames.mindmix.core.common.BaseScreenViewModel
import com.vanbrusselgames.mindmix.core.designsystem.theme.setFullScreen
import com.vanbrusselgames.mindmix.core.logging.Logger
import com.vanbrusselgames.mindmix.core.model.SceneRegistry
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.games.sudoku.ui.GameUI
import com.vanbrusselgames.mindmix.games.sudoku.ui.dialogs.SudokuGameMenuDialog
import com.vanbrusselgames.mindmix.games.sudoku.ui.dialogs.SudokuSettingsDialog
import com.vanbrusselgames.mindmix.games.sudoku.viewmodel.SudokuViewModel
import kotlinx.serialization.Serializable

@Serializable
object SudokuFeatureRoute

@Serializable
object/*data class*/ SudokuGameRoute/*(val mode: Int = 0)*/

@Serializable
object SudokuGameMenuRoute

@Serializable
object SudokuSettingsRoute

fun NavController.navigateToSudoku(
    route: SudokuGameRoute = SudokuGameRoute, navOptions: NavOptions? = null
) {
    Logger.d("Navigate to: Sudoku")
    SceneManager.currentScene = SceneRegistry.Sudoku
    navigate(route, navOptions)
}

fun NavController.navigateToSudokuGameMenu(
    route: SudokuGameMenuRoute = SudokuGameMenuRoute, navOptions: NavOptions? = null
) {
    Logger.d("Navigate to: Sudoku GameMenu")
    navigate(route, navOptions)
}

fun NavController.navigateToSudokuSettings(
    route: SudokuSettingsRoute = SudokuSettingsRoute, navOptions: NavOptions? = null
) {
    Logger.d("Navigate to: Sudoku Settings")
    navigate(route, navOptions)
}

fun NavGraphBuilder.sudoku(
    navController: NavController, setCurrentViewModel: (BaseScreenViewModel?) -> Unit
) {
    navigation<SudokuFeatureRoute>(SudokuGameRoute) {
        composable<SudokuGameRoute> { navBackStackEntry ->
            val vm = hiltViewModel<SudokuViewModel>(remember(navBackStackEntry) {
                navController.getBackStackEntry<SudokuFeatureRoute>()
            })
            setCurrentViewModel(vm)
            // val route = navBackStackEntry.toRoute<SudokuGameRoute>()
            // route.mode
            GameUI(vm, navController)
            BackHandler { navController.navigateToSudokuGameMenu() }
        }

        dialog<SudokuGameMenuRoute>(
            dialogProperties = DialogProperties(true, false, false)
        ) { navBackStackEntry ->
            val vm = hiltViewModel<SudokuViewModel>(remember(navBackStackEntry) {
                navController.getBackStackEntry<SudokuFeatureRoute>()
            })
            val window = (LocalView.current.parent as? DialogWindowProvider)?.window
            if (window != null) setFullScreen(null, window)
            SudokuGameMenuDialog(vm, navController)
        }

        dialog<SudokuSettingsRoute>(
            dialogProperties = DialogProperties(true, false, false)
        ) { navBackStackEntry ->
            val vm = hiltViewModel<SudokuViewModel>(remember(navBackStackEntry) {
                navController.getBackStackEntry<SudokuFeatureRoute>()
            })
            val window = (LocalView.current.parent as? DialogWindowProvider)?.window
            if (window != null) setFullScreen(null, window)
            SudokuSettingsDialog(vm, navController)
        }
    }
}