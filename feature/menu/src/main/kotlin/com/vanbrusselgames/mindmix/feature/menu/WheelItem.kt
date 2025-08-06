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
import androidx.compose.ui.unit.lerp
import androidx.compose.ui.unit.sp
import androidx.compose.ui.unit.times
import androidx.compose.ui.util.lerp
import androidx.compose.ui.zIndex
import androidx.navigation.NavController
import androidx.navigation.compose.rememberNavController
import com.vanbrusselgames.mindmix.core.designsystem.theme.Typography
import com.vanbrusselgames.mindmix.core.model.GameScene
import com.vanbrusselgames.mindmix.core.model.Scene
import com.vanbrusselgames.mindmix.core.model.SceneRegistry
import com.vanbrusselgames.mindmix.core.ui.measureTextWidth
import com.vanbrusselgames.mindmix.feature.settings.navigation.navigateToSettings
import com.vanbrusselgames.mindmix.games.game2048.model.Game2048
import com.vanbrusselgames.mindmix.games.game2048.navigation.navigateToGame2048Settings
import com.vanbrusselgames.mindmix.games.minesweeper.Minesweeper
import com.vanbrusselgames.mindmix.games.solitaire.Solitaire
import com.vanbrusselgames.mindmix.games.sudoku.model.Sudoku
import com.vanbrusselgames.mindmix.games.sudoku.navigation.navigateToSudokuSettings
import kotlin.math.cos
import kotlin.math.sin

const val animDuration = 150
val easing = LinearEasing

data class WheelItem(val game: GameScene) {
    var isSelected = mutableStateOf(false)
    var growthFactor = 0f
    var offsetY = 0f
    var angle = 0f
    var radius = 0f

    val title = when (game) {
        SceneRegistry.Minesweeper -> Minesweeper.NAME_RES_ID
        SceneRegistry.Solitaire -> Solitaire.NAME_RES_ID
        SceneRegistry.Sudoku -> Sudoku.NAME_RES_ID
        SceneRegistry.Game2048 -> Game2048.NAME_RES_ID
        else -> -1
    }

    val image = when (game) {
        SceneRegistry.Minesweeper -> Minesweeper.IMAGE_RES_ID
        SceneRegistry.Solitaire -> Solitaire.IMAGE_RES_ID
        SceneRegistry.Sudoku -> Sudoku.IMAGE_RES_ID
        SceneRegistry.Game2048 -> Game2048.IMAGE_RES_ID
        else -> -1
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
        ), label = "selectedFactorAnim"
    )
    val offsetX = remember { sin(angle * Math.PI / 180f) * model.radius }
    val offsetY = remember { cos(angle * Math.PI / 180f) * model.radius }
    val interactionSource = remember { MutableInteractionSource() }
    Card(
        modifier
            .zIndex(if (model.isSelected.value) 1f else 0f)
            .offset(-offsetX.dp, -offsetY.dp)
            .rotate(-angle)
            .offset { IntOffset(0.dp.roundToPx(), (model.offsetY.dp * selectedFactor).roundToPx()) }
            .clickable(
                interactionSource, null, true, "Play game: ${game.name.lowercase()}"
            ) {
                viewModel.selectedGame = game
                viewModel.navigateToSelectedGame(navController)
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
                WheelItemIconButton(viewModel, navController, model, name, game)
            }
            Spacer(modifier = Modifier.height(5.dp))
            WheelItemImage(model, name, selectedFactor)
        }
    }
}

@Composable
fun WheelItemIconButton(
    viewModel: MenuScreenViewModel,
    navController: NavController,
    model: WheelItem,
    name: String,
    game: Scene
) {
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
        ), label = "ShowSettings"
    ) {
        IconButton({
            viewModel.settingsGame = game
            when (game) {
                SceneRegistry.Game2048 -> navController.navigateToGame2048Settings()
                SceneRegistry.Sudoku -> navController.navigateToSudokuSettings()
                else -> navController.navigateToSettings()
            }
        }) {
            Icon(Icons.Filled.Settings, "$name settings")
        }
    }
}

@Composable
private fun WheelItemTitle(name: String, selectedFactor: Float) {
    val widthSmall = measureTextWidth(name, Typography.titleLarge)
    val widthBig = measureTextWidth(name, Typography.headlineLarge)
    val fontSizeSmall = remember { Typography.titleLarge.fontSize.value.sp }
    val fontSizeBig = remember { Typography.headlineLarge.fontSize.value.sp }

    Text(
        name,
        textAlign = TextAlign.Center,
        modifier = Modifier.width(lerp(widthSmall.value, widthBig.value, selectedFactor).dp),
        maxLines = 1,
        style = Typography.headlineSmall,
        fontSize = lerp(fontSizeSmall, fontSizeBig, selectedFactor)
    )
}

@Composable
private fun WheelItemImage(
    model: WheelItem, name: String, selectedFactor: Float
) {
    val localConfig = LocalConfiguration.current
    val growthFactor = remember {
        val screenHeight = localConfig.screenHeightDp
        val screenWidth = localConfig.screenWidthDp
        val minScreenSize = screenHeight.coerceAtMost(screenWidth) - 300f
        100f.coerceAtMost(minScreenSize)
    }

    val painterResource = painterResource(model.image)
    val maxSize = remember { painterResource.intrinsicSize.maxDimension }
    val height = remember {
        with(painterResource.intrinsicSize) { height / (0.9f.coerceAtLeast(height / maxSize)) }
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
        val item = WheelItem(SceneRegistry.Sudoku)
        item.isSelected.value = true
        WheelItem(MenuScreenViewModel(), rememberNavController(), item)
    }
}