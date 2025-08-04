package com.vanbrusselgames.mindmix.games.sudoku.ui

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
import androidx.compose.foundation.lazy.grid.items
import androidx.compose.foundation.text.BasicText
import androidx.compose.foundation.text.TextAutoSize
import androidx.compose.material3.LocalTextStyle
import androidx.compose.material3.MaterialTheme
import androidx.compose.runtime.Composable
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.blur
import androidx.compose.ui.draw.drawBehind
import androidx.compose.ui.draw.scale
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.AnnotatedString
import androidx.compose.ui.text.TextStyle
import androidx.compose.ui.text.font.FontFamily
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.LineHeightStyle
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.em
import androidx.compose.ui.unit.min
import androidx.compose.ui.unit.sp
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.common.BaseScene
import com.vanbrusselgames.mindmix.core.common.GameLoadingScreen
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.games.sudoku.model.SudokuPuzzleCell
import com.vanbrusselgames.mindmix.games.sudoku.navigation.navigateToSudokuGameMenu
import com.vanbrusselgames.mindmix.games.sudoku.navigation.navigateToSudokuSettings
import com.vanbrusselgames.mindmix.games.sudoku.viewmodel.ISudokuViewModel
import kotlin.math.floor

@Composable
fun GameUI(viewModel: ISudokuViewModel, navController: NavController) {
    BaseScene(
        viewModel,
        navController,
        { navController.navigateToSudokuGameMenu() },
        { navController.navigateToSudokuSettings() }) {
        BlurBox(viewModel.blurStrength) { maxWidth, maxHeight ->
            SudokuSpecificLayout(maxWidth, maxHeight) { horizontal ->
                SceneContent(viewModel, navController, horizontal)
            }
        }
        val loadedState = viewModel.puzzleLoaded.collectAsStateWithLifecycle()
        if (!loadedState.value) GameLoadingScreen()
    }
}

@Composable
fun BlurBox(blurStrength: Dp, content: @Composable (maxWidth: Dp, maxHeight: Dp) -> Unit) {
    BoxWithConstraints(
        Modifier
            .fillMaxSize()
            .blur(if (SceneManager.dialogActiveState.value) blurStrength else 0.dp),
        Alignment.Center
    ) {
        content(this.maxWidth, this.maxHeight)
    }
}

@Composable
fun SudokuSpecificLayout(
    maxWidth: Dp, maxHeight: Dp, content: @Composable (horizontal: Boolean) -> Unit
) {
    if (maxWidth <= maxHeight) {
        val minSize = min(maxHeight * 0.7f, maxWidth) * 0.95f
        Column(
            modifier = Modifier
                .height(minSize / 0.7f)
                .width(minSize),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            content(false)
        }
    } else {
        val minSize = min(maxWidth * 0.7f, maxHeight) * 0.95f
        Row(
            modifier = Modifier
                .height(minSize)
                .width(minSize / 0.7f),
            verticalAlignment = Alignment.CenterVertically
        ) {
            content(true)
        }
    }
}

@Composable
fun SceneContent(viewModel: ISudokuViewModel, navController: NavController, horizontal: Boolean) {
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
fun SudokuGrid(viewModel: ISudokuViewModel) {
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
            items(viewModel.cells, { it.id }, contentType = { SudokuPuzzleCell::class }) { cell ->
                SudokuCell(viewModel, cell)
            }
        }
    }
}

@Composable
fun SudokuCell(viewModel: ISudokuViewModel, cell: SudokuPuzzleCell) {
    val padding = remember(cell.id) {
        val index = cell.id
        val column = index % 3
        val row = (floor(index / 9f) % 3f).toInt()
        PaddingValues(
            start = if (column == 0) 2.dp else if (column == 1) 1.dp else 0.dp,
            end = if (column == 2) 2.dp else if (column == 1) 1.dp else 0.dp,
            top = if (row == 0) 2.dp else if (row == 1) 1.dp else 0.dp,
            bottom = if (row == 2) 2.dp else if (row == 1) 1.dp else 0.dp,
        )
    }
    val colorScheme = MaterialTheme.colorScheme
    Box(
        Modifier
            .fillMaxSize()
            .padding(padding)
            .aspectRatio(1f)
            .clickable(enabled = !cell.isClue.value) { viewModel.setSelectedCell(cell.id) }
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
        SudokuCellValueText(cell)
    }
}

@Composable
fun SudokuCellValueText(cell: SudokuPuzzleCell) {
    val isNoteInput = !cell.isClue.value && cell.value.intValue == 0
    if (!isNoteInput) {
        SudokuRegularCellText(cell, cell.isClue.value)
    } else {
        SudokuNoteCellText(cell)
    }
}

@Composable
fun SudokuRegularCellText(cell: SudokuPuzzleCell, isClue: Boolean) {
    val value = cell.value.intValue
    if (value == 0) return
    BasicText(
        text = AnnotatedString(value.toString()),
        modifier = Modifier.scale(if (isClue) 1.15f else 1f),
        style = LocalTextStyle.current.merge(
            TextStyle(
                color = when (true) {
                    cell.isIncorrect.value -> MaterialTheme.colorScheme.onErrorContainer
                    cell.isSelected.value -> MaterialTheme.colorScheme.onPrimaryContainer
                    else -> MaterialTheme.colorScheme.onSecondaryContainer
                },
                fontWeight = if (isClue) FontWeight.ExtraBold else FontWeight.Light,
                textAlign = TextAlign.Center,
                lineHeightStyle = LineHeightStyle(
                    alignment = LineHeightStyle.Alignment.Center, trim = LineHeightStyle.Trim.Both
                ),
            )
        ),
        maxLines = 1,
        autoSize = TextAutoSize.StepBased(maxFontSize = 250.sp)
    )
}

@Composable
fun SudokuNoteCellText(cell: SudokuPuzzleCell) {
    if (cell.notes.none { n -> n }) return
    val str = StringBuilder()
    var i = 0
    while (i < 9) {
        val hasNote = cell.notes[i]
        val rIndex = i % 3
        if (i != 0 && rIndex == 0) str.append("\n")
        i++
        str.append(if (hasNote) i.toString() else " ")
        if (rIndex < 2) str.append(" ")
    }

    val localTextStyle = LocalTextStyle.current
    val textStyle = remember(localTextStyle) {
        localTextStyle.merge(
            TextStyle(
                fontFamily = FontFamily(Typeface.MONOSPACE),
                textAlign = TextAlign.Center,
                lineHeight = 1.25.em,
                lineHeightStyle = LineHeightStyle(
                    alignment = LineHeightStyle.Alignment.Center, trim = LineHeightStyle.Trim.Both
                )
            )
        )
    }

    BasicText(
        text = AnnotatedString(str.toString()),
        style = textStyle.merge(
            color = when (true) {
                cell.isIncorrect.value -> MaterialTheme.colorScheme.onErrorContainer
                cell.isSelected.value -> MaterialTheme.colorScheme.onPrimaryContainer
                else -> MaterialTheme.colorScheme.onSecondaryContainer
            }
        ),
        maxLines = 3,
        minLines = 3,
        autoSize = TextAutoSize.StepBased(maxFontSize = 250.sp, minFontSize = 5.sp)
    )
}
//#endregion