package com.vanbrusselgames.mindmix.games.solitaire

import androidx.compose.foundation.layout.Box
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
import androidx.compose.ui.draw.rotate
import androidx.compose.ui.graphics.Color
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
import com.vanbrusselgames.mindmix.feature.gamefinished.Buttons
import com.vanbrusselgames.mindmix.feature.gamefinished.GameFinishedRewardRow
import com.vanbrusselgames.mindmix.feature.gamefinished.StatRow
import com.vanbrusselgames.mindmix.feature.gamefinished.Stats
import kotlin.time.Duration.Companion.milliseconds

@Composable
fun SolitaireGameFinishedDialog(
    navController: NavController,
    viewModel: GameViewModel,
    adManager: (() -> AdManager)?,
    backToMenu: () -> Unit
) {
    Column(
        Modifier
            .padding(16.dp)
            .width(IntrinsicSize.Min),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
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
            // TODO: Localization
            StatRow(fieldText = "Moves:", valueText = FinishedGame.moves.toString())
            StatRow(
                fieldText = "Current Time:", valueText = formatDuration(FinishedGame.usedMillis)
            )
            if (FinishedGame.penaltyMillis > 0) {
                StatRow(
                    fieldText = "Penalty Time:",
                    valueText = formatDuration(FinishedGame.penaltyMillis)
                )
            }
            if (FinishedGame.isNewRecord) {
                Box {
                    StatRow(
                        fieldText = "Fastest Time:",
                        valueText = formatDuration(FinishedGame.usedMillis + FinishedGame.penaltyMillis)
                    )
                    Text(
                        "New Best!",
                        Modifier
                            .rotate(-16.25f)
                            .align(Alignment.CenterEnd),
                        Color.Red,
                        13.5f.sp,
                        fontWeight = FontWeight.ExtraBold
                    )
                }
            }
            if (FinishedGame.lastRecordMillis != -1L) StatRow(
                fieldText = if (FinishedGame.isNewRecord) "Last Best Time:" else "Fastest Time:",
                valueText = formatDuration(FinishedGame.lastRecordMillis)
            )
        }

        if (FinishedGame.reward != 0 && adManager != null) {
            Spacer(Modifier.height(8.dp))
            GameFinishedRewardRow(FinishedGame.reward, adManager)
        }
        Spacer(Modifier.height(8.dp))
        Buttons(
            navController, Modifier.fillMaxWidth(), null, { viewModel.startNewGame() }, backToMenu
        )
    }
}

fun formatDuration(millis: Long): String {
    return millis.milliseconds.toComponents { m, s, n ->
        val minutes = if (m < 10) "0$m" else m.toString()
        val seconds = if (s < 10) "0$s" else s.toString()
        val mil = n / 1000000
        val millis = if (mil < 10) "00$mil" else if (mil < 100) "0$mil" else mil.toString()
        "$minutes:$seconds.${millis}"
    }
}

@PreviewLightDark
@Composable
fun Prev_GameFinished() {
    MindMixTheme {
        Surface {
            FinishedGame.usedMillis = 10000000
            FinishedGame.lastRecordMillis = 9999999
            FinishedGame.isNewRecord = false
            SolitaireGameFinishedDialog(rememberNavController(), GameViewModel(), null) {}
        }
    }
}