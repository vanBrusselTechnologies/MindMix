package com.vanbrusselgames.mindmix.games.minesweeper

import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.gestures.detectTapGestures
import androidx.compose.foundation.layout.Box
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
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonColors
import androidx.compose.material3.Icon
import androidx.compose.material3.LocalTextStyle
import androidx.compose.material3.MaterialTheme.colorScheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.blur
import androidx.compose.ui.draw.drawBehind
import androidx.compose.ui.draw.scale
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.input.pointer.pointerInput
import androidx.compose.ui.platform.LocalContext
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
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.common.BaseScene
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.core.utils.PixelHelper.Companion.pxToSp
import com.vanbrusselgames.mindmix.games.minesweeper.GameViewModel.InputMode
import kotlin.math.floor
import kotlin.math.max
import kotlin.math.min
import kotlin.math.roundToInt

private var cellSize: Dp = 0.dp

@Composable
fun GameUI(viewModel: GameViewModel, navController: NavController) {
    BaseScene(viewModel, navController) {
        MinesweeperSpecificLayout(viewModel, navController)
    }
}

@Composable
fun MinesweeperSpecificLayout(viewModel: GameViewModel, navController: NavController) {
    BoxWithConstraints(
        Modifier
            .fillMaxSize()
            .blur(if (SceneManager.dialogActiveState.value) viewModel.blurStrength else 0.dp),
    ) {
        val maxWidth = this.maxWidth
        val maxHeight = this.maxHeight
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
                MinesweeperGrid(viewModel, navController)
                Spacer(
                    Modifier
                        .fillMaxHeight(0.1f)
                        .aspectRatio(1f)
                )
                Tools(viewModel, true)
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
                MinesweeperGrid(viewModel, navController)
                Spacer(
                    Modifier
                        .fillMaxWidth(0.1f)
                        .aspectRatio(1f)
                )
                Tools(viewModel, false)
            }
        }
    }
}

//#region Grid
@Composable
fun MinesweeperGrid(viewModel: GameViewModel, navController: NavController) {
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
                        if (viewModel.finished) return@detectTapGestures
                        if (SceneManager.dialogActiveState.value) return@detectTapGestures
                        val column = floor(offset.x / cellSize.toPx())
                        val row = floor(offset.y / cellSize.toPx())
                        val cellIndex = if (viewModel.sizeX < viewModel.sizeY) {
                            (column + row * viewModel.sizeX).roundToInt()
                        } else {
                            (viewModel.sizeY * (column + 1) - (row + 1)).roundToInt()
                        }
                        onSelectCell(viewModel, viewModel.cells[cellIndex], navController)
                    }
                }) {
            val maxHeight = this.maxHeight.value
            val maxWidth = this.maxWidth.value
            cellSize = min(max(maxHeight, maxWidth) / 22f, min(maxHeight, maxWidth) / 16f).dp
            Column {
                for (j in 0 until viewModel.sizeY) {
                    Row {
                        for (i in 0 until viewModel.sizeX) {
                            val cellIndex = if (viewModel.sizeX < viewModel.sizeY) {
                                j * viewModel.sizeX + i
                            } else {
                                viewModel.sizeY * (i + 1) - (j + 1)
                            }
                            MinesweeperCell(viewModel.cells[cellIndex])
                        }
                    }
                }
            }
        }
    }
}

private fun onSelectCell(
    viewModel: GameViewModel, cell: MinesweeperCell, navController: NavController
) {
    cell.pressed = true
    when (cell.state) {
        MinesweeperCell.State.Empty -> {
            if (viewModel.inputMode == InputMode.Flag) {
                cell.state = MinesweeperCell.State.Flag
                viewModel.calculateMinesLeft()
            } else {
                if (cell.isMine) return viewModel.showAllMines(navController)
                cell.state = MinesweeperCell.State.Number
                if (cell.mineCount == 0) {
                    viewModel.findOtherSafeCells(cell)
                }
                if (viewModel.autoFlag) viewModel.autoFlag()
                viewModel.checkFinished(navController)
            }
        }

        MinesweeperCell.State.Flag -> {
            if (viewModel.inputMode == InputMode.Flag) {
                cell.state = MinesweeperCell.State.Empty
                viewModel.calculateMinesLeft()
            }
        }

        else -> {}
    }
}

