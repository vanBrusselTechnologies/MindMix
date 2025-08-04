package com.vanbrusselgames.mindmix.feature.gamemenu.navigation

import androidx.compose.ui.platform.LocalView
import androidx.compose.ui.window.DialogProperties
import androidx.compose.ui.window.DialogWindowProvider
import androidx.navigation.NavController
import androidx.navigation.NavGraphBuilder
import androidx.navigation.NavOptions
import androidx.navigation.compose.dialog
import com.vanbrusselgames.mindmix.core.designsystem.theme.setFullScreen
import com.vanbrusselgames.mindmix.core.logging.Logger
import com.vanbrusselgames.mindmix.feature.gamemenu.GameMenuDialog
import kotlinx.serialization.Serializable

@Serializable
object GameMenuRoute

fun NavController.navigateToGameMenu(navOptions: NavOptions? = null) {
    Logger.d("Navigate to: GameMenu")
    navigate(GameMenuRoute, navOptions)
}

fun NavGraphBuilder.gameMenuDialog(
    navController: NavController,
    gameNameId: () -> Int,
    startNewGame: () -> Unit,
    openSettings: () -> Unit,
    backToMenu: () -> Unit
) {
    dialog<GameMenuRoute>(dialogProperties = DialogProperties(
        dismissOnBackPress = true,
        dismissOnClickOutside = false,
        usePlatformDefaultWidth = false
    )) {
        if(gameNameId() == -1) {
            navController.popBackStack()
            return@dialog
        }
        val window = (LocalView.current.parent as? DialogWindowProvider)?.window
        if (window != null) setFullScreen(null, window)
        GameMenuDialog(navController, gameNameId(), startNewGame, openSettings, backToMenu)
    }
}