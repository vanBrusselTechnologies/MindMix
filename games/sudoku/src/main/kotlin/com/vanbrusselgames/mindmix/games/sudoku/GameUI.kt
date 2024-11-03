package com.vanbrusselgames.mindmix.games.sudoku

import android.graphics.Typeface
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.BoxWithConstraints
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.PaddingValues
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.lazy.grid.GridCells
import androidx.compose.foundation.lazy.grid.LazyVerticalGrid
import androidx.compose.foundation.lazy.grid.itemsIndexed
import androidx.compose.material3.LocalTextStyle
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.blur
import androidx.compose.ui.draw.drawBehind
import androidx.compose.ui.draw.scale
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.AnnotatedString
import androidx.compose.ui.text.TextStyle
import androidx.compose.ui.text.font.FontFamily
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.LineHeightStyle
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.TextUnit
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.min
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.common.BaseScene
import com.vanbrusselgames.mindmix.core.common.GameLoadingScreen
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.core.utils.PixelHelper
import kotlin.math.floor

@Composable
fun GameUI(viewModel: GameViewModel, navController: NavController) {
    BaseScene(viewModel, navController) {
        if (!SudokuLoader.puzzleLoaded.value) GameLoadingScreen()
        else SudokuSpecificLayout(viewModel, navController)/*GameFinished_Old.Screen(
            viewModel,
            onClickPlayAgain = { viewModel.startNewGame() })*/
    }
}

@Composable
fun SudokuSpecificLayout(viewModel: GameViewModel, navController: NavController) {
    BoxWithConstraints(
        Modifier
            .fillMaxSize()
            .blur(if (SceneManager.dialogActiveState.value) viewModel.blurStrength else 0.dp),
        Alignment.Center
    ) {
        val maxWidth = this.maxWidth
        val maxHeight = this.maxHeight
        if (maxWidth <= maxHeight) {
            val minSize = min(maxHeight * 0.7f, maxWidth) * 0.95f
            Column(
                modifier = Modifier
                    .height(minSize / 0.7f)
                    .width(minSize),
                horizontalAlignment = Alignment.CenterHorizontally
            ) {
                SceneContent(viewModel, navController, false)
            }
        } else {
            val minSize = min(maxWidth * 0.7f, maxHeight) * 0.95f
            Row(
                modifier = Modifier
                    .height(minSize)
                    .width(minSize / 0.7f),
                verticalAlignment = Alignment.CenterVertically
            ) {
                SceneContent(viewModel, navController, true)
            }
        }
    }
}

@Composable
fun SceneContent(viewModel: GameViewModel, navController: NavController, horizontal: Boolean) {
    SudokuGrid(viewModel)
    Spacer(
        Modifier
            .fillMaxSize(0.1f)
            .aspectRatio(1f)
    )
    SudokuNumPad(viewModel, navController, horizontal)
}

//#region Grid
@Composable
fun SudokuGrid(viewModel: GameViewModel) {
    Box(
        contentAlignment = Alignment.Center,
        modifier = Modifier
            .aspectRatio(1f)
            .background(Color.Black)
    ) {
        LazyVerticalGrid(
            columns = GridCells.Fixed(9),
            userScrollEnabled = false,
            modifier = Modifier.fillMaxSize(0.99f)
        ) {
            itemsIndexed(viewModel.cells) { _, cell ->
                SudokuCell(viewModel, cell)
            }
        }
    }
}

@Composable
fun SudokuCell(viewModel: GameViewModel, cell: SudokuPuzzleCell) {
    val index = cell.id
    val column = index % 3
    val row = (floor(index / 9f) % 3f).toInt()
    val padding = PaddingValues(
        start = if (column == 0) 2.dp else if (column == 1) 1.dp else 0.dp,
        end = if (column == 2) 2.dp else if (column == 1) 1.dp else 0.dp,
        top = if (row == 0) 2.dp else if (row == 1) 1.dp else 0.dp,
        bottom = if (row == 2) 2.dp else if (row == 1) 1.dp else 0.dp,
    )
    val colorScheme = MaterialTheme.colorScheme
    BoxWithConstraints(Modifier
        .fillMaxSize()
        .padding(padding)
        .aspectRatio(1f)
        .clickable(enabled = (!cell.isClue.value && !SceneManager.dialogActiveState.value)) {
            viewModel.cells.forEach { it.isSelected.value = false }
            cell.isSelected.value = true
        }
        .drawBehind {
            drawRect(
                when (true) {
                    cell.isIncorrect.value -> colorScheme.errorContainer
                    cell.isSelected.value -> colorScheme.primaryContainer
                    else -> colorScheme.secondaryContainer
                }
            )
        }, Alignment.Center
    ) {
        val res = LocalContext.current.resources
        val space = remember {
            PixelHelper.pxToSp(res, minOf(this.constraints.maxHeight, this.constraints.minHeight))
        }
        SudokuCellValueText(cell, space)
    }
}

@Composable
fun SudokuCellValueText(cell: SudokuPuzzleCell, space: TextUnit) {
    val isNoteInput = !cell.isClue.value && cell.value.intValue == 0
    if (!isNoteInput) {
        SudokuRegularCellText(cell, space, cell.isClue.value)
    } else {
        SudokuNoteCellText(cell, space)
    }
}

@Composable
fun SudokuRegularCellText(cell: SudokuPuzzleCell, space: TextUnit, isClue: Boolean) {
    val value = cell.value.intValue
    if (value == 0) return
    val colorScheme = MaterialTheme.colorScheme
    Text(
        text = AnnotatedString(value.toString()),
        color = when (true) {
            cell.isIncorrect.value -> colorScheme.onErrorContainer
            cell.isSelected.value -> colorScheme.onPrimaryContainer
            else -> colorScheme.onSecondaryContainer
        },
        textAlign = TextAlign.Center,
        fontSize = space / 2f * if (isClue) 1.15f else 1f,
        fontWeight = if (isClue) FontWeight.ExtraBold else FontWeight.Light,
        style = LocalTextStyle.current.merge(
            TextStyle(
                lineHeightStyle = LineHeightStyle(
                    alignment = LineHeightStyle.Alignment.Center, trim = LineHeightStyle.Trim.Both
                )
            ),
        ),
        modifier = Modifier.scale(1.9f)
    )
}

@Composable
fun SudokuNoteCellText(cell: SudokuPuzzleCell, space: TextUnit) {
    val notes = cell.notes
    if (notes.none { n -> n }) return
    val str = StringBuilder()
    var i = 0
    while (i < 9) {
        val hasNote = notes[i]
        val rIndex = i % 3
        if (i != 0 && rIndex == 0) str.append("\n")
        i++
        str.append(if (hasNote) i.toString() else " ")
        if (rIndex < 2) str.append(" ")
    }
    Text(
        text = AnnotatedString(str.toString()),
        color = when (true) {
            cell.isIncorrect.value -> MaterialTheme.colorScheme.onErrorContainer
            cell.isSelected.value -> MaterialTheme.colorScheme.onPrimaryContainer
            else -> MaterialTheme.colorScheme.onSecondaryContainer
        },
        textAlign = TextAlign.Center,
        fontSize = space / 6f,
        lineHeight = space / 5.5f,
        fontWeight = FontWeight.Normal,
        style = LocalTextStyle.current.merge(
            TextStyle(
                lineHeightStyle = LineHeightStyle(
                    alignment = LineHeightStyle.Alignment.Center, trim = LineHeightStyle.Trim.Both
                ), fontFamily = FontFamily(Typeface.MONOSPACE)
            )
        ),
        modifier = Modifier.scale(1.625f)
    )
}
//#endregion