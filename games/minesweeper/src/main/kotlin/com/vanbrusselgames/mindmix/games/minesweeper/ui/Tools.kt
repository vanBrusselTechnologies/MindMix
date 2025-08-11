package com.vanbrusselgames.mindmix.games.minesweeper.ui

import androidx.compose.foundation.Image
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonColors
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.MutableIntState
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.vanbrusselgames.mindmix.games.minesweeper.R
import com.vanbrusselgames.mindmix.games.minesweeper.model.InputMode
import com.vanbrusselgames.mindmix.games.minesweeper.viewmodel.IMinesweeperViewModel

@Composable
fun Tools(viewModel: IMinesweeperViewModel, isHorizontal: Boolean) {
    if (isHorizontal) {
        Row(Modifier.fillMaxSize(), verticalAlignment = Alignment.CenterVertically) {
            ToolsContent(viewModel, Modifier.weight(2f), Modifier.weight(1f))
        }
    } else {
        Column(Modifier.fillMaxSize(), horizontalAlignment = Alignment.CenterHorizontally) {
            ToolsContent(viewModel, Modifier.weight(2f), Modifier.weight(1f))
        }
    }
}

@Composable
private fun ToolsContent(
    viewModel: IMinesweeperViewModel, modifier: Modifier, spacerModifier: Modifier
) {
    Spacer(spacerModifier)
    MinesLeftText(modifier, viewModel.minesLeft)
    ChangeInputModeButton(viewModel, modifier)
    Spacer(spacerModifier)
}

@Composable
private fun MinesLeftText(modifier: Modifier, minesLeft: MutableIntState) {
    Text(
        text = "${stringResource(R.string.mines_left)}\n${minesLeft.intValue}",
        modifier = modifier,
        textAlign = TextAlign.Center,
        fontSize = 22.5.sp
    )
}

@Composable
private fun ChangeInputModeButton(viewModel: IMinesweeperViewModel, modifier: Modifier) {
    val colors = ButtonColors(
        MaterialTheme.colorScheme.secondaryContainer,
        MaterialTheme.colorScheme.onSecondaryContainer,
        MaterialTheme.colorScheme.secondaryContainer,
        MaterialTheme.colorScheme.onSecondaryContainer
    )
    Button(
        { viewModel.changeInputMode() },
        modifier.aspectRatio(1f),
        shape = RoundedCornerShape(10.dp),
        colors = colors
    ) {
        if (viewModel.inputMode.value == InputMode.Flag) {
            Icon(
                painterResource(R.drawable.baseline_flag_24),
                "inputType: Flag",
                Modifier.fillMaxSize()
            )
        } else Image(painterResource(R.drawable.spade), "inputType: Spade")
    }
}