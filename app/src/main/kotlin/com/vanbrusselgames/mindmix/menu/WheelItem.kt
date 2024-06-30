package com.vanbrusselgames.mindmix.menu

import androidx.compose.animation.AnimatedVisibility
import androidx.compose.animation.core.LinearEasing
import androidx.compose.animation.core.animateFloatAsState
import androidx.compose.animation.core.tween
import androidx.compose.animation.expandIn
import androidx.compose.animation.fadeIn
import androidx.compose.animation.fadeOut
import androidx.compose.animation.shrinkOut
import androidx.compose.animation.slideInHorizontally
import androidx.compose.animation.slideOutHorizontally
import androidx.compose.foundation.Image
import androidx.compose.foundation.clickable
import androidx.compose.foundation.interaction.MutableInteractionSource
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.layout.widthIn
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Settings
import androidx.compose.material3.Card
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.draw.rotate
import androidx.compose.ui.platform.LocalConfiguration
import androidx.compose.ui.platform.LocalDensity
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.IntOffset
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.lerp
import androidx.compose.ui.unit.sp
import androidx.compose.ui.unit.times
import androidx.compose.ui.util.lerp
import com.vanbrusselgames.mindmix.BaseLayout.Companion.activeOverlapUI
import com.vanbrusselgames.mindmix.BaseUIHandler
import com.vanbrusselgames.mindmix.R
import com.vanbrusselgames.mindmix.SceneManager
import com.vanbrusselgames.mindmix.ui.theme.Typography
import com.vanbrusselgames.mindmix.utils.text.measureTextWidth
import kotlin.math.cos
import kotlin.math.sin

const val animDuration = 150
val easing = LinearEasing

data class WheelItem(val game: SceneManager.Scene) {
    var isSelected = mutableStateOf(false)
    var growthFactor = 0f
    var offsetY = 0f
    var angle = 0f
    var radius = 0f

    val title = when (game) {
        SceneManager.Scene.MENU -> -1
        SceneManager.Scene.MINESWEEPER -> R.string.minesweeper_name
        SceneManager.Scene.SOLITAIRE -> R.string.solitaire_name
        SceneManager.Scene.SUDOKU -> R.string.sudoku_name
    }

    val image = when (game) {
        SceneManager.Scene.MENU -> -1
        SceneManager.Scene.MINESWEEPER -> R.drawable.game_icon_minesweeper
        SceneManager.Scene.SOLITAIRE -> R.drawable.playingcards_detailed_clovers_a
        SceneManager.Scene.SUDOKU -> R.drawable.game_icon_sudoku
    }

    //todo?
    /*val enabledDifficulties: List<Difficulty> = when (game) {
        SceneManager.Scene.MENU -> listOf()
        SceneManager.Scene.MINESWEEPER -> listOf()
        SceneManager.Scene.SOLITAIRE -> listOf()
        SceneManager.Scene.SUDOKU -> com.vanbrusselgames.mindmix.games.sudoku.enabledDifficulties
    }

    val difficultyState = when (game) {
        SceneManager.Scene.MENU -> mutableStateOf(Difficulty.MEDIUM)
        SceneManager.Scene.MINESWEEPER -> mutableStateOf(Difficulty.MEDIUM)
        SceneManager.Scene.SOLITAIRE -> mutableStateOf(Difficulty.MEDIUM)
        SceneManager.Scene.SUDOKU -> SudokuManager.difficulty
    }

    val callback: (Difficulty) -> Unit = when (game) {
        SceneManager.Scene.MENU -> { _ -> }
        SceneManager.Scene.MINESWEEPER -> { _ -> }
        SceneManager.Scene.SOLITAIRE -> { _ -> }
        SceneManager.Scene.SUDOKU -> { difficulty -> SudokuManager.setDifficulty(difficulty) }
    }*/
}

