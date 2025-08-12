package com.vanbrusselgames.mindmix.feature.menu

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.offset
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.rounded.PlayArrow
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme.colorScheme
import androidx.compose.material3.Surface
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.blur
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.AnnotatedString
import androidx.compose.ui.text.ParagraphStyle
import androidx.compose.ui.text.style.LineHeightStyle
import androidx.compose.ui.text.style.LineHeightStyle.Trim
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.tooling.preview.PreviewScreenSizes
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.navigation.NavController
import androidx.navigation.compose.rememberNavController
import com.vanbrusselgames.mindmix.core.common.BaseScene
import com.vanbrusselgames.mindmix.core.designsystem.theme.MindMixTheme
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.feature.settings.navigation.navigateToSettings

@Composable
fun SceneUI(viewModel: MenuScreenViewModel, navController: NavController) {
    BaseScene(viewModel, {}, {}, { navController.navigateToSettings() }) {
        SetLayoutGameWheel(viewModel, navController)
    }
}

@Composable
fun SetLayoutGameWheel(viewModel: MenuScreenViewModel, navController: NavController) {
    Box(
        Modifier
            .fillMaxSize()
            .blur(if (SceneManager.dialogActiveState.value) 6.dp else 0.dp),
        Alignment.BottomCenter
    ) {
        GameWheel(viewModel, navController, viewModel.wheelModel)
        PlayButton(viewModel, navController, Modifier.align(Alignment.BottomCenter))
    }
}

@Composable
fun PlayButton(viewModel: MenuScreenViewModel, navController: NavController, modifier: Modifier) {
    Button(
        onClick = { viewModel.navigateToSelectedGame(navController) },
        modifier = modifier.offset(0.dp, (-15).dp),
        colors = ButtonDefaults.buttonColors(
            containerColor = colorScheme.primary,
            contentColor = colorScheme.onPrimary,
            disabledContainerColor = colorScheme.primary,
            disabledContentColor = colorScheme.onPrimary,
        )
    ) {
        Row(verticalAlignment = Alignment.CenterVertically) {
            Icon(
                Icons.Rounded.PlayArrow,
                "Play",
                tint = colorScheme.onPrimary,
            )
            Text(
                text = AnnotatedString(
                    text = stringResource(R.string.play), paragraphStyle = ParagraphStyle(
                        lineHeightStyle = LineHeightStyle(
                            alignment = LineHeightStyle.Alignment.Center, trim = Trim.Both
                        )
                    )
                ),
                color = colorScheme.onPrimary,
                fontSize = 21.sp,
                textAlign = TextAlign.Center,
            )
        }
    }
}

@PreviewScreenSizes
@Composable
private fun PrevSettings() {
    MindMixTheme {
        Surface {
            val vm = remember { MenuScreenViewModel() }
            SetLayoutGameWheel(vm, rememberNavController())
        }
    }
}