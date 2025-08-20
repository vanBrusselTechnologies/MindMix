package com.vanbrusselgames.mindmix.games.game2048.ui.dialogs

import androidx.compose.runtime.Composable
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.ui.dialogs.gamehelp.GameHelpDialog
import com.vanbrusselgames.mindmix.games.game2048.R

@Composable
fun Game2048GameHelpDialog(navController: NavController) {
    GameHelpDialog(navController, R.string.game_2048_name, R.string.game_2048_desc)
}