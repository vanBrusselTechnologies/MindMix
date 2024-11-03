package com.vanbrusselgames.mindmix.core.common

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.safeDrawingPadding
import androidx.compose.foundation.layout.size
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Menu
import androidx.compose.material.icons.filled.Settings
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.Scaffold
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.painterResource
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.feature.gamehelp.navigation.navigateToGameHelp
import com.vanbrusselgames.mindmix.feature.gamemenu.navigation.navigateToGameMenu
import com.vanbrusselgames.mindmix.feature.settings.navigation.navigateToSettings

@Composable
fun BaseScene(
    viewModel: BaseScreenViewModel,
    navController: NavController,
    sceneSpecific: @Composable () -> Unit?
) {
    Scaffold(Modifier.safeDrawingPadding(), { TopBar(viewModel, navController) }, snackbarHost = {
        //todo: SnackbarHost(hostState = MainActivity.snackbarHostState)
    }) {
        Box(
            Modifier
                .fillMaxSize()
                .padding(it)
        ) { sceneSpecific() }
    }
}

@Composable
private fun TopBar(viewModel: BaseScreenViewModel, navController: NavController) {
    Box(
        Modifier
            .height(viewModel.topRowHeight)
            .fillMaxWidth()
    ) {
        if (!SceneManager.dialogActiveState.value) {
            if (!viewModel.isMenu) {
                IconButton(
                    onClick = {
                        navController.navigateToGameMenu()
                        viewModel.onOpenDialog()
                    },
                    modifier = Modifier
                        .size(viewModel.topRowButtonSize)
                        .padding(viewModel.padding)
                        .align(Alignment.TopStart)
                ) { Icon(Icons.Default.Menu, "Open game menu", Modifier.fillMaxSize(0.9f)) }
                Row(Modifier.align(Alignment.TopEnd)) {
                    IconButton(
                        onClick = {
                            navController.navigateToGameHelp()
                            viewModel.onOpenDialog()
                        },
                        modifier = Modifier
                            .size(viewModel.topRowButtonSize)
                            .padding(viewModel.padding)
                    ) {
                        Icon(
                            painterResource(R.drawable.outline_help_24),
                            "Help",
                            Modifier.fillMaxSize(0.9f),
                        )
                    }
                }
            } else Row(Modifier.align(Alignment.TopEnd)) {
                /* todo: Add Shop
                    if (isMenu) {
                        IconButton(
                            onClick = {
                                navController.navigateToShop()
                                viewModel.onOpenDialog()
                            },
                            modifier = Modifier
                                .size(viewModel.topRowButtonSize)
                                .padding(viewModel.padding)
                        ) { Icon(Icons.Filled.ShoppingCart, "Shop", Modifier.fillMaxSize(0.9f)) }
                    }
                 */
                IconButton(
                    onClick = {
                        navController.navigateToSettings()
                        viewModel.onOpenDialog()
                    },
                    modifier = Modifier
                        .size(viewModel.topRowButtonSize)
                        .padding(viewModel.padding)
                ) { Icon(Icons.Filled.Settings, "Settings", Modifier.fillMaxSize(0.9f)) }
            }
        }
    }
}