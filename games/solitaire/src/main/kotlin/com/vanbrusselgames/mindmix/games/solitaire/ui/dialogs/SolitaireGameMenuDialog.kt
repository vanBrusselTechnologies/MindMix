package com.vanbrusselgames.mindmix.games.solitaire.ui.dialogs

import androidx.compose.runtime.Composable
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.navigation.navigateToMenu
import com.vanbrusselgames.mindmix.feature.gamemenu.GameMenuDialog
import com.vanbrusselgames.mindmix.games.solitaire.navigation.navigateToSolitaireSettings
import com.vanbrusselgames.mindmix.games.solitaire.viewmodel.ISolitaireViewModel

@Composable
fun SolitaireGameMenuDialog(viewModel: ISolitaireViewModel, navController: NavController) {
    GameMenuDialog(
        navController,
        viewModel.nameResId,
        { viewModel.startNewGame() },
        { navController.navigateToSolitaireSettings() }) { navController.navigateToMenu() }
}