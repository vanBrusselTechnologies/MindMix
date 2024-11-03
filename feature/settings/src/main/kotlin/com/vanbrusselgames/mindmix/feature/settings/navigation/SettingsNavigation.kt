package com.vanbrusselgames.mindmix.feature.settings.navigation

import androidx.compose.runtime.Composable
import androidx.compose.ui.platform.LocalView
import androidx.compose.ui.window.DialogProperties
import androidx.compose.ui.window.DialogWindowProvider
import androidx.navigation.NavController
import androidx.navigation.NavGraphBuilder
import androidx.navigation.NavOptions
import androidx.navigation.compose.dialog
import com.vanbrusselgames.mindmix.core.designsystem.theme.setFullScreen
import com.vanbrusselgames.mindmix.feature.settings.SettingsDialog
import kotlinx.serialization.Serializable

@Serializable
object SettingsRoute

fun NavController.navigateToSettings(navOptions: NavOptions? = null) =
    navigate(route = SettingsRoute, navOptions)

fun NavGraphBuilder.settingsDialog(
    navController: NavController,
    content: @Composable () -> Unit
) {
    dialog<SettingsRoute>(dialogProperties = DialogProperties(true, false, false)) {
        val window = (LocalView.current.parent as? DialogWindowProvider)?.window
        if (window != null) setFullScreen(null, window)
        SettingsDialog(navController, content)
    }
}