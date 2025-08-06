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
import androidx.compose.runtime.MutableState
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
import com.vanbrusselgames.mindmix.core.ui.DialogButton
import com.vanbrusselgames.mindmix.feature.gamefinished.Buttons
import com.vanbrusselgames.mindmix.feature.gamefinished.GameFinishedRewardRow
import com.vanbrusselgames.mindmix.feature.gamefinished.StatRow
import com.vanbrusselgames.mindmix.feature.gamefinished.Stats
import com.vanbrusselgames.mindmix.games.game2048.R
import com.vanbrusselgames.mindmix.games.game2048.model.FinishedGame
import com.vanbrusselgames.mindmix.games.game2048.viewmodel.IGame2048ViewModel
import com.vanbrusselgames.mindmix.games.game2048.viewmodel.MockGame2048ViewModel

@Composable
fun Game2048GameFinishedDialog(
    navController: NavController,
    viewModel: IGame2048ViewModel,
    checkAdLoaded: (adLoaded: MutableState<Boolean>) -> Unit,
    showAd: (adLoaded: MutableState<Boolean>, onAdWatched: (Int) -> Unit) -> Unit,
    backToMenu: () -> Unit,
    forceSave: () -> Unit
) {
    Column(Modifier.padding(16.dp), horizontalAlignment = Alignment.CenterHorizontally) {
        Text(
            stringResource(FinishedGame.titleResId),
            fontSize = 25.sp,
            fontWeight = FontWeight.ExtraBold
        )
        Spacer(Modifier.height(2.dp))
        Text(
            when (FinishedGame.textResId) {
                R.string.game_2048_reach_target_text -> reachedTargetText(
                    FinishedGame.targetTile, FinishedGame.score
                )

                R.string.game_2048_game_over_text -> gameOverText(FinishedGame.targetTile)
                R.string.game_2048_success -> successText(FinishedGame.targetTile)
                else -> {
                    stringResource(FinishedGame.textResId)
                }
            }, Modifier.width(IntrinsicSize.Max), textAlign = TextAlign.Center
        )

        Spacer(Modifier.height(8.dp))
        Stats {
            StatRow(
                fieldText = stringResource(R.string.game_2048_score_label),
                valueText = FinishedGame.score.toString()
            )
            StatRow(
                fieldText = stringResource(R.string.game_2048_stat_max_tile_label),
                valueText = FinishedGame.highestTileValue.toString()
            )
        }

        if (FinishedGame.reward != 0) {
            Spacer(Modifier.height(8.dp))
            GameFinishedRewardRow(FinishedGame.reward, checkAdLoaded, showAd, forceSave)
        }
        Spacer(Modifier.height(8.dp))
        if (FinishedGame.isStuck) {
            Buttons(
                navController,
                Modifier.fillMaxWidth(),
                null,
                { viewModel.startNewGame() },
                backToMenu
            )
        } else {
            DialogButton({ navController.popBackStack() }, Modifier.fillMaxWidth()) {
                Text(stringResource(R.string.game_2048_continue_game))
            }
        }
    }
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
            Game2048GameFinishedDialog(
                rememberNavController(),
                vm,
                {},
                { a, b -> },
                {}) {}
        }
    }
}