package com.vanbrusselgames.mindmix.feature.menu.navigation

import androidx.navigation.NavController
import androidx.navigation.NavGraphBuilder
import androidx.navigation.compose.composable
import com.vanbrusselgames.mindmix.core.common.BaseScreenViewModel
import com.vanbrusselgames.mindmix.core.navigation.AppRoutes
import com.vanbrusselgames.mindmix.feature.menu.MenuScreenViewModel
import com.vanbrusselgames.mindmix.feature.menu.SceneUI

fun NavGraphBuilder.menu(
    navController: NavController,
    viewModel: MenuScreenViewModel,
    setCurrentViewModel: (BaseScreenViewModel?) -> Unit
) {
    composable<AppRoutes.Menu> {
        val vm = viewModel
        setCurrentViewModel(vm)
        SceneUI(vm, navController)
    }
}