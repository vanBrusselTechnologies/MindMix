package com.vanbrusselgames.mindmix.feature.menu.navigation

import androidx.hilt.lifecycle.viewmodel.compose.hiltViewModel
import androidx.navigation.NavController
import androidx.navigation.NavGraphBuilder
import androidx.navigation.compose.composable
import com.vanbrusselgames.mindmix.core.navigation.AppRoutes
import com.vanbrusselgames.mindmix.feature.menu.ui.SceneUI
import com.vanbrusselgames.mindmix.feature.menu.viewmodel.MenuScreenViewModel

fun NavGraphBuilder.menu(navController: NavController) {
    composable<AppRoutes.Menu> {
        val vm = hiltViewModel<MenuScreenViewModel>()
        SceneUI(vm, navController)
    }
}