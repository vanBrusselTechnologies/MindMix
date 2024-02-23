package com.vanbrusselgames.mindmix.sudoku

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.PaddingValues
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.grid.GridCells
import androidx.compose.foundation.lazy.grid.LazyVerticalGrid
import androidx.compose.foundation.lazy.grid.itemsIndexed
import androidx.compose.material3.Icon
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

    companion object {
        @Composable
        fun Show(horizontal: Boolean) {
            LazyVerticalGrid(
                columns = GridCells.Fixed(if (horizontal) 2 else 5),
                userScrollEnabled = false,
                modifier = Modifier
                    .aspectRatio(if (horizontal) 1f / 2.5f else 2.5f)
                    .fillMaxSize()
            ) {
                itemsIndexed(List(10) { 0 }) { index, _ ->
                    SudokuNumPadCell(index, value = index + 1)
                }
            }
        }

        @Composable
        private fun SudokuNumPadCell(index: Int, value: Int) {
            val inputMode = remember { mutableStateOf(SudokuManager.inputMode) }
            val padding = PaddingValues(
                start = 2.dp,
                end = 2.dp,
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
                        painterResource(id = R.drawable.outline_stylus_24)
                    } else painterResource(id = R.drawable.outline_edit_note_24)
                    Icon(
                        imagePainter, "",
                        Modifier
                            .fillMaxSize()
                            .scale(0.9f)
                    )
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
            SudokuManager.inputMode =
                if (SudokuManager.inputMode == InputMode.Normal) InputMode.Note
                else InputMode.Normal
            inputMode.value = SudokuManager.inputMode
        }
    }
}