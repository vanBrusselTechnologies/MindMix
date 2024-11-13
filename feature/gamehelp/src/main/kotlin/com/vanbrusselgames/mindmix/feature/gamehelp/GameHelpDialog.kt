package com.vanbrusselgames.mindmix.feature.gamehelp

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.sizeIn
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Close
import androidx.compose.material3.Card
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.Surface
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.alpha
import androidx.compose.ui.platform.LocalConfiguration
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

@Composable
fun GameHelpDialog(navController: NavController, titleId: Int, descriptionId: Int) {
    val localConfig = LocalConfiguration.current
    val screenWidth = localConfig.screenWidthDp.dp
    val screenHeight = localConfig.screenHeightDp.dp
    Card(
        Modifier
            .sizeIn(maxWidth = screenWidth * 0.95f, maxHeight = screenHeight * 0.95f)
            .alpha(0.9f)
    ) {
        Box(Modifier.align(Alignment.CenterHorizontally)) {
            IconButton({ navController.popBackStack() }, Modifier.align(Alignment.TopEnd)) {
                Icon(Icons.Default.Close, contentDescription = "Close")
            }
            Column(Modifier.padding(16.dp), horizontalAlignment = Alignment.CenterHorizontally) {
                Text(
                    text = stringResource(titleId),
                    fontSize = 32.sp,
                    fontWeight = FontWeight.ExtraBold
                )
                Spacer(Modifier.height(16.dp))
                Text(text = stringResource(descriptionId), textAlign = TextAlign.Center)
            }
        }
    }
}

@Preview(locale = "nl")
@PreviewLightDark
@Composable
fun Prev_GameHelp() {
    MindMixTheme {
        Surface {
            GameHelpDialog(rememberNavController(), 0, 0)
        }
    }
}