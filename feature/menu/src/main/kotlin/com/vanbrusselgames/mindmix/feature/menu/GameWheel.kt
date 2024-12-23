package com.vanbrusselgames.mindmix.feature.menu

import androidx.compose.animation.core.Animatable
import androidx.compose.animation.core.Spring
import androidx.compose.animation.core.spring
import androidx.compose.foundation.gestures.Orientation
import androidx.compose.foundation.gestures.draggable
import androidx.compose.foundation.gestures.rememberDraggableState
import androidx.compose.foundation.interaction.MutableInteractionSource
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.offset
import androidx.compose.runtime.Composable
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.TransformOrigin
import androidx.compose.ui.graphics.graphicsLayer
import androidx.compose.ui.platform.LocalConfiguration
import androidx.compose.ui.tooling.preview.PreviewScreenSizes
import androidx.compose.ui.unit.dp
import androidx.navigation.NavController
import androidx.navigation.compose.rememberNavController
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import kotlinx.coroutines.launch
import kotlin.math.roundToInt

const val radius = 275f

data class GameWheel(val viewModel: MenuScreenViewModel, val gameCount: Int) {
    val withDupsFactor = if (MenuScreenViewModel.ADD_DUPLICATES) 2 else 1
    val wheelItemCount = gameCount * withDupsFactor
    val angleStep = 360f / wheelItemCount

    var selectedId = viewModel.games.filter { g -> g.value == viewModel.selectedGame }.keys.first()
    var rotationAngle = selectedId * angleStep
}

@Composable
fun GameWheel(viewModel: MenuScreenViewModel, navController: NavController, model: GameWheel) {
    val anim = remember { Animatable(model.rotationAngle) }
    val coroutineScope = rememberCoroutineScope()
    val draggableState = rememberDraggableState(onDelta = {
        model.rotationAngle += it / 8f
        coroutineScope.launch {
            anim.animateTo(
                model.rotationAngle, spring(Spring.DampingRatioLowBouncy, Spring.StiffnessVeryLow)
            )
        }
    })
    val items = rememberWheelItems(
        viewModel, model.wheelItemCount, model.gameCount, model.selectedId, model.angleStep
    )
    val interactionSource = remember { MutableInteractionSource() }
    val onDragEnd = {
        val unsafeCurrentItem = (model.rotationAngle / model.angleStep).roundToInt()
        model.rotationAngle = unsafeCurrentItem * model.angleStep
        val gameCount = model.gameCount
        val index = (unsafeCurrentItem.mod(gameCount) + gameCount).mod(gameCount)
        viewModel.selectedGame = viewModel.games[index]!!

        val steps = model.wheelItemCount
        val selectedItem = (unsafeCurrentItem.mod(steps) + steps).mod(steps)
        items[selectedItem].isSelected.value = true
        model.selectedId = selectedItem

        coroutineScope.launch {
            anim.animateTo(
                model.rotationAngle, spring(Spring.DampingRatioLowBouncy, Spring.StiffnessVeryLow)
            )
        }
    }

    Box(
        contentAlignment = Alignment.BottomCenter, modifier = Modifier
            .fillMaxSize()
            .draggable(draggableState,
                Orientation.Horizontal,
                enabled = !SceneManager.dialogActiveState.value,
                interactionSource,
                onDragStarted = { items.forEach { it.isSelected.value = false } },
                onDragStopped = { onDragEnd() })
    ) {
        Box(contentAlignment = Alignment.Center,
            modifier = Modifier
                .height(350.dp)
                .offset(0.dp, 275.dp)
                .graphicsLayer {
                    transformOrigin = TransformOrigin.Center
                    rotationZ = anim.value
                }) {
            for (i in items) {
                WheelItem(viewModel, navController, i)
            }
        }
    }
}

@Composable
fun rememberWheelItems(
    viewModel: MenuScreenViewModel,
    wheelItemCount: Int,
    gameCount: Int,
    selectedId: Int,
    angleStep: Float
): List<WheelItem> {
    val localConfig = LocalConfiguration.current
    return remember {
        val screenHeight = localConfig.screenHeightDp
        val screenWidth = localConfig.screenWidthDp
        val minScreenSize = screenHeight.coerceAtMost(screenWidth) - 300f
        val growthFactor = 100f.coerceAtMost(minScreenSize)

        List(wheelItemCount) { i ->
            val item = WheelItem(viewModel.games[i % gameCount]!!)
            item.growthFactor = growthFactor
            item.offsetY = -60f * (growthFactor / 100f)
            item.radius = radius
            item.angle = i * angleStep
            item.isSelected.value = i == selectedId

            item
        }
    }
}

@PreviewScreenSizes
@Composable
private fun PrevWheel() {
    val viewModel = MenuScreenViewModel()
    viewModel.selectedGame = SceneManager.Scene.SUDOKU
    GameWheel(viewModel, rememberNavController(), GameWheel(viewModel, 3))
}