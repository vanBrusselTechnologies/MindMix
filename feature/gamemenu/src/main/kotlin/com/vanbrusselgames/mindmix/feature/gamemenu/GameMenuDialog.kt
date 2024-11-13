package com.vanbrusselgames.mindmix.feature.gamemenu

import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.IntrinsicSize
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.sizeIn
import androidx.compose.foundation.layout.width
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Home
import androidx.compose.material.icons.filled.PlayArrow
import androidx.compose.material.icons.filled.Settings
import androidx.compose.material3.Card
import androidx.compose.material3.Icon
import androidx.compose.material3.Surface
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.alpha
import androidx.compose.ui.graphics.painter.Painter
import androidx.compose.ui.graphics.vector.ImageVector
import androidx.compose.ui.platform.LocalConfiguration
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.tooling.preview.PreviewLightDark
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.navigation.NavController
import androidx.navigation.compose.rememberNavController
import com.vanbrusselgames.mindmix.core.designsystem.theme.MindMixTheme
import com.vanbrusselgames.mindmix.core.ui.DialogButton

@Composable
fun GameMenuDialog(
    navController: NavController,
    gameNameId: Int,
    startNewGame: () -> Unit,
    openSettings: () -> Unit,
    backToMenu: () -> Unit
) {
    val localConfig = LocalConfiguration.current
    val screenWidth = localConfig.screenWidthDp.dp
    val screenHeight = localConfig.screenHeightDp.dp
    Card(
        Modifier
            .sizeIn(maxWidth = screenWidth * 0.95f, maxHeight = screenHeight * 0.95f)
            .alpha(0.9f)
    ) {
        Column(
            Modifier
                .padding(16.dp)
                .align(Alignment.CenterHorizontally)
                .width(intrinsicSize = IntrinsicSize.Max),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            Text(
                text = stringResource(gameNameId),
                fontSize = 36.sp,
                fontWeight = FontWeight.ExtraBold
            )
            Spacer(Modifier.height(10.dp))
            GameMenuDialogButton(R.string.continue_game, Icons.Default.PlayArrow, "Continue") {
                navController.popBackStack()
            }
            GameMenuDialogButton(
                R.string.play_again,
                painterResource(R.drawable.outline_autorenew_24),
                "Start new game"
            ) {
                startNewGame()
                navController.popBackStack()
            }
            GameMenuDialogButton(
                R.string.settings, Icons.Default.Settings, "Settings", openSettings
            )
            GameMenuDialogButton(
                R.string.back_to_menu, Icons.Default.Home, "Back to menu", backToMenu
            )
        }
    }
}

@Composable
private fun GameMenuDialogButton(
    textId: Int, iconVector: ImageVector, iconContentDescription: String, onClick: () -> Unit
) {
    GameMenuDialogButton(textId, onClick) { Icon(iconVector, iconContentDescription) }
}

@Composable
private fun GameMenuDialogButton(
    textId: Int, iconPainter: Painter, iconContentDescription: String, onClick: () -> Unit
) {
    GameMenuDialogButton(textId, onClick) { Icon(iconPainter, iconContentDescription) }
}

@Composable
private fun GameMenuDialogButton(textId: Int, onClick: () -> Unit, icon: @Composable () -> Unit) {
    DialogButton(onClick, Modifier.fillMaxWidth()) {
        icon()
        Spacer(Modifier.width(8.dp))
        Text(stringResource(textId), Modifier.weight(1f), textAlign = TextAlign.Center)
    }
}

@Preview(locale = "nl")
@PreviewLightDark
@Composable
fun Prev_GameMenu() {
    MindMixTheme {
        Surface {
            GameMenuDialog(rememberNavController(), R.string.settings, {}, {}, {})
        }
    }
}