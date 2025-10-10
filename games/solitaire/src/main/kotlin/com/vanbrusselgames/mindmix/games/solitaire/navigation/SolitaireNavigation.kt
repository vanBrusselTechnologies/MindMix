package com.vanbrusselgames.mindmix.games.solitaire.navigation

import androidx.activity.compose.BackHandler
import androidx.compose.runtime.LaunchedEffect
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
import androidx.navigation.compose.navigation
import com.vanbrusselgames.mindmix.core.designsystem.theme.forceFullScreen
import com.vanbrusselgames.mindmix.core.logging.Logger
import com.vanbrusselgames.mindmix.core.model.SceneRegistry
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.games.solitaire.ui.GameUI
import com.vanbrusselgames.mindmix.games.solitaire.ui.dialogs.SolitaireGameFinishedDialog
import com.vanbrusselgames.mindmix.games.solitaire.ui.dialogs.SolitaireGameHelpDialog
import com.vanbrusselgames.mindmix.games.solitaire.ui.dialogs.SolitaireGameMenuDialog
import com.vanbrusselgames.mindmix.games.solitaire.ui.dialogs.SolitaireSettingsDialog
import com.vanbrusselgames.mindmix.games.solitaire.viewmodel.SolitaireViewModel
import kotlinx.serialization.Serializable

@Serializable
object SolitaireFeatureRoute

@Serializable
object/*data class*/ SolitaireGameRoute/*(val mode: Int = 0)*/

@Serializable
object SolitaireGameFinishedRoute

@Serializable
object SolitaireGameHelpRoute

@Serializable
object SolitaireGameMenuRoute

@Serializable
object SolitaireSettingsRoute

fun NavController.navigateToSolitaire(
    route: SolitaireGameRoute = SolitaireGameRoute, navOptions: NavOptions? = null
) {
    Logger.d("Navigate to: Solitaire")
    SceneManager.currentScene = SceneRegistry.Solitaire
    navigate(route, navOptions)
}

fun NavController.navigateToSolitaireGameFinished(
    route: SolitaireGameFinishedRoute = SolitaireGameFinishedRoute, navOptions: NavOptions? = null
) {
    Logger.d("Navigate to: Solitaire GameFinished")
    navigate(route, navOptions)
}

fun NavController.navigateToSolitaireGameHelp(
    route: SolitaireGameHelpRoute = SolitaireGameHelpRoute, navOptions: NavOptions? = null
) {
    Logger.d("Navigate to: Solitaire GameHelp")
    navigate(route, navOptions)
}

fun NavController.navigateToSolitaireGameMenu(
    route: SolitaireGameMenuRoute = SolitaireGameMenuRoute, navOptions: NavOptions? = null
) {
    Logger.d("Navigate to: Solitaire GameMenu")
    navigate(route, navOptions)
}

fun NavController.navigateToSolitaireSettings(
    route: SolitaireSettingsRoute = SolitaireSettingsRoute, navOptions: NavOptions? = null
) {
    Logger.d("Navigate to: Solitaire Settings")
    navigate(route, navOptions)
}

fun NavGraphBuilder.solitaire(navController: NavController) {
    navigation<SolitaireFeatureRoute>(SolitaireGameRoute) {
        composable<SolitaireGameRoute> { navBackStackEntry ->
            val vm = hiltViewModel<SolitaireViewModel>(remember(navBackStackEntry) {
                navController.getBackStackEntry<SolitaireFeatureRoute>()
            })
            // val route = navBackStackEntry.toRoute<SolitaireGameRoute>()
            // route.mode
            GameUI(vm, navController)
            BackHandler { navController.navigateToSolitaireGameMenu() }
        }

        dialog<SolitaireGameFinishedRoute>(
            dialogProperties = DialogProperties(false, false, false)
        ) { navBackStackEntry ->
            val vm = hiltViewModel<SolitaireViewModel>(remember(navBackStackEntry) {
                navController.getBackStackEntry<SolitaireFeatureRoute>()
            })
            val window = (LocalView.current.parent as? DialogWindowProvider)?.window
            if (window != null) forceFullScreen(window)
            SolitaireGameFinishedDialog(vm, navController)
        }

        dialog<SolitaireGameHelpRoute>(
            dialogProperties = DialogProperties(true, false, false)
        ) { navBackStackEntry ->
            val window = (LocalView.current.parent as? DialogWindowProvider)?.window
            if (window != null) forceFullScreen(window)
            SolitaireGameHelpDialog(navController)
        }

        dialog<SolitaireGameMenuRoute>(
            dialogProperties = DialogProperties(true, false, false)
        ) { navBackStackEntry ->
            val vm = hiltViewModel<SolitaireViewModel>(remember(navBackStackEntry) {
                navController.getBackStackEntry<SolitaireFeatureRoute>()
            })
            val window = (LocalView.current.parent as? DialogWindowProvider)?.window
            if (window != null) forceFullScreen(window)
            SolitaireGameMenuDialog(vm, navController)

            // Force stop timer and save data
            LaunchedEffect(Unit) { vm.onOpenDialog() }
        }

        dialog<SolitaireSettingsRoute>(
            dialogProperties = DialogProperties(true, false, false)
        ) { navBackStackEntry ->
            val vm = hiltViewModel<SolitaireViewModel>(remember(navBackStackEntry) {
                navController.getBackStackEntry<SolitaireFeatureRoute>()
            })
            val window = (LocalView.current.parent as? DialogWindowProvider)?.window
            if (window != null) forceFullScreen(window)
            SolitaireSettingsDialog(vm, navController)
        }
    }
}