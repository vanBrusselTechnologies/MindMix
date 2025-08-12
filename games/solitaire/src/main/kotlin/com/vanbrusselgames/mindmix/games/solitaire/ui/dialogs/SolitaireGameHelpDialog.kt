package com.vanbrusselgames.mindmix.games.solitaire.ui.dialogs

import androidx.compose.runtime.Composable
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.feature.gamehelp.GameHelpDialog
import com.vanbrusselgames.mindmix.games.solitaire.R

@Composable
fun SolitaireGameHelpDialog(navController: NavController) {
    GameHelpDialog(navController, R.string.solitaire_name, R.string.solitaire_desc)
}