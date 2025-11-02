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
import com.vanbrusselgames.mindmix.core.ui.dialogs.gamefinished.Buttons
import com.vanbrusselgames.mindmix.core.ui.dialogs.gamefinished.GameFinishedDialog
import com.vanbrusselgames.mindmix.games.sudoku.R
import com.vanbrusselgames.mindmix.games.sudoku.model.FinishedGame
import com.vanbrusselgames.mindmix.games.sudoku.viewmodel.ISudokuViewModel
import com.vanbrusselgames.mindmix.games.sudoku.viewmodel.MockSudokuViewModel

@Composable
fun SudokuGameFinishedDialog(viewModel: ISudokuViewModel, navController: NavController) {
    GameFinishedDialog {
        Column(
            Modifier
                .padding(16.dp)
                .width(IntrinsicSize.Min),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            viewModel.finishedGame.value

            GameFinishedDialogTitle()
            Spacer(Modifier.height(2.dp))
            GameFinishedDialogText()
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
        stringResource(R.string.sudoku_name), // "Congrats / Smart / Well done"
        fontSize = 25.sp, fontWeight = FontWeight.ExtraBold
    )
}

@Composable
private fun GameFinishedDialogText() {
    Text(
        stringResource(R.string.sudoku_success),
        // """You did great and solved puzzle in ${0} seconds!!
        //     |That's Awesome!
        //     |Share with your friends and challenge them to beat your time!""".trimMargin()
        Modifier.width(IntrinsicSize.Max), textAlign = TextAlign.Center
    )
}

@PreviewLightDark
@Composable
fun Prev_GameFinished() {
    MindMixTheme {
        Surface {
            val vm = remember { MockSudokuViewModel() }
            vm.finishedGame.value = FinishedGame(1)
            SudokuGameFinishedDialog(vm, rememberNavController())
        }
    }
}