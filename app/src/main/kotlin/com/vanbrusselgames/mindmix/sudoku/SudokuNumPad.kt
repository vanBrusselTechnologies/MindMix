package com.vanbrusselgames.mindmix.sudoku

import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.PaddingValues
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.grid.GridCells
import androidx.compose.foundation.lazy.grid.LazyVerticalGrid
import androidx.compose.foundation.lazy.grid.itemsIndexed
import androidx.compose.material3.MaterialTheme
import androidx.compose.runtime.Composable
import androidx.compose.runtime.MutableState
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.scale
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.AnnotatedString
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import com.vanbrusselgames.mindmix.AutoSizeText
import com.vanbrusselgames.mindmix.R
import com.vanbrusselgames.mindmix.sudoku.SudokuManager.Instance.InputMode

class SudokuNumPad {
    @Composable
    fun Show() {
        Box(
            modifier = Modifier
                .fillMaxWidth(0.95f)
                .aspectRatio(1f)
        ) {
            LazyVerticalGrid(
                columns = GridCells.Fixed(5),
                userScrollEnabled = false,
                modifier = Modifier
                    .aspectRatio(2.5f)
                    .fillMaxSize(0.95f)
                    .align(Alignment.Center)
            ) {
                itemsIndexed(List(10) { 0 }) { index, _ ->
                    SudokuNumPadCell(index, value = index + 1)
                }
            }
        }
    }

    @Composable
    private fun SudokuNumPadCell(index: Int, value: Int) {
        val inputMode = remember { mutableStateOf(InputMode.Normal) }
        val padding = PaddingValues(
            start = if (index % 5 == 0) 4.dp else 2.dp,
            end = if (index % 5 == 4) 4.dp else 2.dp,
            top = 2.dp,
            bottom = 2.dp,
        )
        Box(
            Modifier
                .fillMaxSize()
                .aspectRatio(1f)
                .padding(padding)
                .background(MaterialTheme.colorScheme.secondaryContainer)
                .clickable(enabled = !SudokuManager.sudokuFinished.value) {
                    if (value != 10) onClickNumPadCell(index)
                    else changeInputMode(inputMode)
                }) {
            if (value in 1..9) {
                AutoSizeText(
                    text = AnnotatedString(value.toString()),
                    color = MaterialTheme.colorScheme.onSecondaryContainer,
                    textAlign = TextAlign.Center,
                    fontWeight = FontWeight.ExtraBold,
                    modifier = Modifier
                        .align(Alignment.Center)
                        .scale(1.325f)
                )
            }
            if (value == 10) {
                val imagePainter = if (inputMode.value == InputMode.Normal) {
                    painterResource(id = R.drawable.pencil)
                } else painterResource(id = R.drawable.note_paper)
                Image(imagePainter, "", Modifier.scale(0.9f))
            }
        }
    }

    private fun onClickNumPadCell(numPadCellIndex: Int) {
        val gridCellIndex = SudokuManager.selectedCellIndex
        if (gridCellIndex == -1) return
        if (SudokuManager.inputMode == InputMode.Normal) {
            SudokuManager.cells[gridCellIndex].setNumber(numPadCellIndex + 1)
            SudokuManager.checkFinished()
            if (SudokuManager.finished) {
                SudokuManager.selectedCellIndex = -1
                SudokuManager.cells.find { c -> c.isSelected }?.isSelected = false
            } else if (SudokuManager.autoEditNotes) SudokuManager.autoChangeNotes(gridCellIndex)
        } else {
            SudokuManager.cells[gridCellIndex].setNote(numPadCellIndex + 1)
            SudokuManager.cells[gridCellIndex].value = 0
        }
        if (SudokuManager.checkConflictingCells) {
            SudokuManager.checkConflictingCell(gridCellIndex)
        }
    }

    private fun changeInputMode(inputMode: MutableState<InputMode>) {
        SudokuManager.inputMode = if (SudokuManager.inputMode == InputMode.Normal) InputMode.Note
        else InputMode.Normal
        inputMode.value = SudokuManager.inputMode
    }
}