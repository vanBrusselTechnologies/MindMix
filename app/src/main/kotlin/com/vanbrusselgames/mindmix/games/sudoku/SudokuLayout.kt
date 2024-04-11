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
import androidx.compose.ui.unit.TextUnit
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.min
import com.vanbrusselgames.mindmix.BaseLayout
import com.vanbrusselgames.mindmix.Logger
import com.vanbrusselgames.mindmix.PixelHelper.Companion.pxToSp
import com.vanbrusselgames.mindmix.R
import com.vanbrusselgames.mindmix.games.GameFinished
import com.vanbrusselgames.mindmix.games.GameHelp
import com.vanbrusselgames.mindmix.games.GameLoadingScreen
import com.vanbrusselgames.mindmix.games.GameMenu
import kotlin.math.floor

class SudokuLayout : BaseLayout() {

    @Composable
    fun Scene() {
        BaseScene {
            if (!SudokuLoader.puzzleLoaded.value) {
                return@BaseScene GameLoadingScreen()
            }
            SudokuSpecificLayout(activeOverlapUI.value)
            GameHelp.Screen(R.string.sudoku_name, R.string.sudoku_desc)
            GameMenu.Screen(R.string.sudoku_name) { SudokuManager.startNewGame() }
            GameFinished.Screen(onClickPlayAgain = { SudokuManager.startNewGame() })
            SudokuSettings()
        }
    }

    @Composable
    fun SudokuSpecificLayout(isBlurred: Boolean) {
        BoxWithConstraints(
            Modifier
                .fillMaxSize()
                .blur(if (isBlurred) blurStrength else 0.dp), Alignment.Center
        ) {
            val maxWidth = maxWidth
            val maxHeight = maxHeight
            if (maxWidth <= maxHeight) {
                val minSize = min(maxHeight * 0.7f, maxWidth) * 0.95f
                Column(
                    modifier = Modifier
                        .height(minSize / 0.7f)
                        .width(minSize),
                    horizontalAlignment = Alignment.CenterHorizontally
                ) {
                    SceneContent(false)
                }
            } else {
                val minSize = min(maxWidth * 0.7f, maxHeight) * 0.95f
                Row(
                    modifier = Modifier
                        .height(minSize)
                        .width(minSize / 0.7f),
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    SceneContent(true)
                }
            }
        }
    }

    @Composable
    fun SceneContent(horizontal: Boolean) {
        SudokuGrid()
        Spacer(
            Modifier
                .fillMaxSize(0.1f)
                .aspectRatio(1f)
        )
        SudokuNumPad.Show(horizontal)
    }

    //#region Grid
    @Composable
    fun SudokuGrid() {
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
                Logger.d(" 3::: " + SudokuManager.cells.map{it.value}.joinToString(""))
                itemsIndexed(SudokuManager.cells) { _, cell ->
                    SudokuCell(cell)
                }
            }
        }
    }

    @Composable
    fun SudokuCell(cell: SudokuPuzzleCell) {
        val index = cell.id
        val column = index % 3
        val row = (floor(index / 9f) % 3f).toInt()
        val padding = PaddingValues(
            start = if (column == 0) 2.dp else if (column == 1) 1.25.dp else 0.dp,
            end = if (column == 2) 2.dp else if (column == 1) 1.25.dp else 0.dp,
            top = if (index < 9) 1.25.dp else if (row == 0) 2.dp else if (row == 1) 1.25.dp else 0.dp,
            bottom = if (index >= 72) 1.25.dp else if (row == 2) 2.dp else if (row == 1) 1.25.dp else 0.dp,
        )
        val colorScheme = MaterialTheme.colorScheme
        BoxWithConstraints(contentAlignment = Alignment.Center,
            modifier = Modifier
                .fillMaxSize()
                .padding(padding)
                .aspectRatio(1f)
                .clickable(enabled = (!cell.mutableIsClue.value && !GameFinished.visible.value && !activeOverlapUI.value)) {
                    cell.isSelected = true

                    val currSelectedCellId = SudokuManager.selectedCellIndex
                    if (currSelectedCellId != -1 && currSelectedCellId != cell.id) {
                        SudokuManager.cells[currSelectedCellId].isSelected = false
                    }

                    SudokuManager.selectedCellIndex = cell.id
                }
                .drawBehind {
                    drawRect(
                        when (true) {
                            cell.mutableIsIncorrect.value -> colorScheme.errorContainer
                            cell.mutableIsSelected.value -> colorScheme.primaryContainer
                            else -> colorScheme.secondaryContainer
                        }
                    )
                }) {
            val space = pxToSp(minOf(constraints.maxHeight, constraints.minHeight))
            SudokuCellValueText(cell, space)
        }
    }

    @Composable
    fun SudokuCellValueText(cell: SudokuPuzzleCell, space: TextUnit) {
        val isNoteInput = !cell.isClue && cell.mutableCellValue.intValue == 0
        if (!isNoteInput) {
            SudokuRegularCellText(cell = cell, space = space, isClue = cell.mutableIsClue.value)
        } else {
            SudokuNoteCellText(cell = cell, space = space)
        }
    }

    @Composable
    fun SudokuRegularCellText(
        cell: SudokuPuzzleCell, space: TextUnit, isClue: Boolean
    ) {
        val value = cell.mutableCellValue.intValue
        if (value == 0) return
        val colorScheme = MaterialTheme.colorScheme
        Text(
            text = AnnotatedString(value.toString()),
            color = when (true) {
                cell.mutableIsIncorrect.value -> colorScheme.onErrorContainer
                cell.mutableIsSelected.value -> colorScheme.onPrimaryContainer
                else -> colorScheme.onSecondaryContainer
            },
            textAlign = TextAlign.Center,
            fontSize = space / 2f * if (isClue) 1.15f else 1f,
            fontWeight = if (isClue) FontWeight.ExtraBold else FontWeight.Light,
            style = LocalTextStyle.current.merge(
                TextStyle(
                    lineHeightStyle = LineHeightStyle(
                        alignment = LineHeightStyle.Alignment.Center,
                        trim = LineHeightStyle.Trim.Both
                    )//, fontFamily = FontFamily(Typeface.MONOSPACE)
                ),
            ),
            modifier = Modifier.scale(1.9f)
        )
    }

    @Composable
    fun SudokuNoteCellText(cell: SudokuPuzzleCell, space: TextUnit) {
        val notes = cell.mutableCellNotes
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
                cell.mutableIsIncorrect.value -> MaterialTheme.colorScheme.onErrorContainer
                cell.mutableIsSelected.value -> MaterialTheme.colorScheme.onPrimaryContainer
                else -> MaterialTheme.colorScheme.onSecondaryContainer
            },
            textAlign = TextAlign.Center,
            fontSize = space / 6f,
            lineHeight = space / 5.5f,
            fontWeight = FontWeight.Normal,
            style = LocalTextStyle.current.merge(
                TextStyle(
                    lineHeightStyle = LineHeightStyle(
                        alignment = LineHeightStyle.Alignment.Center,
                        trim = LineHeightStyle.Trim.Both
                    ), fontFamily = FontFamily(Typeface.MONOSPACE)
                )
            ),
            modifier = Modifier.scale(1.625f)
        )
    }
    //#endregion
}