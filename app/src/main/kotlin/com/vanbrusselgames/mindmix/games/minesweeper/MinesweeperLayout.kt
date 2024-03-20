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
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.MutableState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.blur
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
import com.vanbrusselgames.mindmix.BaseLayout
import com.vanbrusselgames.mindmix.BaseUIHandler
import com.vanbrusselgames.mindmix.PixelHelper.Companion.pxToSp
import com.vanbrusselgames.mindmix.R
import com.vanbrusselgames.mindmix.games.GameHelp
import com.vanbrusselgames.mindmix.games.GameMenu
import com.vanbrusselgames.mindmix.games.minesweeper.MinesweeperManager.Instance.InputMode
import com.vanbrusselgames.mindmix.games.minesweeper.MinesweeperManager.Instance.findOtherSafeCells
import com.vanbrusselgames.mindmix.games.minesweeper.MinesweeperManager.Instance.minesweeperFinished
import com.vanbrusselgames.mindmix.games.minesweeper.MinesweeperManager.Instance.showAllMines
import kotlin.math.floor
import kotlin.math.max
import kotlin.math.min
import kotlin.math.roundToInt

class MinesweeperLayout : BaseLayout() {
    override var uiHandler: BaseUIHandler = MinesweeperUIHandler()
    private var cellSize: Dp = 0.dp

    @Composable
    fun Scene() {
        BaseScene {
            MinesweeperSpecificLayout(disableTopRowButtons.value)
            GameHelp.Screen(R.string.minesweeper_name, R.string.minesweeper_desc)
            GameMenu.Screen(R.string.minesweeper_name) { MinesweeperManager.startNewGame() }
            MinesweeperSettings()
            if (minesweeperFinished.value) GameFinishedPopUp(MinesweeperManager.cells.any { c -> c.state == MinesweeperCell.State.Bomb })
        }
    }

    @Composable
    fun MinesweeperSpecificLayout(isBlurred: Boolean) {
        BoxWithConstraints(
            Modifier
                .fillMaxSize()
                .blur(if (isBlurred) blurStrength else 0.dp),
        ) {
            val maxWidth = maxWidth
            val maxHeight = maxHeight
            if (maxWidth < maxHeight) {
                MinesweeperManager.sizeX = 16
                MinesweeperManager.sizeY = 22
                Column(
                    modifier = Modifier
                        .aspectRatio(0.55f)
                        .fillMaxSize()
                        .align(Alignment.Center),
                    horizontalAlignment = Alignment.CenterHorizontally
                ) {
                    MinesweeperGrid()
                    Spacer(
                        modifier = Modifier
                            .fillMaxHeight(0.1f)
                            .aspectRatio(1f)
                    )
                    Tools(true)
                }
            } else {
                MinesweeperManager.sizeX = 22
                MinesweeperManager.sizeY = 16
                Row(
                    modifier = Modifier
                        .aspectRatio(1f / 0.55f)
                        .fillMaxSize()
                        .align(Alignment.Center), verticalAlignment = Alignment.CenterVertically
                ) {
                    MinesweeperGrid()
                    Spacer(
                        modifier = Modifier
                            .fillMaxWidth(0.1f)
                            .aspectRatio(1f)
                    )
                    Tools(false)
                }
            }
        }
    }

    //#region Grid
    @Composable
    fun MinesweeperGrid() {
        Box(
            modifier = if (MinesweeperManager.sizeX < MinesweeperManager.sizeY) {
                Modifier.fillMaxWidth(0.95f)
            } else {
                Modifier.fillMaxHeight(0.95f)
            }
        ) {
            BoxWithConstraints(modifier = Modifier
                .align(Alignment.Center)
                .background(Color.Black)
                .pointerInput(Unit) {
                    detectTapGestures { offset ->
                        if (MinesweeperManager.finished) return@detectTapGestures
                        if (disableTopRowButtons.value) return@detectTapGestures
                        val column = floor(offset.x / cellSize.toPx())
                        val row = floor(offset.y / cellSize.toPx())
                        val cellIndex = if (MinesweeperManager.sizeX < MinesweeperManager.sizeY) {
                            (column + row * MinesweeperManager.sizeX).roundToInt()
                        } else {
                            (MinesweeperManager.sizeY * (column + 1) - (row + 1)).roundToInt()
                        }
                        onSelectCell(MinesweeperManager.cells[cellIndex])
                    }
                }) {

                val maxHeight = maxHeight.value
                val maxWidth = maxWidth.value

                cellSize = min(max(maxHeight, maxWidth) / 22f, min(maxHeight, maxWidth) / 16f).dp

                Column {
                    for (j in 0 until MinesweeperManager.sizeY) {
                        Row {
                            for (i in 0 until MinesweeperManager.sizeX) {
                                val cellIndex =
                                    if (MinesweeperManager.sizeX < MinesweeperManager.sizeY) {
                                        j * MinesweeperManager.sizeX + i
                                    } else {
                                        MinesweeperManager.sizeY * (i + 1) - (j + 1)
                                    }
                                MinesweeperCell(MinesweeperManager.cells[cellIndex])
                            }
                        }
                    }
                }
            }
        }
    }

