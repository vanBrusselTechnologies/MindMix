package com.vanbrusselgames.mindmix.games.sudoku

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.PaddingValues
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.grid.GridCells
import androidx.compose.foundation.lazy.grid.LazyVerticalGrid
import androidx.compose.foundation.lazy.grid.itemsIndexed
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.scale
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.AnnotatedString
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.core.ui.AutoSizeText

@Composable
fun SudokuNumPad(viewModel: SudokuViewModel, navController: NavController, horizontal: Boolean) {
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
        itemsIndexed(List(10) { 0 }, { index, _ -> index }) { index, _ ->
            if (index == 9) SudokuNumPadInputModeCell(viewModel, modifier)
            else SudokuNumPadNumberCell(viewModel, modifier, index, navController)
        }
    }
}

@Composable
private fun SudokuNumPadNumberCell(
    viewModel: SudokuViewModel, modifier: Modifier, index: Int, navController: NavController
) {
    Box(modifier.clickable(enabled = !SceneManager.dialogActiveState.value) {
        viewModel.onClickNumPadCell(index, navController)
    }) {
        AutoSizeText(
            text = AnnotatedString((index + 1).toString()),
            color = MaterialTheme.colorScheme.onSecondaryContainer,
            textAlign = TextAlign.Center,
            fontWeight = FontWeight.ExtraBold,
            modifier = Modifier
                .align(Alignment.Center)
                .scale(1.125f)
                .offset(0.dp, (-5).dp)
        )
    }
}

@Composable
private fun SudokuNumPadInputModeCell(viewModel: SudokuViewModel, modifier: Modifier) {
    Box(modifier.clickable(enabled = !SceneManager.dialogActiveState.value) {
        viewModel.changeInputMode()
    }) {
        Icon(
            painterResource(id = if (viewModel.inputMode.value == InputMode.Normal) R.drawable.outline_stylus_24 else R.drawable.outline_edit_note_24),
            "Change Sudoku InputMode",
            Modifier
                .fillMaxSize()
                .scale(0.9f)
        )
    }
}