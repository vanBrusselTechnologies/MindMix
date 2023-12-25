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
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material.icons.filled.Settings
import androidx.compose.material.icons.filled.ShoppingCart
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.Scaffold
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalConfiguration
import androidx.compose.ui.unit.dp

abstract class BaseLayout {

    private val topRowButtonSize = 55f
    private val padding = topRowButtonSize / 100f
    private val topRowHeight = (topRowButtonSize + 2f * padding).dp
    protected var screenHeight = 0.dp
    protected var screenWidth = 0.dp

    abstract var uiHandler: BaseUIHandler

    @Composable
    open fun BaseScene(sceneSpecific: @Composable () -> Unit?, isMenu: Boolean = false) {
        val localCurrentConfig = LocalConfiguration.current
        screenHeight = localCurrentConfig.screenHeightDp.dp
        screenWidth = localCurrentConfig.screenWidthDp.dp
        Scaffold(topBar = { TopBar(isMenu) }, modifier = Modifier.safeDrawingPadding()) {
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
            IconButton(
                onClick = { uiHandler.backToMenu() },
                modifier = Modifier
                    .size(topRowButtonSize.dp)
                    .padding(padding.dp)
                    .align(Alignment.TopStart)
            ) {
                Icon(
                    imageVector = Icons.Default.ArrowBack,
                    contentDescription = "Back",
                    Modifier.fillMaxSize(0.9f)
                )
            }
            Row(Modifier.align(Alignment.TopEnd)) {
                if (isMenu) {
                    IconButton(
                        onClick = { uiHandler.openShop() },
                        modifier = Modifier
                            .size(topRowButtonSize.dp)
                            .padding(padding.dp)
                    ) {
                        Icon(
                            Icons.Filled.ShoppingCart, "Shop", Modifier.fillMaxSize(0.8f)
                        )
                    }
                }
                IconButton(
                    onClick = { uiHandler.openSettings() },
                    modifier = Modifier
                        .size(topRowButtonSize.dp)
                        .padding(padding.dp)
                ) {
                    Icon(
                        Icons.Filled.Settings, "Settings", Modifier.fillMaxSize(0.9f)
                    )
                }
            }
        }
    }
}