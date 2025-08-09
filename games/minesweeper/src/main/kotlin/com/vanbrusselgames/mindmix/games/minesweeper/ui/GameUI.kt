package com.vanbrusselgames.mindmix.games.minesweeper.ui

import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.gestures.detectTapGestures
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.BoxScope
import androidx.compose.foundation.layout.BoxWithConstraints
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.PaddingValues
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.text.BasicText
import androidx.compose.foundation.text.TextAutoSize
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonColors
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.MutableIntState
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.blur
import androidx.compose.ui.draw.drawBehind
import androidx.compose.ui.draw.scale
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.input.pointer.pointerInput
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.AnnotatedString
import androidx.compose.ui.text.TextStyle
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.LineHeightStyle
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.common.BaseScene
import com.vanbrusselgames.mindmix.core.common.GameLoadingScreen
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.games.minesweeper.R
import com.vanbrusselgames.mindmix.games.minesweeper.model.CellState
import com.vanbrusselgames.mindmix.games.minesweeper.model.InputMode
import com.vanbrusselgames.mindmix.games.minesweeper.model.MinesweeperCell
import com.vanbrusselgames.mindmix.games.minesweeper.navigation.navigateToMinesweeperGameMenu
import com.vanbrusselgames.mindmix.games.minesweeper.navigation.navigateToMinesweeperSettings
import com.vanbrusselgames.mindmix.games.minesweeper.viewmodel.IMinesweeperViewModel
import kotlin.math.max
import kotlin.math.min

private var cellSize: Dp = 0.dp

@Composable
fun GameUI(viewModel: IMinesweeperViewModel, navController: NavController) {
    BaseScene(
        viewModel,
        navController,
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

//#region Grid
@Composable
fun MinesweeperGrid(viewModel: IMinesweeperViewModel, navController: NavController) {
    Box(
        if (viewModel.sizeX < viewModel.sizeY) {
            Modifier.fillMaxWidth(0.95f)
        } else {
            Modifier.fillMaxHeight(0.95f)
        }
    ) {
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
            cellSize = min(max(maxHeight, maxWidth) / 22f, min(maxHeight, maxWidth) / 16f).dp
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
                            MinesweeperCell(viewModel.cells[cellIndex])
                        }
                    }
                }
            }
        }
    }
}

@Composable
fun MinesweeperCell(cell: MinesweeperCell) {
    val cs = MaterialTheme.colorScheme
    Box(
        contentAlignment = Alignment.Center,
        modifier = Modifier
            .size(cellSize)
            .padding(PaddingValues(0.75f.dp))
            .drawBehind {
                drawRect(if (cell.background.value == Color.Red) cs.errorContainer else cs.secondaryContainer)
            }
            .aspectRatio(1f)) {
        MinesweeperCellContent(cell)
    }
}

@Composable
fun MinesweeperCellContent(cell: MinesweeperCell) {
    when (cell.mutableCellState.value) {
        CellState.Empty -> return
        CellState.Bomb -> {
            if (cell.pressed) cell.background.value = Color.Red
            MinesweeperMineCell()
        }

        CellState.Flag -> MinesweeperFlagCell()
        CellState.Number -> MinesweeperTextCell(cell.mineCount)
    }
}

@Composable
private fun MinesweeperTextCell(value: Int) {
    BasicText(
        text = AnnotatedString(if (value == 0) "-" else value.toString()),
        modifier = Modifier.scale(1.125f),
        style = TextStyle(
            color = MaterialTheme.colorScheme.onSecondaryContainer,
            fontWeight = FontWeight.Bold,
            fontFeatureSettings = "tnum",
            textAlign = TextAlign.Center,
            lineHeightStyle = LineHeightStyle(
                alignment = LineHeightStyle.Alignment.Center, trim = LineHeightStyle.Trim.Both
            )
        ),
        maxLines = 1,
        autoSize = TextAutoSize.StepBased(maxFontSize = 250.sp, minFontSize = 5.sp)
    )
}

@Composable
fun MinesweeperFlagCell() {
    Icon(painterResource(R.drawable.baseline_flag_24), "Flag", Modifier.fillMaxSize())
}

@Composable
fun MinesweeperMineCell() {
    Icon(painterResource(R.drawable.outline_bomb_24), "Bomb", Modifier.fillMaxSize())
}
//#endregion

//#region Tools
@Composable
fun Tools(viewModel: IMinesweeperViewModel, isHorizontal: Boolean) {
    if (isHorizontal) {
        Row(Modifier.fillMaxSize(), verticalAlignment = Alignment.CenterVertically) {
            ToolsContent(viewModel, Modifier.weight(2f), Modifier.weight(1f))
        }
    } else {
        Column(Modifier.fillMaxSize(), horizontalAlignment = Alignment.CenterHorizontally) {
            ToolsContent(viewModel, Modifier.weight(2f), Modifier.weight(1f))
        }
    }
}

@Composable
private fun ToolsContent(
    viewModel: IMinesweeperViewModel, modifier: Modifier, spacerModifier: Modifier
) {
    Spacer(spacerModifier)
    MinesLeftText(modifier, viewModel.minesLeft)
    ChangeInputModeButton(viewModel, modifier)
    Spacer(spacerModifier)
}

@Composable
private fun MinesLeftText(modifier: Modifier, minesLeft: MutableIntState) {
    Text(
        text = "${stringResource(R.string.mines_left)}\n${minesLeft.intValue}",
        modifier = modifier,
        textAlign = TextAlign.Center,
        fontSize = 22.5.sp
    )
}

@Composable
private fun ChangeInputModeButton(viewModel: IMinesweeperViewModel, modifier: Modifier) {
    val colors = ButtonColors(
        MaterialTheme.colorScheme.secondaryContainer,
        MaterialTheme.colorScheme.onSecondaryContainer,
        MaterialTheme.colorScheme.secondaryContainer,
        MaterialTheme.colorScheme.onSecondaryContainer
    )
    Button(
        { viewModel.changeInputMode() },
        modifier.aspectRatio(1f),
        shape = RoundedCornerShape(10.dp),
        colors = colors
    ) {
        if (viewModel.inputMode.value == InputMode.Flag) {
            Icon(
                painterResource(R.drawable.baseline_flag_24),
                "inputType: Flag",
                Modifier.fillMaxSize()
            )
        } else Image(painterResource(R.drawable.spade), "inputType: Spade")
    }
}
//#endregion