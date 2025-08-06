package com.vanbrusselgames.mindmix.games.game2048.ui.dialogs

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.IntrinsicSize
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.ButtonDefaults
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
import com.vanbrusselgames.mindmix.core.ui.EnumDropdown
import com.vanbrusselgames.mindmix.feature.settings.SettingsDialog
import com.vanbrusselgames.mindmix.games.game2048.R
import com.vanbrusselgames.mindmix.games.game2048.model.Game2048
import com.vanbrusselgames.mindmix.games.game2048.model.GridSize2048
import com.vanbrusselgames.mindmix.games.game2048.viewmodel.IGame2048ViewModel
import com.vanbrusselgames.mindmix.games.game2048.viewmodel.MockGame2048ViewModel

@Composable
fun Game2048SettingsDialog(viewModel: IGame2048ViewModel, navController: NavController) {
    SettingsDialog(navController) {
        Column(
            Modifier
                .padding(24.dp)
                .width(IntrinsicSize.Max),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            Text(
                text = stringResource(Game2048.NAME_RES_ID),
                Modifier.fillMaxWidth(),
                fontSize = 36.sp,
                fontWeight = FontWeight.ExtraBold,
                textAlign = TextAlign.Center
            )
            val loadedState = viewModel.preferencesLoaded.collectAsStateWithLifecycle()
            if (!loadedState.value) return@SettingsDialog
            Spacer(Modifier.height(20.dp))
            SizeDropdownRow(viewModel)
        }
    }
}

@Composable
fun SizeDropdownRow(viewModel: IGame2048ViewModel) {
    val defaultButtonColors = ButtonDefaults.buttonColors()
    Row(
        Modifier
            .fillMaxWidth()
            .background(defaultButtonColors.containerColor, RoundedCornerShape(6.dp)),
        Arrangement.Center,
        Alignment.CenterVertically
    ) {
        Text(
            stringResource(R.string.game_2048_size_label),
            Modifier.padding(20.dp),
            defaultButtonColors.contentColor
        )
        Spacer(Modifier.weight(1f))
        val modifier = Modifier
            .padding(4.dp)
            .width(IntrinsicSize.Min)
        val callback = { size: GridSize2048 -> viewModel.setSize(size) }
        EnumDropdown(modifier, viewModel.gridSize, callback, GridSize2048.entries.toList())
    }
}

@PreviewLightDark
@Composable
fun Prev_Settings() {
    MindMixTheme {
        Surface {
            val vm = remember { MockGame2048ViewModel() }
            Game2048SettingsDialog(vm, rememberNavController())
        }
    }
}