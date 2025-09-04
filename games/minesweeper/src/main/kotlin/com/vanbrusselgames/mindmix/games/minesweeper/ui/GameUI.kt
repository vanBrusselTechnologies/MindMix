package com.vanbrusselgames.mindmix.games.minesweeper.ui

import androidx.compose.foundation.background
import androidx.compose.foundation.gestures.detectTapGestures
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.BoxScope
import androidx.compose.foundation.layout.BoxWithConstraints
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.runtime.Composable
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.blur
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.input.pointer.pointerInput
import androidx.compose.ui.tooling.preview.PreviewScreenSizes
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.dp
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import androidx.navigation.NavController
import androidx.navigation.compose.rememberNavController
import com.vanbrusselgames.mindmix.core.common.BaseScene
import com.vanbrusselgames.mindmix.core.common.GameLoadingScreen
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.games.minesweeper.navigation.navigateToMinesweeperGameHelp
import com.vanbrusselgames.mindmix.games.minesweeper.navigation.navigateToMinesweeperGameMenu
import com.vanbrusselgames.mindmix.games.minesweeper.navigation.navigateToMinesweeperSettings
import com.vanbrusselgames.mindmix.games.minesweeper.viewmodel.IMinesweeperViewModel
import com.vanbrusselgames.mindmix.games.minesweeper.viewmodel.MockMinesweeperViewModel
import kotlin.math.max
import kotlin.math.min

@Composable
fun GameUI(viewModel: IMinesweeperViewModel, navController: NavController) {
    BaseScene(
        viewModel,
        { navController.navigateToMinesweeperGameHelp() },
        { navController.navigateToMinesweeperGameMenu() },
        { navController.navigateToMinesweeperSettings() }) {
        BlurBox(viewModel.blurStrength) { maxWidth, maxHeight ->
            MinesweeperSpecificLayout(maxWidth, maxHeight, viewModel) { horizontal ->
                SceneContent(viewModel, navController, horizontal)
            }
        }
    }

    val loadedState = viewModel.puzzleLoaded.collectAsStateWithLifecycle()
    if (!loadedState.value) GameLoadingScreen()
}

@Composable
fun BlurBox(blurStrength: Dp, content: @Composable (maxWidth: Dp, maxHeight: Dp) -> Unit) {
    BoxWithConstraints(
        Modifier
            .fillMaxSize()
            .blur(if (SceneManager.dialogActiveState.value) blurStrength else 0.dp),
    ) {
        content(this.maxWidth, this.maxHeight)
    }
}

@Composable
fun BoxScope.MinesweeperSpecificLayout(
    maxWidth: Dp,
    maxHeight: Dp,
    viewModel: IMinesweeperViewModel,
    content: @Composable (horizontal: Boolean) -> Unit
) {
    if (maxWidth < maxHeight) {
        viewModel.sizeX = 16
        viewModel.sizeY = 22
        Column(
            Modifier
                .aspectRatio(0.55f)
                .fillMaxSize()
                .align(Alignment.Center),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            content(true)
        }
    } else {
        viewModel.sizeX = 22
        viewModel.sizeY = 16
        Row(
            Modifier
                .aspectRatio(1f / 0.55f)
                .fillMaxSize()
                .align(Alignment.Center),
            verticalAlignment = Alignment.CenterVertically
        ) {
            content(false)
        }
    }
}

@Composable
fun SceneContent(
    viewModel: IMinesweeperViewModel, navController: NavController, horizontal: Boolean
) {
    MinesweeperGrid(viewModel, navController)
    val modifier = if (horizontal) Modifier.fillMaxHeight(0.1f) else Modifier.fillMaxWidth(0.1f)
    Spacer(modifier.aspectRatio(1f))
    Tools(viewModel, horizontal)
}

@Composable
fun MinesweeperGrid(viewModel: IMinesweeperViewModel, navController: NavController) {
    Box(
        if (viewModel.sizeX < viewModel.sizeY) {
            Modifier.fillMaxWidth(0.95f)
        } else {
            Modifier.fillMaxHeight(0.95f)
        }, contentAlignment = Alignment.Center
    ) {
        var cellSize: Dp = 0.dp
        BoxWithConstraints(
            Modifier
                .align(Alignment.Center)
                .background(Color.Black)
                .pointerInput(Unit) {
                    detectTapGestures { offset ->
                        viewModel.onSelectCell(offset, cellSize.toPx(), navController)
                    }
                }) {
            val maxHeight = this.maxHeight.value
            val maxWidth = this.maxWidth.value
            cellSize = remember(maxWidth, maxHeight) {
                min(max(maxHeight, maxWidth) / 22f, min(maxHeight, maxWidth) / 16f).dp
            }
            Column {
                repeat(viewModel.sizeY) { j ->
                    Row {
                        repeat(viewModel.sizeX) { i ->
                            val cellIndex = remember {
                                if (viewModel.sizeX < viewModel.sizeY) {
                                    j * viewModel.sizeX + i
                                } else {
                                    viewModel.sizeY * (i + 1) - (j + 1)
                                }
                            }
                            MinesweeperCell(viewModel.cells[cellIndex], cellSize)
                        }
                    }
                }
            }
        }
    }
}

@PreviewScreenSizes
@Composable
private fun MinesweeperGridPreview() {
    val vm = remember { MockMinesweeperViewModel() }
    GameUI(vm, rememberNavController())
}