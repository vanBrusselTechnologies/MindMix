package com.vanbrusselgames.mindmix.games.minesweeper.ui.dialogs

import androidx.compose.runtime.Composable
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.feature.gamehelp.GameHelpDialog
import com.vanbrusselgames.mindmix.games.minesweeper.R

@Composable
fun MinesweeperGameHelpDialog(navController: NavController) {
    GameHelpDialog(navController, R.string.minesweeper_name, R.string.minesweeper_desc)
}