package com.vanbrusselgames.mindmix.feature.menu.navigation

import androidx.compose.material3.SnackbarHostState
import androidx.navigation.NamedNavArgument
import androidx.navigation.NavController
import androidx.navigation.NavGraphBuilder
import androidx.navigation.NavOptions
import androidx.navigation.compose.composable
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.feature.menu.MenuScreenViewModel
import com.vanbrusselgames.mindmix.feature.menu.SceneUI
import kotlinx.serialization.Serializable

@Serializable
object MenuRoute {
    const val NAV_ROUTE = "menu"
    val NAV_ARGUMENTS = emptyList<NamedNavArgument>()
}

fun NavController.navigateToMenu(navOptions: NavOptions? = null) {
    SceneManager.currentScene = SceneManager.Scene.MENU
    navigate(route = MenuRoute) {
        navOptions
        popUpTo(MenuRoute) { inclusive = true }
    }
}

fun NavGraphBuilder.menu(
    navController: NavController,
    viewModel: MenuScreenViewModel,
    snackbarHostState: SnackbarHostState
) {
    composable<MenuRoute> {
        SceneUI(viewModel, navController, snackbarHostState)
    }
}