    private fun onSelectCell(cell: MinesweeperCell) {
        cell.pressed = true
        when (cell.state) {
            MinesweeperCell.State.Empty -> {
                if (MinesweeperManager.inputMode == InputMode.Flag) {
                    cell.state = MinesweeperCell.State.Flag
                    MinesweeperManager.calculateMinesLeft()
                } else {
                    if (cell.isMine) return showAllMines()
                    cell.state = MinesweeperCell.State.Number
                    if (cell.mineCount == 0) {
                        val tempCheckList = mutableListOf(-1)
                        findOtherSafeCells(cell, tempCheckList)
                        tempCheckList.sort()
                    }
                    MinesweeperManager.checkFinished()
                }
            }

            MinesweeperCell.State.Flag -> {
                if (MinesweeperManager.inputMode == InputMode.Flag) {
                    cell.state = MinesweeperCell.State.Empty
                    MinesweeperManager.calculateMinesLeft()
                }
            }

            else -> {}
        }
    }

    @Composable
    fun MinesweeperCell(cell: MinesweeperCell) {
        Box(
            contentAlignment = Alignment.Center,
            modifier = Modifier
                .size(cellSize)
                .padding(PaddingValues(0.75f.dp))
                .background(if (cell.background.value == Color.Red) MaterialTheme.colorScheme.errorContainer else MaterialTheme.colorScheme.secondaryContainer)
                .aspectRatio(1f)
        ) {
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
                color = MaterialTheme.colorScheme.onSecondaryContainer,
                textAlign = TextAlign.Center,
                fontSize = pxToSp(minOf(constraints.maxHeight, constraints.minHeight)) / 2f,
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
        Icon(painterResource(R.drawable.baseline_flag_24), "Flag", Modifier.fillMaxSize())
    }

    @Composable
    fun MinesweeperMineCell() {
        Icon(painterResource(R.drawable.outline_bomb_24), "Bomb", Modifier.fillMaxSize())
    }
    //#endregion

    //#region Tools
    @Composable
    fun Tools(isHorizontal: Boolean) {
        val colors = ButtonColors(
            MaterialTheme.colorScheme.secondaryContainer,
            MaterialTheme.colorScheme.onSecondaryContainer,
            MaterialTheme.colorScheme.secondaryContainer,
            MaterialTheme.colorScheme.onSecondaryContainer
        )
        if (isHorizontal) {
            Row(Modifier.fillMaxSize(), verticalAlignment = Alignment.CenterVertically) {
                Spacer(modifier = Modifier.weight(1f))
                Box(modifier = Modifier.weight(2f)) {
                    Text(
                        text = "${stringResource(R.string.mines_left)}\n${MinesweeperManager.minesLeft.intValue}",
                        modifier = Modifier.align(Alignment.Center),
                        textAlign = TextAlign.Center,
                        fontSize = 22.5.sp
                    )
                }
                val inputMode = remember { mutableStateOf(MinesweeperManager.inputMode) }

                Button(
                    onClick = { changeInputMode(inputMode) },
                    Modifier
                        .weight(2f)
                        .aspectRatio(1f),
                    enabled = !disableTopRowButtons.value,
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
                Spacer(modifier = Modifier.weight(1f))
            }
        } else {
            Column(Modifier.fillMaxSize(), horizontalAlignment = Alignment.CenterHorizontally) {
                Spacer(modifier = Modifier.weight(1f))
                Box(modifier = Modifier.weight(2f)) {
                    Text(
                        text = "${stringResource(R.string.mines_left)}\n${MinesweeperManager.minesLeft.intValue}",
                        modifier = Modifier.align(Alignment.Center),
                        textAlign = TextAlign.Center,
                        fontSize = 22.5.sp
                    )
                }
                val inputMode = remember { mutableStateOf(MinesweeperManager.inputMode) }

                Button(
                    onClick = { changeInputMode(inputMode) },
                    Modifier
                        .weight(2f)
                        .aspectRatio(1f),
                    enabled = !minesweeperFinished.value,
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
                Spacer(modifier = Modifier.weight(1f))
            }
        }
    }

    private fun changeInputMode(inputMode: MutableState<InputMode>) {
        inputMode.value = MinesweeperManager.changeInputMode()
    }
    //#endregion

    @Composable
    fun GameFinishedPopUp(failed: Boolean) {
        val title = stringResource(R.string.minesweeper_name)
        //if (failed) "Failed" else "Congrats / Smart / Well done"
        val desc =
            if (failed) stringResource(R.string.minesweeper_failed) else stringResource(R.string.minesweeper_success)
        //if (failed) "A mine exploded" else """You did great and solved puzzle in ${0} seconds!!
        //     |That's Awesome!
        //     |Share with your friends and challenge them to beat your time!""".trimMargin()
        val reward = if (failed) 0 else 10
        //val onClickShare = if (failed) null else ({})
        val onClickPlayAgain = {
            MinesweeperManager.startNewGame()
        }
        val onClickReturnToMenu = {
            MinesweeperManager.startNewGame()
        }
        BaseGameFinishedPopUp(
            title, desc, reward, null, onClickPlayAgain, onClickReturnToMenu
        )
    }
}