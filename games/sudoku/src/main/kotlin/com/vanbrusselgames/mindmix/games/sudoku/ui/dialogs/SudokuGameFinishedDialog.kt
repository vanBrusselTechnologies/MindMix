package com.vanbrusselgames.mindmix.games.sudoku.ui.dialogs

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
import com.vanbrusselgames.mindmix.feature.gamefinished.Buttons
import com.vanbrusselgames.mindmix.feature.gamefinished.GameFinishedRewardRow
import com.vanbrusselgames.mindmix.games.sudoku.model.FinishedGame
import com.vanbrusselgames.mindmix.games.sudoku.viewmodel.ISudokuViewModel
import com.vanbrusselgames.mindmix.games.sudoku.viewmodel.MockSudokuViewModel

@Composable
fun SudokuGameFinishedDialog(
    navController: NavController,
    viewModel: ISudokuViewModel,
    checkAdLoaded: (adLoaded: MutableState<Boolean>) -> Unit,
    showAd: (adLoaded: MutableState<Boolean>, onAdWatched: (Int) -> Unit) -> Unit,
    backToMenu: () -> Unit,
    forceSave: () -> Unit
) {
    Column(
        Modifier
            .padding(16.dp)
            .width(IntrinsicSize.Max),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Text(
            stringResource(FinishedGame.titleResId),
            fontSize = 25.sp,
            fontWeight = FontWeight.ExtraBold
        )
        Spacer(Modifier.height(2.dp))
        Text(stringResource(FinishedGame.textResId), textAlign = TextAlign.Center)
        if (FinishedGame.reward != 0) {
            Spacer(Modifier.height(8.dp))
            GameFinishedRewardRow(FinishedGame.reward, checkAdLoaded, showAd, forceSave)
        }
        Spacer(Modifier.height(8.dp))
        Buttons(
            navController, Modifier.fillMaxWidth(), null, { viewModel.startNewGame() }, backToMenu
        )
    }
}

@PreviewLightDark
@Composable
fun Prev_GameFinished() {
    MindMixTheme {
        Surface {
            val vm = remember { MockSudokuViewModel() }
            SudokuGameFinishedDialog(rememberNavController(), vm, {}, { a, b -> }, {}) {}
        }
    }
}