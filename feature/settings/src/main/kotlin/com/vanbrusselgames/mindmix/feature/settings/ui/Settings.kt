package com.vanbrusselgames.mindmix.feature.settings.ui

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
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
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme.colorScheme
import androidx.compose.material3.Surface
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.State
import androidx.compose.runtime.getValue
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalUriHandler
import androidx.compose.ui.res.painterResource
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
import com.vanbrusselgames.mindmix.core.designsystem.theme.SelectedTheme
import com.vanbrusselgames.mindmix.core.ui.EnumDropdown
import com.vanbrusselgames.mindmix.core.ui.dialogs.settings.SettingsDialog
import com.vanbrusselgames.mindmix.feature.settings.R
import com.vanbrusselgames.mindmix.feature.settings.viewmodel.ISettingsViewModel
import com.vanbrusselgames.mindmix.feature.settings.viewmodel.MockSettingsViewModel

private const val DATA_DELETION_URL =
    "https://docs.google.com/forms/d/e/1FAIpQLSf-_Yo6db7aYUt3qcEBq-rxCgjVCN1uVcWeeZ_oXQyZH8DIyQ/viewform?usp=pp_url&entry.2052602114="

@Composable
fun MainSettings(
    viewModel: ISettingsViewModel, navController: NavController, onClickSignIn: () -> Unit
) {
    SettingsDialog(navController) {
        Column(
            Modifier
                .padding(24.dp)
                .width(IntrinsicSize.Max),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            Text(
                text = stringResource(R.string.settings),
                Modifier.fillMaxWidth(),
                fontSize = 36.sp,
                fontWeight = FontWeight.ExtraBold,
                textAlign = TextAlign.Center
            )
            Spacer(Modifier.height(20.dp))
            val loadedState = viewModel.preferencesLoaded.collectAsStateWithLifecycle()
            if (!loadedState.value) return@SettingsDialog
            ThemeDropdownRow(viewModel)
            Spacer(Modifier.height(4.dp))
            SignInButton(viewModel.signedIn, onClickSignIn)
            DeleteDataButton { viewModel.userId.value }
            Spacer(Modifier.height(8.dp))
            Text(
                "User ID: ${viewModel.userId.value}",
                Modifier.fillMaxWidth(),
                fontSize = 10.sp,
                textAlign = TextAlign.Center
            )
        }
    }
}

@Composable
fun ThemeDropdownRow(viewModel: ISettingsViewModel) {
    val defaultButtonColors = ButtonDefaults.buttonColors()
    Row(
        Modifier
            .fillMaxWidth()
            .background(defaultButtonColors.containerColor, RoundedCornerShape(6.dp)),
        Arrangement.Center,
        Alignment.CenterVertically
    ) {
        Text(
            stringResource(R.string.theme),
            Modifier.padding(20.dp),
            defaultButtonColors.contentColor
        )
        Spacer(Modifier.weight(1f))
        val modifier = Modifier
            .padding(4.dp)
            .width(IntrinsicSize.Min)
        val callback = { theme: SelectedTheme -> viewModel.setTheme(theme) }
        EnumDropdown(modifier, viewModel.theme, callback, SelectedTheme.entries.toList())
    }
}

@Composable
fun SignInButton(signedInState: State<Boolean>, onClickSignIn: () -> Unit) {
    val signedIn by signedInState
    Button(
        onClickSignIn, Modifier.fillMaxWidth(), enabled = !signedIn, RoundedCornerShape(6.dp)
    ) {
        Row(Modifier.heightIn(max = 36.dp), verticalAlignment = Alignment.CenterVertically) {
            Icon(painterResource(R.drawable.games_controller), "Play Games Sign-In")
            Spacer(Modifier.width(8.dp))
            Text(stringResource(if (signedIn) R.string.connected else R.string.signin))
        }
    }
}

@Composable
fun DeleteDataButton(getUserId: () -> String) {
    val uriHandler = LocalUriHandler.current
    Button(
        { uriHandler.openUri("$DATA_DELETION_URL${getUserId()}") },
        Modifier.fillMaxWidth(),
        colors = ButtonDefaults.buttonColors(colorScheme.error, colorScheme.onError),
        shape = RoundedCornerShape(6.dp)
    ) { Text(stringResource(R.string.request_data_deletion)) }
}

@PreviewLightDark
@Composable
private fun PrevSettings() {
    MindMixTheme {
        Surface {
            val vm = remember { MockSettingsViewModel() }
            MainSettings(vm, rememberNavController()) {}
        }
    }
}