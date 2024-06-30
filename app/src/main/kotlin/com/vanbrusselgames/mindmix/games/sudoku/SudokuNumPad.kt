package com.vanbrusselgames.mindmix.games.sudoku

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
import androidx.compose.runtime.mutableStateOf
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.scale
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.AnnotatedString
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import com.vanbrusselgames.mindmix.AutoSizeText
import com.vanbrusselgames.mindmix.BaseLayout
import com.vanbrusselgames.mindmix.R
import com.vanbrusselgames.mindmix.games.GameFinished

class SudokuNumPad {

    companion object {
        private val inputMode = mutableStateOf(InputMode.Normal)

        @Composable
        fun Show(horizontal: Boolean) {
            val modifier = Modifier
                .fillMaxSize()
                .aspectRatio(1f)
                .padding(PaddingValues(2.dp))
                .background(MaterialTheme.colorScheme.secondaryContainer)
            LazyVerticalGrid(
                columns = GridCells.Fixed(if (horizontal) 2 else 5),
                userScrollEnabled = false,
                modifier = Modifier
                    .aspectRatio(if (horizontal) 1f / 2.5f else 2.5f)
                    .fillMaxSize()
            ) {
                itemsIndexed(List(10) { 0 }) { index, _ ->
                    if (index == 9) SudokuNumPadInputModeCell(modifier, index)
                    else SudokuNumPadNumberCell(modifier, index)
                }
            }
        }

        @Composable
        private fun SudokuNumPadNumberCell(modifier: Modifier, index: Int) {
            Box(modifier.clickable(enabled = !GameFinished.visible.value && !BaseLayout.activeOverlapUI.value) {
                onClickNumPadCell(index)
            }) {
                AutoSizeText(
                    text = AnnotatedString((index + 1).toString()),
                    color = MaterialTheme.colorScheme.onSecondaryContainer,
                    textAlign = TextAlign.Center,
                    fontWeight = FontWeight.ExtraBold,
                    modifier = Modifier
                        .align(Alignment.Center)
                        .scale(1.325f)
                )
            }
        }

        @Composable
        private fun SudokuNumPadInputModeCell(modifier: Modifier, index: Int) {
            Box(modifier.clickable(enabled = !GameFinished.visible.value && !BaseLayout.activeOverlapUI.value) {
                changeInputMode()
            }) {
                Icon(
                    painterResource(id = if (inputMode.value == InputMode.Normal) R.drawable.outline_stylus_24 else R.drawable.outline_edit_note_24),
                    "",
                    Modifier
                        .fillMaxSize()
                        .scale(0.9f)
                )
            }
        }

        private fun onClickNumPadCell(numPadCellIndex: Int) {
            val cell = SudokuManager.cells.find { it.isSelected.value }
            if (cell == null) return
            if (inputMode.value == InputMode.Normal) {
                cell.value.intValue = numPadCellIndex + 1
                SudokuManager.checkFinished()
                if (SudokuManager.finished) {
                    SudokuManager.cells.forEach { it.isSelected.value = false }
                } else if (SudokuManager.autoEditNotes.value) SudokuManager.autoChangeNotes(cell)
            } else {
                cell.setNote(numPadCellIndex + 1)
                cell.value.intValue = 0
            }
            if (SudokuManager.checkConflictingCells.value) {
                SudokuManager.checkConflictingCell(cell)
            }
        }

        private fun changeInputMode() {
            inputMode.value = if (inputMode.value == InputMode.Normal) InputMode.Note
            else InputMode.Normal
        }
    }
}