@Composable
fun MinesweeperCell(cell: MinesweeperCell) {
    val cs = colorScheme
    Box(contentAlignment = Alignment.Center,
        modifier = Modifier
            .size(cellSize)
            .padding(PaddingValues(0.75f.dp))
            .drawBehind {
                drawRect(if (cell.background.value == Color.Red) cs.errorContainer else cs.secondaryContainer)
            }
            .aspectRatio(1f)) {
        val state by remember { cell.mutableCellState }
        when (state) {
            MinesweeperCell.State.Empty -> return@Box
            MinesweeperCell.State.Bomb -> {
                if (cell.pressed) cell.background.value = Color.Red
                MinesweeperMineCell()
            }

            MinesweeperCell.State.Flag -> MinesweeperFlagCell()
            MinesweeperCell.State.Number -> MinesweeperTextCell(cell.mineCount)
        }
    }
}

@Composable
fun MinesweeperTextCell(value: Int) {
    BoxWithConstraints(Modifier.fillMaxSize()) {
        Text(
            text = AnnotatedString(if (value == 0) "-" else value.toString()),
            color = colorScheme.onSecondaryContainer,
            textAlign = TextAlign.Center,
            fontSize = pxToSp(
                LocalContext.current.resources,
                minOf(this.constraints.maxHeight, this.constraints.minHeight)
            ) / 2f,
            fontWeight = FontWeight.Bold,
            style = LocalTextStyle.current.merge(
                TextStyle(
                    lineHeightStyle = LineHeightStyle(
                        alignment = LineHeightStyle.Alignment.Center,
                        trim = LineHeightStyle.Trim.Both
                    ), fontFeatureSettings = "tnum"
                ),
            ),
            modifier = Modifier
                .align(Alignment.Center)
                .scale(2f)
        )
    }
}

@Composable
fun MinesweeperFlagCell() {
    Icon(
        painterResource(R.drawable.baseline_flag_24), "Flag", Modifier.fillMaxSize()
    )
}

@Composable
fun MinesweeperMineCell() {
    Icon(
        painterResource(R.drawable.outline_bomb_24), "Bomb", Modifier.fillMaxSize()
    )
}
//#endregion

//#region Tools
@Composable
fun Tools(viewModel: GameViewModel, isHorizontal: Boolean) {
    if (isHorizontal) {
        Row(Modifier.fillMaxSize(), verticalAlignment = Alignment.CenterVertically) {
            ToolsContent(viewModel, Modifier.weight(1f), Modifier.weight(2f))
        }
    } else {
        Column(Modifier.fillMaxSize(), horizontalAlignment = Alignment.CenterHorizontally) {
            ToolsContent(viewModel, Modifier.weight(1f), Modifier.weight(2f))
        }
    }
}

@Composable
private fun ToolsContent(
    viewModel: GameViewModel, modifierWeight1f: Modifier, modifierWeight2f: Modifier
) {
    val colors = ButtonColors(
        colorScheme.secondaryContainer,
        colorScheme.onSecondaryContainer,
        colorScheme.secondaryContainer,
        colorScheme.onSecondaryContainer
    )
    Spacer(modifierWeight1f)
    Box(modifierWeight2f) {
        Text(
            text = "${stringResource(R.string.mines_left)}\n${viewModel.minesLeft.intValue}",
            modifier = Modifier.align(Alignment.Center),
            textAlign = TextAlign.Center,
            fontSize = 22.5.sp
        )
    }
    val inputMode = remember { mutableStateOf(viewModel.inputMode) }
    Button(
        { viewModel.changeInputMode(inputMode) },
        modifierWeight2f.aspectRatio(1f),
        enabled = !SceneManager.dialogActiveState.value,
        RoundedCornerShape(10.dp),
        colors
    ) {
        if (inputMode.value == InputMode.Flag) {
            Icon(
                painterResource(R.drawable.baseline_flag_24),
                "inputType: Flag",
                Modifier.fillMaxSize()
            )
        } else Image(painterResource(R.drawable.spade), "inputType: Spade")
    }
    Spacer(modifierWeight1f)
}
//#endregion