package com.vanbrusselgames.mindmix.feature.gamehelp.navigation

import androidx.compose.ui.platform.LocalView
import androidx.compose.ui.window.DialogProperties
import androidx.compose.ui.window.DialogWindowProvider
import androidx.navigation.NavController
import androidx.navigation.NavGraphBuilder
import androidx.navigation.NavOptions
import androidx.navigation.compose.dialog
import com.vanbrusselgames.mindmix.core.designsystem.theme.setFullScreen
import com.vanbrusselgames.mindmix.feature.gamehelp.GameHelpDialog
import kotlinx.serialization.Serializable

@Serializable
object GameHelpRoute

fun NavController.navigateToGameHelp(navOptions: NavOptions? = null) =
    navigate(route = GameHelpRoute, navOptions)

fun NavGraphBuilder.gameHelpDialog(
    navController: NavController, nameResId: () -> Int, descResId: () -> Int
) {
    dialog<GameHelpRoute>(dialogProperties = DialogProperties(true, false, false)) {
        val window = (LocalView.current.parent as? DialogWindowProvider)?.window
        if (window != null) setFullScreen(null, window)
        GameHelpDialog(navController, nameResId(), descResId())
    }
}