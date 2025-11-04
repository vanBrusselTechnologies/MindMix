package com.vanbrusselgames.mindmix.feature.settings.navigation

import androidx.compose.ui.platform.LocalView
import androidx.compose.ui.window.DialogProperties
import androidx.compose.ui.window.DialogWindowProvider
import androidx.hilt.lifecycle.viewmodel.compose.hiltViewModel
import androidx.navigation.NavController
import androidx.navigation.NavGraphBuilder
import androidx.navigation.NavOptions
import androidx.navigation.compose.dialog
import com.vanbrusselgames.mindmix.core.designsystem.theme.forceFullScreen
import com.vanbrusselgames.mindmix.core.logging.Logger
import com.vanbrusselgames.mindmix.feature.settings.ui.MainSettings
import com.vanbrusselgames.mindmix.feature.settings.viewmodel.SettingsViewModel
import kotlinx.serialization.Serializable

@Serializable
object SettingsRoute

fun NavController.navigateToSettings(navOptions: NavOptions? = null) {
    Logger.d("Navigate to: Settings")
    navigate(SettingsRoute, navOptions)
}

fun NavGraphBuilder.settingsDialog(navController: NavController, onClickSignIn: () -> Unit) {
    dialog<SettingsRoute>(dialogProperties = DialogProperties(true, false, false)) {
        val window = (LocalView.current.parent as? DialogWindowProvider)?.window
        if (window != null) forceFullScreen(window)

        val vm = hiltViewModel<SettingsViewModel>()
        MainSettings(vm, navController, onClickSignIn)
    }
}