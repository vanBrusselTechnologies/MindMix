package com.vanbrusselgames.mindmix.games.game2048.navigation

import androidx.activity.compose.BackHandler
import androidx.compose.runtime.remember
import androidx.compose.ui.platform.LocalView
import androidx.compose.ui.window.DialogProperties
import androidx.compose.ui.window.DialogWindowProvider
import androidx.hilt.lifecycle.viewmodel.compose.hiltViewModel
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
import com.vanbrusselgames.mindmix.games.game2048.ui.GameUI
import com.vanbrusselgames.mindmix.games.game2048.ui.dialogs.Game2048GameFinishedDialog
import com.vanbrusselgames.mindmix.games.game2048.ui.dialogs.Game2048GameHelpDialog
import com.vanbrusselgames.mindmix.games.game2048.ui.dialogs.Game2048GameMenuDialog
import com.vanbrusselgames.mindmix.games.game2048.ui.dialogs.Game2048SettingsDialog
import com.vanbrusselgames.mindmix.games.game2048.viewmodel.Game2048ViewModel
import kotlinx.serialization.Serializable

@Serializable
object Game2048FeatureRoute

@Serializable
object/*data class*/ Game2048GameRoute/*(val mode: Int = 0)*/

@Serializable
object Game2048GameFinishedRoute

@Serializable
object Game2048GameHelpRoute

@Serializable
object Game2048GameMenuRoute

@Serializable
object Game2048SettingsRoute

fun NavController.navigateToGame2048(
    route: Game2048GameRoute = Game2048GameRoute, navOptions: NavOptions? = null
) {
    Logger.d("Navigate to: 2048")
    SceneManager.currentScene = SceneRegistry.Game2048
    navigate(route, navOptions)
}

fun NavController.navigateToGame2048GameFinished(
    route: Game2048GameFinishedRoute = Game2048GameFinishedRoute, navOptions: NavOptions? = null
) {
    Logger.d("Navigate to: 2048 GameFinished")
    navigate(route, navOptions)
}

fun NavController.navigateToGame2048GameHelp(
    route: Game2048GameHelpRoute = Game2048GameHelpRoute, navOptions: NavOptions? = null
) {
    Logger.d("Navigate to: 2048 GameHelp")
    navigate(route, navOptions)
}

fun NavController.navigateToGame2048GameMenu(
    route: Game2048GameMenuRoute = Game2048GameMenuRoute, navOptions: NavOptions? = null
) {
    Logger.d("Navigate to: 2048 GameMenu")
    navigate(route, navOptions)
}

fun NavController.navigateToGame2048Settings(
    route: Game2048SettingsRoute = Game2048SettingsRoute, navOptions: NavOptions? = null
) {
    Logger.d("Navigate to: 2048 Settings")
    navigate(route, navOptions)
}

fun NavGraphBuilder.game2048(navController: NavController) {
    navigation<Game2048FeatureRoute>(Game2048GameRoute) {
        composable<Game2048GameRoute> { navBackStackEntry ->
            val vm = hiltViewModel<Game2048ViewModel>(remember(navBackStackEntry) {
                navController.getBackStackEntry<Game2048FeatureRoute>()
            })
            // val route = navBackStackEntry.toRoute<Game2048GameRoute>()
            // route.mode
            GameUI(vm, navController)
            BackHandler { navController.navigateToGame2048GameMenu() }
        }

        dialog<Game2048GameFinishedRoute>(
            dialogProperties = DialogProperties(false, false, false)
        ) { navBackStackEntry ->
            val vm = hiltViewModel<Game2048ViewModel>(remember(navBackStackEntry) {
                navController.getBackStackEntry<Game2048FeatureRoute>()
            })
            val window = (LocalView.current.parent as? DialogWindowProvider)?.window
            if (window != null) setFullScreen(null, window)
            Game2048GameFinishedDialog(vm, navController)
        }

        dialog<Game2048GameHelpRoute>(
            dialogProperties = DialogProperties(true, false, false)
        ) { navBackStackEntry ->
            val window = (LocalView.current.parent as? DialogWindowProvider)?.window
            if (window != null) setFullScreen(null, window)
            Game2048GameHelpDialog(navController)
        }

        dialog<Game2048GameMenuRoute>(
            dialogProperties = DialogProperties(true, false, false)
        ) { navBackStackEntry ->
            val vm = hiltViewModel<Game2048ViewModel>(remember(navBackStackEntry) {
                navController.getBackStackEntry<Game2048FeatureRoute>()
            })
            val window = (LocalView.current.parent as? DialogWindowProvider)?.window
            if (window != null) setFullScreen(null, window)
            Game2048GameMenuDialog(vm, navController)
        }

        dialog<Game2048SettingsRoute>(
            dialogProperties = DialogProperties(true, false, false)
        ) { navBackStackEntry ->
            val vm = hiltViewModel<Game2048ViewModel>(remember(navBackStackEntry) {
                navController.getBackStackEntry<Game2048FeatureRoute>()
            })
            val window = (LocalView.current.parent as? DialogWindowProvider)?.window
            if (window != null) setFullScreen(null, window)
            Game2048SettingsDialog(vm, navController)
        }
    }
}