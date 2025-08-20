package com.vanbrusselgames.mindmix.core.common

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.BoxScope
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
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.times
import com.vanbrusselgames.mindmix.core.navigation.SceneManager

val topRowButtonSize = 48.dp
val padding = topRowButtonSize / 100f
val topRowHeight = (topRowButtonSize + 2f * padding)

@Composable
fun BaseScene(
    viewModel: IBaseScreenViewModel,
    openGameHelp: () -> Unit,
    openGameMenu: () -> Unit,
    openSettings: () -> Unit,
    sceneSpecific: @Composable BoxScope.() -> Unit
) {
    Scaffold(
        Modifier.safeDrawingPadding(),
        { TopBar(viewModel, openGameHelp, openGameMenu, openSettings) }) {
        Box(
            Modifier
                .fillMaxSize()
                .padding(it)
        ) { sceneSpecific() }
    }
}

@Composable
private fun TopBar(
    viewModel: IBaseScreenViewModel,
    openGameHelp: () -> Unit,
    openGameMenu: () -> Unit,
    openSettings: () -> Unit
) {
    Box(
        Modifier
            .height(topRowHeight)
            .fillMaxWidth()
    ) {
        if (!SceneManager.dialogActiveState.value) {
            if (!viewModel.isMenu) {
                IconButton(
                    onClick = {
                        openGameMenu()
                        viewModel.onOpenDialog()
                    },
                    modifier = Modifier
                        .size(topRowButtonSize)
                        .padding(padding)
                        .align(Alignment.TopStart)
                ) { Icon(Icons.Default.Menu, "Open game menu", Modifier.fillMaxSize(0.9f)) }
                Row(Modifier.align(Alignment.TopEnd)) {
                    IconButton(
                        onClick = {
                            openGameHelp()
                            viewModel.onOpenDialog()
                        }, modifier = Modifier
                            .size(topRowButtonSize)
                            .padding(padding)
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
                        openSettings()
                        viewModel.onOpenDialog()
                    }, modifier = Modifier
                        .size(topRowButtonSize)
                        .padding(padding)
                ) { Icon(Icons.Filled.Settings, "Settings", Modifier.fillMaxSize(0.9f)) }
            }
        }
    }
}