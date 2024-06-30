package com.vanbrusselgames.mindmix.menu

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.PaddingValues
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.offset
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.rounded.PlayArrow
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Surface
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.blur
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.AnnotatedString
import androidx.compose.ui.text.ParagraphStyle
import androidx.compose.ui.text.style.LineHeightStyle
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.tooling.preview.PreviewScreenSizes
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.vanbrusselgames.mindmix.BaseLayout
import com.vanbrusselgames.mindmix.R
import com.vanbrusselgames.mindmix.SceneManager
import com.vanbrusselgames.mindmix.games.minesweeper.MinesweeperSettings
import com.vanbrusselgames.mindmix.games.solitaire.SolitaireSettings
import com.vanbrusselgames.mindmix.games.sudoku.SudokuSettings
import com.vanbrusselgames.mindmix.menu.MenuManager.Instance.wheelModel
import com.vanbrusselgames.mindmix.ui.theme.MindMixTheme

class MenuLayout : BaseLayout() {
    @Composable
    fun Scene() {
        BaseScene(true) {
            SetLayoutGameWheel()
            when (MenuManager.settingsGame.value) {
                SceneManager.Scene.MENU -> MenuSettings()
                SceneManager.Scene.MINESWEEPER -> MinesweeperSettings()
                SceneManager.Scene.SOLITAIRE -> SolitaireSettings()
                SceneManager.Scene.SUDOKU -> SudokuSettings()
            }
        }
    }

    @Composable
    fun SetLayoutGameWheel() {
        Box(
            Modifier
                .fillMaxSize()
                .blur(if (activeOverlapUI.value) 6.dp else 0.dp),
            Alignment.BottomCenter
        ) {
            GameWheel(wheelModel)
            PlayButton(Modifier.align(Alignment.BottomCenter))
        }
    }

    @Composable
    fun PlayButton(modifier: Modifier) {
        Button(
            onClick = { SceneManager.loadScene(MenuManager.selectedGame) },
            modifier = modifier.offset(0.dp, (-15).dp),
            enabled = !activeOverlapUI.value,
            colors = ButtonDefaults.buttonColors(
                containerColor = MaterialTheme.colorScheme.primary,
                contentColor = MaterialTheme.colorScheme.onPrimary,
                disabledContainerColor = MaterialTheme.colorScheme.primary,
                disabledContentColor = MaterialTheme.colorScheme.onPrimary,
            )
        ) {
            Row(verticalAlignment = Alignment.CenterVertically) {
                Icon(
                    Icons.Rounded.PlayArrow,
                    "Play",
                    tint = MaterialTheme.colorScheme.onPrimary,
                )
                Text(
                    text = AnnotatedString(
                        text = stringResource(R.string.play), paragraphStyle = ParagraphStyle(
                            lineHeightStyle = LineHeightStyle(
                                alignment = LineHeightStyle.Alignment.Center,
                                trim = LineHeightStyle.Trim.Both
                            )
                        )
                    ),
                    color = MaterialTheme.colorScheme.onPrimary,
                    fontSize = 21.sp,
                    textAlign = TextAlign.Center,
                )
            }
        }
    }
}

@PreviewScreenSizes
@Composable
private fun PrevSettings() {
    MindMixTheme {
        Surface {
            MenuLayout().SetLayoutGameWheel()
        }
    }
}