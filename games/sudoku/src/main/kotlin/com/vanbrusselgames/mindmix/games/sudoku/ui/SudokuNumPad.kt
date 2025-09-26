package com.vanbrusselgames.mindmix.games.sudoku.ui

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.PaddingValues
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.grid.GridCells
import androidx.compose.foundation.lazy.grid.LazyVerticalGrid
import androidx.compose.foundation.text.TextAutoSize
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.scale
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.TextStyle
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.games.sudoku.R
import com.vanbrusselgames.mindmix.games.sudoku.model.InputMode
import com.vanbrusselgames.mindmix.games.sudoku.viewmodel.ISudokuViewModel

@Composable
fun SudokuNumPad(viewModel: ISudokuViewModel, navController: NavController, horizontal: Boolean) {
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
        items(10, { it }) { index ->
            if (index == 9) SudokuNumPadInputModeCell(viewModel, modifier)
            else SudokuNumPadNumberCell(viewModel, modifier, index, navController)
        }
    }
}

@Composable
private fun SudokuNumPadNumberCell(
    viewModel: ISudokuViewModel, modifier: Modifier, index: Int, navController: NavController
) {
    Box(modifier.clickable {
        viewModel.onClickNumPadCell(index, navController)
    }) {
        Text(
            text = (index + 1).toString(),
            modifier = Modifier
                .align(Alignment.Center)
                .scale(1.125f),
            autoSize = TextAutoSize.StepBased(maxFontSize = 250.sp),
            style = TextStyle(
                color = MaterialTheme.colorScheme.onSecondaryContainer,
                fontWeight = FontWeight.ExtraBold,
                textAlign = TextAlign.Center
            ),
            maxLines = 1
        )
    }
}

@Composable
private fun SudokuNumPadInputModeCell(viewModel: ISudokuViewModel, modifier: Modifier) {
    Box(modifier.clickable { viewModel.changeInputMode() }) {
        Icon(
            painterResource(if (viewModel.inputMode.value == InputMode.Normal) R.drawable.outline_stylus_24 else R.drawable.outline_edit_note_24),
            "Change Sudoku InputMode",
            Modifier
                .fillMaxSize()
                .scale(0.9f)
        )
    }
}