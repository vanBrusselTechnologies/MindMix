package com.vanbrusselgames.mindmix.games.minesweeper.ui.dialogs

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
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.tooling.preview.PreviewLightDark
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import androidx.navigation.NavController
import androidx.navigation.compose.rememberNavController
import com.vanbrusselgames.mindmix.core.designsystem.theme.MindMixTheme
import com.vanbrusselgames.mindmix.feature.settings.SettingsDialog
import com.vanbrusselgames.mindmix.games.minesweeper.R
import com.vanbrusselgames.mindmix.games.minesweeper.model.Minesweeper
import com.vanbrusselgames.mindmix.games.minesweeper.viewmodel.IMinesweeperViewModel
import com.vanbrusselgames.mindmix.games.minesweeper.viewmodel.MockMinesweeperViewModel

@Composable
fun MinesweeperSettingsDialog(viewModel: IMinesweeperViewModel, navController: NavController) {
    SettingsDialog(navController) {
        Column(
            Modifier
                .padding(24.dp)
                .width(IntrinsicSize.Max),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            Text(
                text = stringResource(R.string.minesweeper_name),
                Modifier.fillMaxWidth(),
                fontSize = 36.sp,
                fontWeight = FontWeight.ExtraBold,
                textAlign = TextAlign.Center
            )
            val loadedState = viewModel.preferencesLoaded.collectAsStateWithLifecycle()
            if (!loadedState.value) return@SettingsDialog
            Spacer(Modifier.height(20.dp))
            Button(
                { viewModel.onClickUpdateAutoFlag() },
                Modifier.fillMaxWidth(),
                shape = RoundedCornerShape(6.dp)
            ) {
                Row(
                    Modifier.heightIn(max = 36.dp), verticalAlignment = Alignment.CenterVertically
                ) {
                    Text(stringResource(R.string.auto_flag))
                    Spacer(Modifier.weight(1f))
                    Checkbox(
                        checked = viewModel.autoFlag.value,
                        onCheckedChange = { viewModel.onClickUpdateAutoFlag() },
                        colors = CheckboxDefaults.colors()
                            .copy(uncheckedBorderColor = colorScheme.onPrimary)
                    )
                }
            }

            Spacer(Modifier.height(2.dp))

            Button(
                { viewModel.onClickUpdateSafeStart() },
                Modifier.fillMaxWidth(),
                shape = RoundedCornerShape(6.dp)
            ) {
                Row(
                    Modifier.heightIn(max = 36.dp), verticalAlignment = Alignment.CenterVertically
                ) {
                    Text(stringResource(R.string.safe_area))
                    Spacer(Modifier.weight(1f))
                    Checkbox(
                        checked = viewModel.safeStart.value,
                        onCheckedChange = {
                            viewModel.onClickUpdateSafeStart()
                        },
                        colors = CheckboxDefaults.colors()
                            .copy(uncheckedBorderColor = colorScheme.onPrimary)
                    )
                }
            }
        }
    }
}

@PreviewLightDark
@Composable
fun Prev_Settings() {
    MindMixTheme {
        Surface {
            val vm = remember { MockMinesweeperViewModel() }
            MinesweeperSettingsDialog(vm, rememberNavController())
        }
    }
}