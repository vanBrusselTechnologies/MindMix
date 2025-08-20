package com.vanbrusselgames.mindmix.games.minesweeper.navigation

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
import com.vanbrusselgames.mindmix.core.designsystem.theme.setFullScreen
import com.vanbrusselgames.mindmix.core.logging.Logger
import com.vanbrusselgames.mindmix.core.model.SceneRegistry
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.games.minesweeper.ui.GameUI
import com.vanbrusselgames.mindmix.games.minesweeper.ui.dialogs.MinesweeperGameFinishedDialog
import com.vanbrusselgames.mindmix.games.minesweeper.ui.dialogs.MinesweeperGameHelpDialog
import com.vanbrusselgames.mindmix.games.minesweeper.ui.dialogs.MinesweeperGameMenuDialog
import com.vanbrusselgames.mindmix.games.minesweeper.ui.dialogs.MinesweeperSettingsDialog
import com.vanbrusselgames.mindmix.games.minesweeper.viewmodel.MinesweeperViewModel
import kotlinx.serialization.Serializable

@Serializable
object MinesweeperFeatureRoute

@Serializable
object/*data class*/ MinesweeperGameRoute/*(val mode: Int = 0)*/

@Serializable
object MinesweeperGameFinishedRoute

@Serializable
object MinesweeperGameHelpRoute

@Serializable
object MinesweeperGameMenuRoute

@Serializable
object MinesweeperSettingsRoute

fun NavController.navigateToMinesweeper(
    route: MinesweeperGameRoute = MinesweeperGameRoute, navOptions: NavOptions? = null
) {
    Logger.d("Navigate to: Minesweeper")
    SceneManager.currentScene = SceneRegistry.Minesweeper
    navigate(route, navOptions)
}

fun NavController.navigateToMinesweeperGameFinished(
    route: MinesweeperGameFinishedRoute = MinesweeperGameFinishedRoute,
    navOptions: NavOptions? = null
) {
    Logger.d("Navigate to: Minesweeper GameFinished")
    navigate(route, navOptions)
}

fun NavController.navigateToMinesweeperGameHelp(
    route: MinesweeperGameHelpRoute = MinesweeperGameHelpRoute, navOptions: NavOptions? = null
) {
    Logger.d("Navigate to: Minesweeper GameHelp")
    navigate(route, navOptions)
}

fun NavController.navigateToMinesweeperGameMenu(
    route: MinesweeperGameMenuRoute = MinesweeperGameMenuRoute, navOptions: NavOptions? = null
) {
    Logger.d("Navigate to: Minesweeper GameMenu")
    navigate(route, navOptions)
}

fun NavController.navigateToMinesweeperSettings(
    route: MinesweeperSettingsRoute = MinesweeperSettingsRoute, navOptions: NavOptions? = null
) {
    Logger.d("Navigate to: Minesweeper Settings")
    navigate(route, navOptions)
}

fun NavGraphBuilder.minesweeper(navController: NavController) {
    navigation<MinesweeperFeatureRoute>(MinesweeperGameRoute) {
        composable<MinesweeperGameRoute> { navBackStackEntry ->
            val vm = hiltViewModel<MinesweeperViewModel>(remember(navBackStackEntry) {
                navController.getBackStackEntry<MinesweeperFeatureRoute>()
            })
            // val route = navBackStackEntry.toRoute<MinesweeperGameRoute>()
            // route.mode
            GameUI(vm, navController)
            BackHandler { navController.navigateToMinesweeperGameMenu() }
        }

        dialog<MinesweeperGameFinishedRoute>(
            dialogProperties = DialogProperties(false, false, false)
        ) { navBackStackEntry ->
            val vm = hiltViewModel<MinesweeperViewModel>(remember(navBackStackEntry) {
                navController.getBackStackEntry<MinesweeperFeatureRoute>()
            })
            val window = (LocalView.current.parent as? DialogWindowProvider)?.window
            if (window != null) setFullScreen(null, window)
            MinesweeperGameFinishedDialog(vm, navController)
        }

        dialog<MinesweeperGameHelpRoute>(
            dialogProperties = DialogProperties(true, false, false)
        ) { navBackStackEntry ->
            val window = (LocalView.current.parent as? DialogWindowProvider)?.window
            if (window != null) setFullScreen(null, window)
            MinesweeperGameHelpDialog(navController)
        }

        dialog<MinesweeperGameMenuRoute>(
            dialogProperties = DialogProperties(true, false, false)
        ) { navBackStackEntry ->
            val vm = hiltViewModel<MinesweeperViewModel>(remember(navBackStackEntry) {
                navController.getBackStackEntry<MinesweeperFeatureRoute>()
            })
            val window = (LocalView.current.parent as? DialogWindowProvider)?.window
            if (window != null) setFullScreen(null, window)
            MinesweeperGameMenuDialog(vm, navController)
        }

        dialog<MinesweeperSettingsRoute>(
            dialogProperties = DialogProperties(true, false, false)
        ) { navBackStackEntry ->
            val vm = hiltViewModel<MinesweeperViewModel>(remember(navBackStackEntry) {
                navController.getBackStackEntry<MinesweeperFeatureRoute>()
            })
            val window = (LocalView.current.parent as? DialogWindowProvider)?.window
            if (window != null) setFullScreen(null, window)
            MinesweeperSettingsDialog(vm, navController)
        }
    }
}