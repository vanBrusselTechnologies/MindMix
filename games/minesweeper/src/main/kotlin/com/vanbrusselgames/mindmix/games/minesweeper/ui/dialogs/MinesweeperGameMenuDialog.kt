package com.vanbrusselgames.mindmix.games.minesweeper.ui.dialogs

import androidx.compose.runtime.Composable
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.navigation.navigateToMenu
import com.vanbrusselgames.mindmix.feature.gamemenu.GameMenuDialog
import com.vanbrusselgames.mindmix.games.minesweeper.navigation.navigateToMinesweeperSettings
import com.vanbrusselgames.mindmix.games.minesweeper.viewmodel.IMinesweeperViewModel

@Composable
fun MinesweeperGameMenuDialog(viewModel: IMinesweeperViewModel, navController: NavController) {
    GameMenuDialog(
        navController,
        viewModel.nameResId,
        { viewModel.startNewGame() },
        { navController.navigateToMinesweeperSettings() }) { navController.navigateToMenu() }
}