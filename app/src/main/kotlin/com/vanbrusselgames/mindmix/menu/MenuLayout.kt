package com.vanbrusselgames.mindmix.menu

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.layout.size
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.PlayArrow
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.blur
import androidx.compose.ui.platform.LocalConfiguration
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.AnnotatedString
import androidx.compose.ui.text.ParagraphStyle
import androidx.compose.ui.text.style.LineHeightStyle
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.times
import com.vanbrusselgames.mindmix.AutoSizeText
import com.vanbrusselgames.mindmix.BaseLayout
import com.vanbrusselgames.mindmix.R
import com.vanbrusselgames.mindmix.Settings

class MenuLayout : BaseLayout() {

    @Composable
    fun Scene() {
        BaseScene(true) {
            SetLayoutGameWheel()
            MenuSettings()
        }
    }

    @Composable
    fun SetLayoutGameWheel() {
        Box(
            Modifier
                .fillMaxSize()
                .blur(if (Settings.visible.value) 6.dp else 0.dp),
            Alignment.BottomCenter
        ) {
            val localCurrentConfig = LocalConfiguration.current
            val screenHeight = localCurrentConfig.screenHeightDp.dp
            val screenWidth = localCurrentConfig.screenWidthDp.dp
            GameWheel(MenuManager.GAME_COUNT, screenWidth, screenHeight)
            PlayButton(Modifier.align(Alignment.BottomCenter), screenWidth, screenHeight)
        }
    }

    @Composable
    fun PlayButton(modifier: Modifier, screenWidth: Dp, screenHeight: Dp) {
        val playGameButtonSize = minOf(screenHeight / 400f, screenWidth / 250f)
        val iconSize = minOf(62.75f * playGameButtonSize * 0.4f, 25f * playGameButtonSize)
        Button(
            onClick = { if (!Settings.visible.value) MenuUIHandler.startGame(MenuManager.selectedGame) },
            modifier = modifier.offset(0.dp, screenHeight * -0.05f),
            colors = ButtonDefaults.buttonColors(
                containerColor = MaterialTheme.colorScheme.primary,
                contentColor = MaterialTheme.colorScheme.onPrimary
            )
        ) {
            Row(
                verticalAlignment = Alignment.CenterVertically, modifier = Modifier
            ) {
                Icon(
                    Icons.Filled.PlayArrow,
                    "Play",
                    tint = MaterialTheme.colorScheme.onPrimary,
                    modifier = Modifier
                        .size(iconSize)
                        .aspectRatio(1f)
                )
                AutoSizeText(
                    text = AnnotatedString(
                        text = stringResource(R.string.play), paragraphStyle = ParagraphStyle(
                            lineHeightStyle = LineHeightStyle(
                                alignment = LineHeightStyle.Alignment.Center,
                                trim = LineHeightStyle.Trim.Both
                            )
                        )
                    ),
                    textAlign = TextAlign.Center,
                    color = MaterialTheme.colorScheme.onPrimary,
                    modifier = Modifier.size(
                        62.75f * playGameButtonSize - iconSize * 2f / 3f, 25f * playGameButtonSize
                    )
                )
            }
        }
    }
}