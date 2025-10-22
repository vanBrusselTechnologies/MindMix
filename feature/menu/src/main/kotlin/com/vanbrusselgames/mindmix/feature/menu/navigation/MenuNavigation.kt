package com.vanbrusselgames.mindmix.feature.menu.navigation

import androidx.compose.animation.ExperimentalSharedTransitionApi
import androidx.compose.animation.SharedTransitionScope
import androidx.hilt.lifecycle.viewmodel.compose.hiltViewModel
import androidx.navigation.NavController
import androidx.navigation.NavGraphBuilder
import androidx.navigation.compose.composable
import com.vanbrusselgames.mindmix.core.navigation.AppRoutes
import com.vanbrusselgames.mindmix.feature.menu.ui.SceneUI
import com.vanbrusselgames.mindmix.feature.menu.viewmodel.MenuScreenViewModel

@OptIn(ExperimentalSharedTransitionApi::class)
fun NavGraphBuilder.menu(
    navController: NavController,
    sharedTransitionScope: SharedTransitionScope
) {
    composable<AppRoutes.Menu> {
        val vm = hiltViewModel<MenuScreenViewModel>()
        with(sharedTransitionScope) {
            SceneUI(vm, navController, this@composable)
        }
    }
}