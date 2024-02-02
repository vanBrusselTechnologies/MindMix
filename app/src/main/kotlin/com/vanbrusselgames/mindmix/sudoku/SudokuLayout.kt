package com.vanbrusselgames.mindmix.sudoku

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
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.LineHeightStyle
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.TextUnit
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.min
import com.vanbrusselgames.mindmix.BaseLayout
import com.vanbrusselgames.mindmix.BaseUIHandler
import com.vanbrusselgames.mindmix.PixelHelper.Companion.pxToSp
import com.vanbrusselgames.mindmix.sudoku.SudokuManager.Instance.sudokuFinished
import kotlin.math.floor

class SudokuLayout : BaseLayout() {
    override var uiHandler: BaseUIHandler = SudokuUIHandler()

    @Composable
    fun BaseScene() {
        super.BaseScene(isMenu = false, sceneSpecific = {
            val finished = sudokuFinished.value
            SudokuSpecificLayout(sudokuFinished.value)
            if (finished) GameFinishedPopUp()
        })
    }

    @Composable
    fun SudokuSpecificLayout(isBlurred: Boolean) {
        BoxWithConstraints(
            Modifier
                .fillMaxSize()
                .blur(if (isBlurred) 4.dp else 0.dp),
            contentAlignment = Alignment.Center
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
                itemsIndexed(SudokuManager.cells) { _, cellData ->
                    SudokuCell(cellData)
                }
            }
        }
    }

    @Composable
    fun SudokuCell(cellData: SudokuPuzzleCell) {
        val index = cellData.id
        val vertIndex = index % 3
        val horzIndex = floor(index / 9f) % 3f
        val padding = PaddingValues(
            start = if (vertIndex == 0) 2.dp else if (vertIndex == 1) 1.25.dp else 0.dp,
            end = if (vertIndex == 2) 2.dp else if (vertIndex == 1) 1.25.dp else 0.dp,
            top = if (index < 9) 1.25.dp else if (horzIndex == 0f) 2.dp else if (horzIndex == 1f) 1.25.dp else 0.dp,
            bottom = if (horzIndex == 2f) 2.dp else if (horzIndex == 1f) 1.25.dp else 0.dp,
        )
        val colorScheme = MaterialTheme.colorScheme
        BoxWithConstraints(contentAlignment = Alignment.Center,
            modifier = Modifier
                .fillMaxSize()
                .padding(padding)
                .aspectRatio(1f)
                .clickable(enabled = (!cellData.isClue && !SudokuManager.sudokuFinished.value)) {
                    cellData.isSelected = true

                    val currSelectedCellId = SudokuManager.selectedCellIndex
                    if (currSelectedCellId != -1 && currSelectedCellId != cellData.id) {
                        SudokuManager.cells[currSelectedCellId].isSelected = false
                    }

                    SudokuManager.selectedCellIndex = cellData.id
                }
                .drawBehind {
                    drawRect(
                        when (true) {
                            cellData.mutableIsIncorrect.value -> colorScheme.errorContainer
                            cellData.mutableIsSelected.value -> colorScheme.primaryContainer
                            else -> colorScheme.secondaryContainer
                        }
                    )
                }) {
            val space = pxToSp(minOf(constraints.maxHeight, constraints.minHeight))
            SudokuCellValueText(cellData, space)
        }
    }

    @Composable
    fun SudokuCellValueText(cellData: SudokuPuzzleCell, space: TextUnit) {
        val isNoteInput = !cellData.isClue && cellData.mutableCellValue.intValue == 0
        if (!isNoteInput) {
            SudokuRegularCellText(cellData = cellData, space = space, isClue = cellData.isClue)
        } else {
            SudokuNoteCellText(cellData = cellData, space = space)
        }
    }

    @Composable
    fun SudokuRegularCellText(
        cellData: SudokuPuzzleCell, space: TextUnit, isClue: Boolean
    ) {
        val value = cellData.mutableCellValue.intValue
        if (value == 0) return
        Text(
            text = AnnotatedString(value.toString()),
            color = when (true) {
                cellData.mutableIsIncorrect.value -> MaterialTheme.colorScheme.onErrorContainer
                cellData.mutableIsSelected.value -> MaterialTheme.colorScheme.onPrimaryContainer
                else -> MaterialTheme.colorScheme.onSecondaryContainer
            },
            textAlign = TextAlign.Center,
            fontSize = space / 2f * if (isClue) 1.15f else 1f,
            fontWeight = if (isClue) FontWeight.ExtraBold else FontWeight.Normal,
            style = LocalTextStyle.current.merge(
                TextStyle(
                    lineHeightStyle = LineHeightStyle(
                        alignment = LineHeightStyle.Alignment.Center,
                        trim = LineHeightStyle.Trim.Both
                    ), fontFeatureSettings = "tnum"
                ),
            ),
            modifier = Modifier.scale(1.9f)
        )
    }

    @Composable
    fun SudokuNoteCellText(cellData: SudokuPuzzleCell, space: TextUnit) {
        val notes = cellData.mutableCellNotes
        if (notes.none { n -> n }) return
        val str = StringBuilder()
        var i = 0
        while (i < 9) {
            val hasNote = notes[i]
            val rIndex = i % 3
            if (i != 0 && rIndex == 0) str.append("\n")
            i++
            str.append(if (hasNote) i.toString() else "  ")
            if (rIndex < 2) str.append(" ")
        }
        Text(
            text = AnnotatedString(str.toString()),
            color = when (true) {
                cellData.mutableIsIncorrect.value -> MaterialTheme.colorScheme.onErrorContainer
                cellData.mutableIsSelected.value -> MaterialTheme.colorScheme.onPrimaryContainer
                else -> MaterialTheme.colorScheme.onSecondaryContainer
            },
            textAlign = TextAlign.Center,
            fontSize = space / 6f,
            lineHeight = space / 6f,
            fontWeight = FontWeight.Normal,
            style = LocalTextStyle.current.merge(
                TextStyle(
                    lineHeightStyle = LineHeightStyle(
                        alignment = LineHeightStyle.Alignment.Center,
                        trim = LineHeightStyle.Trim.Both
                    ), fontFeatureSettings = "tnum"
                )
            ),
            modifier = Modifier.scale(1.9f)
        )
    }
    //#endregion

    @Composable
    fun GameFinishedPopUp() {
        val title = "Congrats"
        val desc = """You did great and solved puzzle in ${0} seconds!!
                        |That's Awesome!
                        |Share with your friends and challenge them to beat your time!""".trimMargin()
        val reward = 10
        val onClickShare = {}
        val onClickPlayAgain = {
            SudokuManager.reset()
            SudokuManager.loadPuzzle()
        }
        val onClickReturnToMenu = {
            uiHandler.backToMenu()
            SudokuManager.reset()
        }
        BaseGameFinishedPopUp(
            title, desc, reward, onClickShare, onClickPlayAgain, onClickReturnToMenu
        )
    }
}