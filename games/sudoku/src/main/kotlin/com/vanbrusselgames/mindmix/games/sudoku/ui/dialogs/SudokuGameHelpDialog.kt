package com.vanbrusselgames.mindmix.games.sudoku.ui.dialogs

import androidx.compose.runtime.Composable
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.feature.gamehelp.GameHelpDialog
import com.vanbrusselgames.mindmix.games.sudoku.R

@Composable
fun SudokuGameHelpDialog(navController: NavController) {
    GameHelpDialog(navController, R.string.sudoku_name, R.string.sudoku_desc)
}