package com.vanbrusselgames.mindmix.feature.gamefinished.navigation

import androidx.compose.runtime.Composable
import androidx.compose.ui.platform.LocalView
import androidx.compose.ui.window.DialogProperties
import androidx.compose.ui.window.DialogWindowProvider
import androidx.navigation.NavController
import androidx.navigation.NavGraphBuilder
import androidx.navigation.NavOptions
import androidx.navigation.compose.dialog
import com.vanbrusselgames.mindmix.core.designsystem.theme.setFullScreen
import com.vanbrusselgames.mindmix.core.logging.Logger
import com.vanbrusselgames.mindmix.feature.gamefinished.GameFinishedDialog
import kotlinx.serialization.Serializable

@Serializable
object GameFinishedRoute

fun NavController.navigateToGameFinished(navOptions: NavOptions? = null) {
    Logger.d("Navigate to: GameFinished")
    navigate(GameFinishedRoute, navOptions)
}

fun NavGraphBuilder.gameFinishedDialog(content: @Composable () -> Unit) {
    dialog<GameFinishedRoute>(dialogProperties = DialogProperties(false, false, false)) {
        val window = (LocalView.current.parent as? DialogWindowProvider)?.window
        if (window != null) setFullScreen(null, window)
        GameFinishedDialog(content)
    }
}