package com.vanbrusselgames.mindmix.games.game2048.ui.dialogs

import androidx.compose.material3.Surface
import androidx.compose.runtime.Composable
import androidx.compose.runtime.remember
import androidx.compose.ui.tooling.preview.PreviewLightDark
import androidx.navigation.NavController
import androidx.navigation.compose.rememberNavController
import com.vanbrusselgames.mindmix.core.designsystem.theme.MindMixTheme
import com.vanbrusselgames.mindmix.core.navigation.navigateToMenu
import com.vanbrusselgames.mindmix.feature.gamemenu.GameMenuDialog
import com.vanbrusselgames.mindmix.games.game2048.R
import com.vanbrusselgames.mindmix.games.game2048.navigation.navigateToGame2048Settings
import com.vanbrusselgames.mindmix.games.game2048.viewmodel.IGame2048ViewModel
import com.vanbrusselgames.mindmix.games.game2048.viewmodel.MockGame2048ViewModel

@Composable
fun Game2048GameMenuDialog(viewModel: IGame2048ViewModel, navController: NavController) {
    GameMenuDialog(
        navController,
        R.string.game_2048_name,
        { viewModel.startNewGame() },
        { navController.navigateToGame2048Settings() }) { navController.navigateToMenu() }
}

@PreviewLightDark
@Composable
fun Prev_GameMenu() {
    MindMixTheme {
        Surface {
            val vm = remember { MockGame2048ViewModel() }
            Game2048GameMenuDialog(vm, rememberNavController())
        }
    }
}