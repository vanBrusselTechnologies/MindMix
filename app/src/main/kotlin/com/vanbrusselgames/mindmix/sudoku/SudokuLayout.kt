package com.vanbrusselgames.mindmix.sudoku

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.BoxWithConstraints
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.PaddingValues
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.grid.GridCells
import androidx.compose.foundation.lazy.grid.LazyVerticalGrid
import androidx.compose.foundation.lazy.grid.itemsIndexed
import androidx.compose.material3.LocalTextStyle
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.MutableState
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.scale
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.AnnotatedString
import androidx.compose.ui.text.TextStyle
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.LineHeightStyle
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.TextUnit
import androidx.compose.ui.unit.dp
import com.vanbrusselgames.mindmix.BaseLayout
import com.vanbrusselgames.mindmix.BaseUIHandler
import com.vanbrusselgames.mindmix.PixelHelper.Companion.pxToSp
import kotlin.math.floor

class SudokuLayout : BaseLayout() {

    override var uiHandler: BaseUIHandler = SudokuUIHandler()
    private var cellRememberValueList = arrayOf<MutableState<Int>>()
    private var cellColorList = arrayOf<MutableState<Int>>()

    @Composable
    fun BaseScene() {
        super.BaseScene(isMenu = false, sceneSpecific = {
            SudokuSpecificLayout()
        })
    }

    @Composable
    fun SudokuSpecificLayout() {
        val selectedCellIndex = remember { mutableStateOf(-1) }
        val isSudokuFinished = remember { SudokuManager.sudokuFinished }
        if (cellRememberValueList.isEmpty()) {
            cellRememberValueList = Array(81) { remember { mutableStateOf(SudokuData.Input[it]) } }
            cellColorList = Array(81) { remember { mutableStateOf(0) } }
        }
        Box(
            modifier = Modifier.fillMaxSize()
        ) {
            Column(
                modifier = Modifier
                    .aspectRatio(0.7f)
                    .fillMaxSize()
                    .align(Alignment.Center),
                horizontalAlignment = Alignment.CenterHorizontally
            ) {
                SudokuGrid(selectedCellIndex)

                Spacer(
                    modifier = Modifier
                        .fillMaxSize(0.1f)
                        .aspectRatio(1f)
                )

                SudokuNumPad(
                    selectedCellIndex, cellRememberValueList, cellColorList, isSudokuFinished
                ).Show()
            }
        }
    }

    //#region Grid
    @Composable
    fun SudokuGrid(selectedCellIndex: MutableState<Int>) {
        Box(
            modifier = Modifier
                .fillMaxWidth(0.95f)
                .aspectRatio(1f)
                .background(Color.Black)
        ) {
            LazyVerticalGrid(
                columns = GridCells.Fixed(9),
                userScrollEnabled = false,
                modifier = Modifier.align(Alignment.Center)
            ) {
                itemsIndexed(SudokuManager.clues) { index, _ ->
                    SudokuCell(
                        index = index
                    ) {
                        if (cellColorList[index].value != -1) {
                            cellColorList[index].value = 1
                        }
                        if (selectedCellIndex.value != -1 && cellColorList[selectedCellIndex.value].value != -1) {
                            cellColorList[selectedCellIndex.value].value = 0
                        }
                        selectedCellIndex.value = index
                    }
                }
            }
        }
    }

    @Composable
    fun SudokuCell(
        index: Int, onSelected: () -> Unit
    ) {
        val vertIndex = index % 3
        val horzIndex = floor(index / 9f) % 3f
        val padding = PaddingValues(
            start = if (vertIndex == 0) 2.dp else if (vertIndex == 1) 1.25.dp else 0.dp,
            end = if (vertIndex == 2) 2.dp else if (vertIndex == 1) 1.25.dp else 0.dp,
            top = if (horzIndex == 0f) 2.dp else if (horzIndex == 1f) 1.25.dp else 0.dp,
            bottom = if (horzIndex == 2f) 2.dp else if (horzIndex == 1f) 1.25.dp else 0.dp,
        )
        val isClue = SudokuManager.clues[index] in 1..9
        BoxWithConstraints(
            Modifier
                .fillMaxSize()
                .padding(padding)
                .background(
                    when (cellColorList[index].value) {
                        -1 -> MaterialTheme.colorScheme.errorContainer
                        1 -> MaterialTheme.colorScheme.primaryContainer
                        else -> MaterialTheme.colorScheme.secondaryContainer
                    }
                )
                .aspectRatio(1f)
                .clickable(enabled = (!isClue && !SudokuManager.sudokuFinished.value)) { onSelected() }) {
            val space = pxToSp(minOf(constraints.maxHeight, constraints.minHeight))
            val isNoteInput =
                !isClue && cellRememberValueList[index].value <= 0 && !SudokuData.InputNotes[index].contentEquals(
                    IntArray(9) { 0 })
            val textColor = when (cellColorList[index].value) {
                -1 -> MaterialTheme.colorScheme.onErrorContainer
                1 -> MaterialTheme.colorScheme.onPrimaryContainer
                else -> MaterialTheme.colorScheme.onSecondaryContainer
            }

            if (!isNoteInput) {
                val cellValue = if (isClue) SudokuManager.clues[index] else SudokuData.Input[index]
                if (cellValue == 0) return@BoxWithConstraints
                SudokuRegularCellText(
                    value = cellValue,
                    space = space,
                    textColor = textColor,
                    isClue = isClue
                )
            } else {
                SudokuNoteCellText(
                    value = SudokuData.InputNotes[index],
                    space = space,
                    textColor = textColor
                )
            }
        }
    }

    @Composable
    fun SudokuRegularCellText(space: TextUnit, value: Int, textColor: Color, isClue: Boolean) {
        Box(Modifier.fillMaxSize()) {
            Text(
                text = AnnotatedString(value.toString()),
                color = textColor,
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
                modifier = Modifier
                    .align(Alignment.Center)
                    .scale(2f)
            )
        }
    }

    @Composable
    fun SudokuNoteCellText(value: IntArray, space: TextUnit, textColor: Color) {
        Box(Modifier.fillMaxSize()) {
            val str = StringBuilder()
            var i = 0
            while (i < 9) {
                val num = value[i]
                val rIndex = i % 3
                if (i != 0 && rIndex == 0) str.append("\n")
                if (num == 0) str.append("  ") else str.append(num.toString())
                if (rIndex < 2) str.append(" ")
                i++
            }
            Text(
                text = AnnotatedString(str.toString()),
                color = textColor,
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
                modifier = Modifier
                    .align(Alignment.Center)
                    .scale(2f)
            )
        }
    }
    //#endregion
}