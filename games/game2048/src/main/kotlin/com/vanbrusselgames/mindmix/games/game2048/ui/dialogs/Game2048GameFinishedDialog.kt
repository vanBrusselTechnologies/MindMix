package com.vanbrusselgames.mindmix.games.game2048.ui.dialogs

import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.IntrinsicSize
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.width
import androidx.compose.material3.Surface
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.tooling.preview.PreviewLightDark
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.navigation.NavController
import androidx.navigation.compose.rememberNavController
import com.vanbrusselgames.mindmix.core.designsystem.theme.MindMixTheme
import com.vanbrusselgames.mindmix.core.navigation.navigateToMenu
import com.vanbrusselgames.mindmix.core.ui.dialogs.DialogButton
import com.vanbrusselgames.mindmix.core.ui.dialogs.gamefinished.Buttons
import com.vanbrusselgames.mindmix.core.ui.dialogs.gamefinished.GameFinishedDialog
import com.vanbrusselgames.mindmix.core.ui.dialogs.gamefinished.StatRow
import com.vanbrusselgames.mindmix.core.ui.dialogs.gamefinished.Stats
import com.vanbrusselgames.mindmix.games.game2048.R
import com.vanbrusselgames.mindmix.games.game2048.model.FinishedGame
import com.vanbrusselgames.mindmix.games.game2048.model.SuccessType
import com.vanbrusselgames.mindmix.games.game2048.viewmodel.IGame2048ViewModel
import com.vanbrusselgames.mindmix.games.game2048.viewmodel.MockGame2048ViewModel

@Composable
fun Game2048GameFinishedDialog(viewModel: IGame2048ViewModel, navController: NavController) {
    GameFinishedDialog {
        Column(
            Modifier
                .padding(16.dp)
                .width(IntrinsicSize.Min),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            val finishedGame = viewModel.finishedGame.value
            val successType = finishedGame.successType

            GameFinishedDialogTitle(successType)
            Spacer(Modifier.height(2.dp))
            GameFinishedDialogText(successType, finishedGame.targetTile, finishedGame.score)
            Spacer(Modifier.height(8.dp))
            Stats {
                StatRow(
                    fieldText = stringResource(R.string.game_2048_score_label),
                    valueText = finishedGame.score.toString()
                )
                StatRow(
                    fieldText = stringResource(R.string.game_2048_stat_max_tile_label),
                    valueText = finishedGame.highestTileValue.toString()
                )
            }
            Spacer(Modifier.height(8.dp))
            if (successType == SuccessType.REACHED_TARGET) {
                DialogButton({
                    viewModel.continueGame()
                    navController.popBackStack()
                }, Modifier.fillMaxWidth()) {
                    Text(stringResource(R.string.game_2048_continue_game))
                }
            } else {
                Buttons(
                    navController,
                    Modifier.fillMaxWidth(),
                    null,
                    { viewModel.startNewGame() }) { navController.navigateToMenu() }
            }
        }
    }
}

@Composable
private fun GameFinishedDialogTitle(successType: SuccessType) {
    Text(
        stringResource(
            when (successType) {
                SuccessType.GAME_OVER -> R.string.game_2048_game_over_title
                SuccessType.REACHED_TARGET -> R.string.game_2048_reach_target_title
                SuccessType.SUCCESS -> R.string.game_2048_name
            }
        ), fontSize = 25.sp, fontWeight = FontWeight.ExtraBold
    )
}

@Composable
private fun GameFinishedDialogText(successType: SuccessType, targetTile: Long, score: Long) {
    Text(
        when (successType) {
            SuccessType.GAME_OVER -> gameOverText(targetTile)
            SuccessType.REACHED_TARGET -> reachedTargetText(targetTile, score)
            SuccessType.SUCCESS -> successText(targetTile)
        }, Modifier.width(IntrinsicSize.Max), textAlign = TextAlign.Center
    )
}

@Composable
private fun reachedTargetText(targetTile: Long, score: Long): String {
    return stringResource(R.string.game_2048_reach_target_text, targetTile, score)
}

@Composable
private fun gameOverText(targetTile: Long): String {
    return stringResource(R.string.game_2048_game_over_text, targetTile)
}

@Composable
private fun successText(targetTile: Long): String {
    return stringResource(R.string.game_2048_success, targetTile)
}

@PreviewLightDark
@Composable
fun Prev_GameFinished() {
    MindMixTheme {
        Surface {
            val vm = remember { MockGame2048ViewModel() }
            vm.finishedGame.value = FinishedGame(SuccessType.REACHED_TARGET, reward = 1)
            Game2048GameFinishedDialog(vm, rememberNavController())
        }
    }
}