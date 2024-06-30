package com.vanbrusselgames.mindmix

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
import androidx.compose.material3.SnackbarHost
import androidx.compose.runtime.Composable
import androidx.compose.runtime.mutableStateOf
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.times
import com.vanbrusselgames.mindmix.menu.MenuManager

abstract class BaseLayout {
    companion object {
        val activeOverlapUI = mutableStateOf(false)
    }

    protected val blurStrength = 10.dp

    private val topRowButtonSize = 48.dp
    private val padding = topRowButtonSize / 100f
    private val topRowHeight = (topRowButtonSize + 2f * padding)

    @Composable
    fun BaseScene(isMenu: Boolean = false, sceneSpecific: @Composable () -> Unit?) {
        Scaffold(topBar = { TopBar(isMenu) }, snackbarHost = {
            SnackbarHost(hostState = MainActivity.snackbarHostState)
        }, modifier = Modifier.safeDrawingPadding()) {
            Box(
                Modifier
                    .fillMaxSize()
                    .padding(it)
            ) {
                sceneSpecific()
            }
        }
    }

    @Composable
    private fun TopBar(isMenu: Boolean) {
        Box(
            Modifier
                .height(topRowHeight)
                .fillMaxWidth()
        ) {
            if (!activeOverlapUI.value) {
                if (!isMenu) {
                    IconButton(
                        onClick = { BaseUIHandler.openGameMenu() },
                        modifier = Modifier
                            .size(topRowButtonSize)
                            .padding(padding)
                            .align(Alignment.TopStart)
                    ) {
                        Icon(
                            imageVector = Icons.Default.Menu,
                            contentDescription = "Open game menu",
                            Modifier.fillMaxSize(0.9f)
                        )
                    }
                    Row(
                        Modifier.align(Alignment.TopEnd)
                    ) {
                        IconButton(
                            onClick = { BaseUIHandler.openHelp() },
                            modifier = Modifier
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
                } else Row(
                    Modifier.align(Alignment.TopEnd)
                ) {
                    /*if (isMenu) {
                    IconButton(
                        onClick = { MenuUIHandler.openShop() },
                        modifier = Modifier
                            .size(topRowButtonSize)
                            .padding(padding)
                    ) {
                        Icon(
                            Icons.Filled.ShoppingCart, "Shop", Modifier.fillMaxSize(0.8f)
                        )
                    }
                }*/
                    IconButton(
                        onClick = {
                            MenuManager.settingsGame.value = SceneManager.Scene.MENU
                            BaseUIHandler.openSettings()
                        }, modifier = Modifier
                            .size(topRowButtonSize)
                            .padding(padding)
                    ) {
                        Icon(
                            Icons.Filled.Settings, "Settings", Modifier.fillMaxSize(0.9f),
                        )
                    }
                }
            }
        }
    }
}