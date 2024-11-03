package com.vanbrusselgames.mindmix.games.minesweeper

import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.IntrinsicSize
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.heightIn
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Button
import androidx.compose.material3.Checkbox
import androidx.compose.material3.CheckboxDefaults
import androidx.compose.material3.MaterialTheme.colorScheme
import androidx.compose.material3.Surface
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.tooling.preview.PreviewLightDark
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.vanbrusselgames.mindmix.core.designsystem.theme.MindMixTheme

@Composable
fun MinesweeperSettings(viewModel: GameViewModel) {
    Column(
        Modifier
            .padding(24.dp)
            .width(IntrinsicSize.Max),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        val ctx = LocalContext.current
        Text(
            text = stringResource(Minesweeper.NAME_RES_ID),
            Modifier.fillMaxWidth(),
            fontSize = 36.sp,
            fontWeight = FontWeight.ExtraBold,
            textAlign = TextAlign.Center
        )
        Spacer(Modifier.height(20.dp))

        val autoFlagTicked = remember { mutableStateOf(viewModel.autoFlag) }
        Button(
            { autoFlagTicked.value = !autoFlagTicked.value },
            Modifier.fillMaxWidth(),
            shape = RoundedCornerShape(6.dp)
        ) {
            Row(Modifier.heightIn(max = 36.dp), verticalAlignment = Alignment.CenterVertically) {
                Text(stringResource(R.string.auto_flag))
                Spacer(Modifier.weight(1f))
                Checkbox(
                    checked = autoFlagTicked.value,
                    onCheckedChange = {
                        autoFlagTicked.value = it
                        viewModel.autoFlag = it
                        //todo: savePreferences(ctx, PREF_KEY_MINESWEEPER__AUTO_FLAG, it)
                        if (it) viewModel.autoFlag()
                    },
                    colors = CheckboxDefaults.colors()
                        .copy(uncheckedBorderColor = colorScheme.onPrimary)
                )
            }
        }

        Spacer(Modifier.height(2.dp))

        val safeStartTicked = remember { mutableStateOf(viewModel.safeStart) }
        Button(
            { safeStartTicked.value = !safeStartTicked.value },
            Modifier.fillMaxWidth(),
            shape = RoundedCornerShape(6.dp)
        ) {
            Row(Modifier.heightIn(max = 36.dp), verticalAlignment = Alignment.CenterVertically) {
                Text(stringResource(R.string.safe_area))
                Spacer(Modifier.weight(1f))
                Checkbox(
                    checked = safeStartTicked.value,
                    onCheckedChange = {
                        safeStartTicked.value = it
                        viewModel.safeStart = it
                        //todo: savePreferences(ctx, PREF_KEY_MINESWEEPER__SAFE_START, it)
                    },
                    colors = CheckboxDefaults.colors()
                        .copy(uncheckedBorderColor = colorScheme.onPrimary)
                )
            }
        }
    }
}

@PreviewLightDark
@Composable
fun Prev_Settings() {
    MindMixTheme {
        Surface {
            MinesweeperSettings(GameViewModel())
        }
    }
}