package com.vanbrusselgames.mindmix.games.game2048

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
import com.vanbrusselgames.mindmix.core.advertisement.AdManager
import com.vanbrusselgames.mindmix.core.designsystem.theme.MindMixTheme
import com.vanbrusselgames.mindmix.core.ui.DialogButton
import com.vanbrusselgames.mindmix.feature.gamefinished.Buttons
import com.vanbrusselgames.mindmix.feature.gamefinished.GameFinishedRewardRow
import com.vanbrusselgames.mindmix.feature.gamefinished.StatRow
import com.vanbrusselgames.mindmix.feature.gamefinished.Stats

@Composable
fun Game2048GameFinishedDialog(
    navController: NavController,
    viewModel: GameViewModel,
    adManager: (() -> AdManager)?,
    backToMenu: () -> Unit
) {
    Column(Modifier.padding(16.dp), horizontalAlignment = Alignment.CenterHorizontally) {
        Text(
            stringResource(FinishedGame.titleResId),
            fontSize = 25.sp,
            fontWeight = FontWeight.ExtraBold
        )
        Spacer(Modifier.height(2.dp))
        Text(
            stringResource(FinishedGame.textResId),
            Modifier.width(IntrinsicSize.Max),
            textAlign = TextAlign.Center
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

        if (FinishedGame.reward != 0 && adManager != null) {
            Spacer(Modifier.height(8.dp))
            GameFinishedRewardRow(FinishedGame.reward, adManager)
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

@PreviewLightDark
@Composable
fun Prev_GameFinished() {
    MindMixTheme {
        Surface {
            Game2048GameFinishedDialog(rememberNavController(), GameViewModel(), null) {}
        }
    }
}