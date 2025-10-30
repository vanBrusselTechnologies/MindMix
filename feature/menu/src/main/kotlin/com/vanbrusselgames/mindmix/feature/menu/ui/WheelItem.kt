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
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.IntrinsicSize
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.sizeIn
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.layout.wrapContentHeight
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
import androidx.compose.ui.graphics.graphicsLayer
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
import androidx.compose.ui.unit.max
import androidx.compose.ui.unit.min
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

const val animDurationMs = 150
val easing = LinearEasing

val standardWheelItemCardHeight = 280.dp
val standardWheelItemCardWidth = 250.dp
val padding = 10.dp
val spacerHeight = 5.dp
val textHeight = 48.dp

@OptIn(ExperimentalSharedTransitionApi::class)
@Composable
fun SharedTransitionScope.WheelItem(
    viewModel: IMenuScreenViewModel,
    navController: NavController,
    animatedContentScope: AnimatedContentScope,
    model: WheelItem,
    maxHeight: Dp
) {
    val wheelItemCardHeight = min(maxHeight, standardWheelItemCardHeight)
    val wheelItemCardWidth = standardWheelItemCardWidth
    val offsetX =
        remember(wheelItemCardHeight.value) { sin(model.angle * Math.PI / 180f) * (model.radius + wheelItemCardHeight.value / 2f) }
    val offsetY =
        remember(wheelItemCardHeight.value) { cos(model.angle * Math.PI / 180f) * (model.radius + wheelItemCardHeight.value / 2f) }
    val interactionSource = remember { MutableInteractionSource() }
    if (!model.visible.value) return

    val selectedFactor by animateFloatAsState(
        targetValue = if (model.isSelected.value) 1f else 0f,
        tween(durationMillis = (animDurationMs * 1.25f).toInt(), easing = easing),
        label = "selectedFactorAnim"
    )

    val onClickLabel = "Play game: ${model.game.name}"
    // TODO: stringResource(R.string.accessibility_action_select_game)

    val heightScaleChange = 0.4f
    val offsetChangeDp = wheelItemCardHeight * heightScaleChange / 2

    Card(
        Modifier
            .sizeIn(10.dp, 10.dp, wheelItemCardWidth, wheelItemCardHeight)
            .zIndex(if (model.isSelected.value) 1f else 0f)
            .offset(-offsetX.dp, -offsetY.dp)
            .rotate(-model.angle)
            .offset { IntOffset(0, (offsetChangeDp * (1 - selectedFactor)).roundToPx()) }
            .graphicsLayer {
                val scaleFactor = 0.6f + selectedFactor * heightScaleChange
                scaleY = scaleFactor
                scaleX = scaleFactor
            }
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
            }, RoundedCornerShape(8.dp)
    ) {
        Column(
            Modifier
                .width(IntrinsicSize.Max)
                .padding(padding),
            verticalArrangement = Arrangement.Center,
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            val name = stringResource(model.title)
            Row(
                Modifier.padding(start = if (model.isSelected.value) 12.dp else 0.dp),
                verticalAlignment = Alignment.CenterVertically
            ) {
                WheelItemTitle(name)
                WheelItemIconButton(navController, model.isSelected, name, model.game)
            }
            Spacer(Modifier.height(spacerHeight))
            WheelItemImage(
                animatedContentScope, model.gameType, wheelItemCardWidth, wheelItemCardHeight
            )
        }
    }
}

@Composable
fun WheelItemIconButton(
    navController: NavController, isSelected: MutableState<Boolean>, name: String, game: Scene
) {
    val settingsContentDescription = "$name settings"
    // TODO: stringResource(R.string.accessibility_settings_for_game, name)

    val enterTransition = remember {
        slideInHorizontally(tween(durationMillis = animDurationMs, easing = easing)) +
                expandIn(tween(durationMillis = animDurationMs, easing = easing)) +
                fadeIn(tween(durationMillis = animDurationMs, easing = easing))
    }
    val exitTransition = remember {
        slideOutHorizontally(tween(durationMillis = animDurationMs, easing = easing)) +
                shrinkOut(tween(durationMillis = animDurationMs, easing = easing)) +
                fadeOut(tween(durationMillis = animDurationMs, easing = easing))
    }

    AnimatedVisibility(
        isSelected.value, Modifier, enterTransition, exitTransition, "ShowSettings"
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
private fun WheelItemTitle(name: String) {
    val textWidthDp = measureTextWidth(name, Typography.titleLarge)

    Text(
        name,
        Modifier
            .width(textWidthDp)
            .height(textHeight)
            .wrapContentHeight(),
        textAlign = TextAlign.Center,
        maxLines = 1,
        style = Typography.titleLarge
    )
}

@OptIn(ExperimentalSharedTransitionApi::class)
@Composable
private fun SharedTransitionScope.WheelItemImage(
    animatedContentScope: AnimatedContentScope,
    gameType: GameType,
    wheelItemWidth: Dp,
    wheelItemHeight: Dp
) {
    val painterResource = painterResource(gameType.iconRes)
    val size = painterResource.intrinsicSize
    val maxFactorX = (wheelItemWidth - padding * 2) / size.width
    val maxFactorY = (wheelItemHeight - padding * 2 - spacerHeight - textHeight) / size.height
    val factor = min(maxFactorX, maxFactorY)
    val imgSize = max(size.width * factor, size.height * factor)

    Image(
        painterResource,
        null,
        Modifier
            .sizeIn(10.dp, 10.dp, imgSize, imgSize)
            .clip(RoundedCornerShape(8.dp))
            .sharedElement(
                rememberSharedContentState("image-${gameType.name}"), animatedContentScope
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
            val isSelected = true
            val heightScaleChange = 0.4f
            val offsetChangeDp = standardWheelItemCardHeight * heightScaleChange / 2
            Box(Modifier.offset {
                IntOffset(0, (offsetChangeDp * if (isSelected) 1 else 0).roundToPx())
            }) {
                val item = WheelItem(SceneRegistry.Solitaire, 0f, 0f)
                item.isSelected.value = isSelected
                val vm = remember { MockMenuScreenViewModel() }
                WheelItem(
                    vm,
                    rememberNavController(),
                    this@AnimatedContent,
                    item,
                    standardWheelItemCardHeight
                )
            }
        }
    }
}