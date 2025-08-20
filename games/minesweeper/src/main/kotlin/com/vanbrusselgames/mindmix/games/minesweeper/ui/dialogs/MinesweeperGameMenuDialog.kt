package com.vanbrusselgames.mindmix.games.minesweeper.ui.dialogs

import androidx.compose.runtime.Composable
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.navigation.navigateToMenu
import com.vanbrusselgames.mindmix.core.ui.dialogs.gamemenu.GameMenuDialog
import com.vanbrusselgames.mindmix.games.minesweeper.R
import com.vanbrusselgames.mindmix.games.minesweeper.navigation.navigateToMinesweeperSettings
import com.vanbrusselgames.mindmix.games.minesweeper.viewmodel.IMinesweeperViewModel

@Composable
fun MinesweeperGameMenuDialog(viewModel: IMinesweeperViewModel, navController: NavController) {
    GameMenuDialog(
        navController,
        R.string.minesweeper_name,
        { viewModel.startNewGame() },
        { navController.navigateToMinesweeperSettings() }) { navController.navigateToMenu() }
}