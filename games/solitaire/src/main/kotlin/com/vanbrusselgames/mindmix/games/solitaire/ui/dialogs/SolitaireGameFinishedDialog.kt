package com.vanbrusselgames.mindmix.games.solitaire.ui.dialogs

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
import androidx.compose.runtime.remember
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
import com.vanbrusselgames.mindmix.core.designsystem.theme.MindMixTheme
import com.vanbrusselgames.mindmix.core.navigation.navigateToMenu
import com.vanbrusselgames.mindmix.core.ui.dialogs.gamefinished.Buttons
import com.vanbrusselgames.mindmix.core.ui.dialogs.gamefinished.GameFinishedDialog
import com.vanbrusselgames.mindmix.core.ui.dialogs.gamefinished.StatRow
import com.vanbrusselgames.mindmix.core.ui.dialogs.gamefinished.Stats
import com.vanbrusselgames.mindmix.games.solitaire.R
import com.vanbrusselgames.mindmix.games.solitaire.model.FinishedGame
import com.vanbrusselgames.mindmix.games.solitaire.viewmodel.ISolitaireViewModel
import com.vanbrusselgames.mindmix.games.solitaire.viewmodel.MockSolitaireViewModel
import kotlin.time.Duration.Companion.milliseconds

@Composable
fun SolitaireGameFinishedDialog(viewModel: ISolitaireViewModel, navController: NavController) {
    GameFinishedDialog {
        Column(
            Modifier
                .padding(16.dp)
                .width(IntrinsicSize.Min),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            val finishedGame = viewModel.finishedGame.value

            GameFinishedDialogTitle()
            Spacer(Modifier.height(2.dp))
            GameFinishedDialogText()
            Spacer(Modifier.height(8.dp))
            Stats {
                StatRow(
                    fieldText = stringResource(R.string.game_soltaire_label_moves),
                    valueText = finishedGame.moves.toString()
                )
                StatRow(
                    fieldText = stringResource(R.string.game_solitaire_label_current_time),
                    valueText = formatDuration(finishedGame.usedMillis)
                )
                if (finishedGame.penaltyMillis > 0) {
                    StatRow(
                        fieldText = stringResource(R.string.game_soltaire_label_penalty_time),
                        valueText = formatDuration(finishedGame.penaltyMillis)
                    )
                }
                if (finishedGame.isNewRecord) {
                    Box {
                        StatRow(
                            fieldText = stringResource(R.string.game_solitaire_label_fastest_time),
                            valueText = formatDuration(finishedGame.usedMillis + finishedGame.penaltyMillis)
                        )
                        Text(
                            stringResource(R.string.game_solitaire_new_record),
                            Modifier
                                .rotate(-16.25f)
                                .align(Alignment.CenterEnd),
                            Color.Red,
                            fontSize = 13.5.sp,
                            fontWeight = FontWeight.ExtraBold
                        )
                    }
                }
                if (finishedGame.lastRecordMillis != -1L) StatRow(
                    fieldText = stringResource(if (finishedGame.isNewRecord) R.string.game_solitaire_label_last_best else R.string.game_solitaire_label_fastest_time),
                    valueText = formatDuration(finishedGame.lastRecordMillis)
                )
            }

            Spacer(Modifier.height(8.dp))
            Buttons(
                navController,
                Modifier.fillMaxWidth(),
                null,
                { viewModel.startNewGame() }) { navController.navigateToMenu() }
        }
    }
}

@Composable
private fun GameFinishedDialogTitle() {
    Text(
        stringResource(R.string.solitaire_name), // "Congrats / Smart / Well done"
        fontSize = 25.sp, fontWeight = FontWeight.ExtraBold
    )
}

@Composable
private fun GameFinishedDialogText() {
    Text(
        stringResource(R.string.solitaire_success),
        // """You did great and solved puzzle in ${0} seconds!!
        //     |That's Awesome!
        //     |Share with your friends and challenge them to beat your time!""".trimMargin()
        Modifier.width(IntrinsicSize.Max), textAlign = TextAlign.Center
    )
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
            val vm = remember { MockSolitaireViewModel() }
            vm.finishedGame.value = FinishedGame(1, 10000000, 0, 9999999, true)
            SolitaireGameFinishedDialog(vm, rememberNavController())
        }
    }
}