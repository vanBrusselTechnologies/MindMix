package com.vanbrusselgames.mindmix.feature.menu.ui

import androidx.compose.animation.AnimatedContent
import androidx.compose.animation.AnimatedContentScope
import androidx.compose.animation.AnimatedVisibility
import androidx.compose.animation.ExperimentalSharedTransitionApi
import androidx.compose.animation.SharedTransitionLayout
import androidx.compose.animation.SharedTransitionScope
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
import androidx.compose.runtime.MutableState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.draw.rotate
import androidx.compose.ui.platform.LocalDensity
import androidx.compose.ui.platform.LocalWindowInfo
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.semantics.CollectionItemInfo
import androidx.compose.ui.semantics.Role
import androidx.compose.ui.semantics.collectionItemInfo
import androidx.compose.ui.semantics.semantics
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.IntOffset
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.lerp
import androidx.compose.ui.unit.sp
import androidx.compose.ui.unit.times
import androidx.compose.ui.zIndex
import androidx.navigation.NavController
import androidx.navigation.compose.rememberNavController
import com.vanbrusselgames.mindmix.core.designsystem.theme.Typography
import com.vanbrusselgames.mindmix.core.games.model.GameType
import com.vanbrusselgames.mindmix.core.model.Scene
import com.vanbrusselgames.mindmix.core.model.SceneRegistry
import com.vanbrusselgames.mindmix.core.ui.measureTextWidth
import com.vanbrusselgames.mindmix.feature.menu.model.WheelItem
import com.vanbrusselgames.mindmix.feature.menu.viewmodel.IMenuScreenViewModel
import com.vanbrusselgames.mindmix.feature.menu.viewmodel.MockMenuScreenViewModel
import com.vanbrusselgames.mindmix.feature.settings.navigation.navigateToSettings
import com.vanbrusselgames.mindmix.games.game2048.navigation.navigateToGame2048Settings
import com.vanbrusselgames.mindmix.games.minesweeper.navigation.navigateToMinesweeperSettings
import com.vanbrusselgames.mindmix.games.solitaire.navigation.navigateToSolitaireSettings
import com.vanbrusselgames.mindmix.games.sudoku.navigation.navigateToSudokuSettings
import kotlin.math.cos
import kotlin.math.sin

const val animDuration = 150
val easing = LinearEasing

@OptIn(ExperimentalSharedTransitionApi::class)
@Composable
fun SharedTransitionScope.WheelItem(
    viewModel: IMenuScreenViewModel,
    navController: NavController,
    animatedContentScope: AnimatedContentScope,
    model: WheelItem,
    modifier: Modifier = Modifier
) {
    val offsetX = remember { sin(model.angle * Math.PI / 180f) * model.radius }
    val offsetY = remember { cos(model.angle * Math.PI / 180f) * model.radius }
    val interactionSource = remember { MutableInteractionSource() }
    if (!model.visible.value) return

    val selectedFactor by animateFloatAsState(
        targetValue = if (model.isSelected.value) 1f else 0f, tween(
            durationMillis = (animDuration * 1.25f).toInt(), easing = easing
        ), label = "selectedFactorAnim"
    )

    val onClickLabel =
        "Play game: ${model.game.name}"// TODO: stringResource(R.string.accessibility_action_select_game)

    Card(
        modifier
            .zIndex(if (model.isSelected.value) 1f else 0f)
            .offset(-offsetX.dp, -offsetY.dp)
            .rotate(-model.angle)
            .offset { IntOffset(0.dp.roundToPx(), (model.offsetY.dp * selectedFactor).roundToPx()) }
            .semantics {
                this.collectionItemInfo = CollectionItemInfo(
                    1,
                    1,
                    viewModel.wheelModel.items.indexOfFirst { it.game.name == model.game.name },
                    1
                )
            }
            .clickable(interactionSource, null, true, onClickLabel, Role.Button) {
                viewModel.selectedGame.value = model.game
                viewModel.navigateToSelectedGame(navController)
            }) {
        Column(
            Modifier
                .padding(10.dp)
                .widthIn(1.dp, Dp.Infinity),
            horizontalAlignment = Alignment.CenterHorizontally,
            verticalArrangement = Arrangement.Center
        ) {
            val name = stringResource(model.title)
            Row(verticalAlignment = Alignment.CenterVertically) {
                WheelItemTitle(name, selectedFactor)
                WheelItemIconButton(navController, model.isSelected, name, model.game)
            }
            Spacer(modifier = Modifier.height(5.dp))
            WheelItemImage(animatedContentScope, model.gameType, selectedFactor)
        }
    }
}

@Composable
fun WheelItemIconButton(
    navController: NavController, isSelected: MutableState<Boolean>, name: String, game: Scene
) {
    val settingsContentDescription =
        "$name settings"// TODO: stringResource(R.string.accessibility_settings_for_game, name)

    AnimatedVisibility(
        isSelected.value, enter = slideInHorizontally(
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
            when (game) {
                SceneRegistry.Game2048 -> navController.navigateToGame2048Settings()
                SceneRegistry.Minesweeper -> navController.navigateToMinesweeperSettings()
                SceneRegistry.Solitaire -> navController.navigateToSolitaireSettings()
                SceneRegistry.Sudoku -> navController.navigateToSudokuSettings()
                else -> navController.navigateToSettings()
            }
        }) {
            Icon(Icons.Filled.Settings, settingsContentDescription)
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
        modifier = Modifier.width(lerp(widthSmall, widthBig, selectedFactor)),
        fontSize = lerp(fontSizeSmall, fontSizeBig, selectedFactor),
        textAlign = TextAlign.Center,
        maxLines = 1,
        style = Typography.headlineSmall
    )
}

@OptIn(ExperimentalSharedTransitionApi::class)
@Composable
private fun SharedTransitionScope.WheelItemImage(
    animatedContentScope: AnimatedContentScope, gameType: GameType, selectedFactor: Float
) {
    val localDensity = LocalDensity.current
    val containerSize = LocalWindowInfo.current.containerSize
    val growthFactor = remember(containerSize) {
        with(localDensity) {
            val screenHeight = containerSize.height.toDp().value
            val screenWidth = containerSize.width.toDp().value
            val minScreenSize = screenHeight.coerceAtMost(screenWidth) - 300f
            100f.coerceAtMost(minScreenSize)
        }
    }

    val painterResource = painterResource(gameType.iconRes)
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
        null,
        modifier = Modifier
            .size(width * factor, height * factor)
            .fillMaxSize()
            .clip(RoundedCornerShape(8.dp))
            .sharedElement(
                rememberSharedContentState(key = "image-${gameType.name}"), animatedContentScope
            )
    )
}

@OptIn(ExperimentalSharedTransitionApi::class)
@Preview
@Composable
private fun Prev_WheelItem() {
    SharedTransitionLayout {
        AnimatedContent(null) {
            it
            Column {
                val item = WheelItem(SceneRegistry.Sudoku, 0f, 0f)
                item.isSelected.value = true
                val vm = remember { MockMenuScreenViewModel() }
                WheelItem(vm, rememberNavController(), this@AnimatedContent, item, Modifier)
            }
        }
    }
}