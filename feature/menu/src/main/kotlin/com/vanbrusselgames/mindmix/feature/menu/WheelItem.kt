package com.vanbrusselgames.mindmix.feature.menu

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
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.IntOffset
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.compose.ui.unit.times
import androidx.compose.ui.util.lerp
import androidx.compose.ui.zIndex
import androidx.navigation.NavController
import androidx.navigation.compose.rememberNavController
import com.vanbrusselgames.mindmix.core.designsystem.theme.Typography
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.core.navigation.SceneManager.Scene
import com.vanbrusselgames.mindmix.core.ui.measureTextWidth
import com.vanbrusselgames.mindmix.feature.settings.navigation.navigateToSettings
import com.vanbrusselgames.mindmix.games.game2048.Game2048
import com.vanbrusselgames.mindmix.games.minesweeper.Minesweeper
import com.vanbrusselgames.mindmix.games.solitaire.Solitaire
import com.vanbrusselgames.mindmix.games.sudoku.Sudoku
import kotlin.math.cos
import kotlin.math.sin

const val animDuration = 150
val easing = LinearEasing

data class WheelItem(val game: Scene) {
    var isSelected = mutableStateOf(false)
    var growthFactor = 0f
    var offsetY = 0f
    var angle = 0f
    var radius = 0f

    val title = when (game) {
        Scene.MENU -> Menu.NAME_RES_ID
        Scene.MINESWEEPER -> Minesweeper.NAME_RES_ID
        Scene.SOLITAIRE -> Solitaire.NAME_RES_ID
        Scene.SUDOKU -> Sudoku.NAME_RES_ID
        Scene.GAME2048 -> Game2048.NAME_RES_ID
    }

    val image = when (game) {
        Scene.MENU -> Menu.IMAGE_RES_ID
        Scene.MINESWEEPER -> Minesweeper.IMAGE_RES_ID
        Scene.SOLITAIRE -> Solitaire.IMAGE_RES_ID
        Scene.SUDOKU -> Sudoku.IMAGE_RES_ID
        Scene.GAME2048 -> Game2048.IMAGE_RES_ID
    }
}

@Composable
fun WheelItem(
    viewModel: MenuScreenViewModel,
    navController: NavController,
    model: WheelItem,
    modifier: Modifier = Modifier
) {
    val game = model.game
    val angle = model.angle
    val name = stringResource(model.title)

    val selectedFactor by animateFloatAsState(
        targetValue = if (model.isSelected.value) 1f else 0f, tween(
            durationMillis = (animDuration * 1.25f).toInt(), easing = easing
        ), label = "selectedFactor"
    )
    val offsetX = sin(angle * Math.PI / 180f) * model.radius
    val offsetY = cos(angle * Math.PI / 180f) * model.radius

    Card(modifier
        .zIndex(if (model.isSelected.value) 1f else 0f)
        .offset(-offsetX.dp, -offsetY.dp)
        .rotate(-model.angle)
        .offset { IntOffset(0.dp.roundToPx(), (model.offsetY.dp * selectedFactor).roundToPx()) }
        .clickable(
            remember { MutableInteractionSource() },
            null,
            true,
            "Play game: ${game.name.lowercase()}"
        ) {
            if (!SceneManager.dialogActiveState.value) {
                viewModel.selectedGame = game
                viewModel.navigateToSelectedGame(navController)
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
                        viewModel.settingsGame = game
                        navController.navigateToSettings()
                    }) {
                        Box(Modifier.fillMaxSize()) {
                            Icon(
                                Icons.Filled.Settings,
                                "$name settings",
                                Modifier
                                    .align(Alignment.TopCenter)
                                    .fillMaxSize(0.75f)
                            )
                        }
                    }
                }
            }
            Spacer(modifier = Modifier.height(5.dp))
            WheelItemImage(model, name, selectedFactor)
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
        modifier = Modifier.width(lerp(widthSmall.value, widthBig.value, selectedFactor).dp),
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
        with(painterResource.intrinsicSize) { width / (0.9f.coerceAtLeast(width / maxSize)) }
    }

    val factor = ((125 + selectedFactor * growthFactor) / maxSize).dp

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
        val item = WheelItem(Scene.SUDOKU)
        item.isSelected.value = true
        WheelItem(MenuScreenViewModel(), rememberNavController(), item)
    }
}