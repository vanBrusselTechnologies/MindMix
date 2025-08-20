package com.vanbrusselgames.mindmix.games.sudoku.ui.dialogs

import androidx.compose.runtime.Composable
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.navigation.navigateToMenu
import com.vanbrusselgames.mindmix.core.ui.dialogs.gamemenu.GameMenuDialog
import com.vanbrusselgames.mindmix.games.sudoku.R
import com.vanbrusselgames.mindmix.games.sudoku.navigation.navigateToSudokuSettings
import com.vanbrusselgames.mindmix.games.sudoku.viewmodel.ISudokuViewModel

@Composable
fun SudokuGameMenuDialog(viewModel: ISudokuViewModel, navController: NavController) {
    GameMenuDialog(
        navController,
        R.string.sudoku_name,
        { viewModel.startNewGame() },
        { navController.navigateToSudokuSettings() }) { navController.navigateToMenu() }
}