@Composable
fun WheelItem(model: WheelItem, modifier: Modifier = Modifier) {
    val game = model.game
    val angle = model.angle
    val name = stringResource(model.title)

    val selectedFactor by animateFloatAsState(
        targetValue = if (model.isSelected.value) 1f else 0f,
        tween(durationMillis = (animDuration * 1.25f).toInt(), easing = easing),
        label = "selectedFactor"
    )
    val offsetX = sin(angle * Math.PI / 180f) * model.radius
    val offsetY = cos(angle * Math.PI / 180f) * model.radius

    val localDensity = LocalDensity.current
    Card(
        modifier
            .offset(-offsetX.dp, -offsetY.dp)
            .rotate(-model.angle)
            .offset {
                with(localDensity) {
                    IntOffset(0.dp.roundToPx(), (model.offsetY.dp * selectedFactor).roundToPx())
                }
            }
            .clickable(
                remember { MutableInteractionSource() },
                null,
                true,
                "Play game: ${game.name.lowercase()}"
            ) {
                if (!activeOverlapUI.value) {
                    MenuManager.selectedGame = game
                    SceneManager.loadScene(game)
                }
            }) {
        Column(
            Modifier
                .padding(10.dp)
                .widthIn(1.dp, Dp.Infinity),
            horizontalAlignment = Alignment.CenterHorizontally,
            verticalArrangement = Arrangement.Center
        ) {
            Row(verticalAlignment = Alignment.CenterVertically) {
                WheelItemTitle(name, selectedFactor)
                AnimatedVisibility(
                    model.isSelected.value, enter = slideInHorizontally(
                        tween(durationMillis = animDuration, easing = easing)
                    ) + expandIn(
                        tween(durationMillis = animDuration, easing = easing)
                    ) + fadeIn(
                        tween(durationMillis = animDuration, easing = easing)
                    ), exit = slideOutHorizontally(
                        tween(durationMillis = animDuration, easing = easing)
                    ) + shrinkOut(
                        tween(durationMillis = animDuration, easing = easing)
                    ) + fadeOut(
                        tween(durationMillis = animDuration, easing = easing)
                    )
                ) {
                    IconButton({
                        MenuManager.settingsGame.value = game
                        BaseUIHandler.openSettings()
                    }) {
                        Box(Modifier.fillMaxSize()) {
                            Icon(
                                Icons.Filled.Settings,
                                "$name settings",
                                Modifier.align(Alignment.TopCenter).fillMaxSize(0.75f)
                            )
                        }
                    }
                }
            }
            Spacer(modifier = Modifier.height(5.dp))
            WheelItemImage(model, name, selectedFactor)

            /*todo?
            AnimatedVisibility(
                visible = model.enabledDifficulties.isNotEmpty() && model.isSelected.value,
                enter = slideInVertically(
                    tween(durationMillis = animDuration, easing = easing)
                ) + expandIn(
                    tween(durationMillis = animDuration, easing = easing)
                ) + fadeIn(
                    tween(durationMillis = animDuration, easing = easing)
                ),
                exit = slideOutVertically(
                    tween(durationMillis = animDuration, easing = easing)
                ) + shrinkOut(
                    tween(durationMillis = animDuration, easing = easing)
                ) + fadeOut(
                    tween(durationMillis = animDuration, easing = easing)
                )
            ) {
                DifficultyDropdown(
                    Modifier
                        .align(Alignment.CenterHorizontally)
                        .padding(0.dp),
                    model.difficultyState,
                    model.callback,
                    model.enabledDifficulties
                )
            }*/
        }
    }
}

@Composable
private fun WheelItemTitle(name: String, selectedFactor: Float) {
    val widthBig = measureTextWidth(name, Typography.headlineLarge)
    val widthSmall = measureTextWidth(name, Typography.titleLarge)

    Text(
        name,
        textAlign = TextAlign.Center,
        modifier = Modifier.width(lerp(widthSmall, widthBig, selectedFactor)),
        maxLines = 1,
        style = Typography.headlineSmall,
        fontSize = lerp(
            Typography.titleLarge.fontSize.value,
            Typography.headlineLarge.fontSize.value,
            selectedFactor
        ).sp
    )
}

@Composable
private fun WheelItemImage(
    model: WheelItem, name: String, selectedFactor: Float
) {
    val configuration = LocalConfiguration.current
    val growthFactor = remember {
        val screenHeight = configuration.screenHeightDp
        val screenWidth = configuration.screenWidthDp
        val minScreenSize = screenHeight.coerceAtMost(screenWidth) - 300f
        100f.coerceAtMost(minScreenSize)
    }

    val painterResource = painterResource(model.image)
    val maxSize = remember { painterResource.intrinsicSize.maxDimension }
    val height = remember {
        with(painterResource.intrinsicSize) {
            height / (0.9f.coerceAtLeast(height / maxSize))
        }
    }
    val width = remember {
        with(painterResource.intrinsicSize) {
            width / (0.9f.coerceAtLeast(width / maxSize))
        }
    }

    val factor = (125.dp + lerp(0.dp, 1.dp, selectedFactor) * growthFactor) / maxSize

    Image(
        painterResource,
        name,
        modifier = Modifier
            .size(width * factor, height * factor)
            .fillMaxSize()
            .clip(RoundedCornerShape(8.dp))
    )
}

@Preview
@Composable
private fun Prev_WheelItem() {
    Column {
        val item = WheelItem(SceneManager.Scene.SUDOKU)
        item.isSelected.value = true
        WheelItem(item)
    }
}