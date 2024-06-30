package com.vanbrusselgames.mindmix.menu

import androidx.compose.animation.core.Animatable
import androidx.compose.animation.core.Spring
import androidx.compose.animation.core.spring
import androidx.compose.foundation.gestures.Orientation
import androidx.compose.foundation.gestures.draggable
import androidx.compose.foundation.gestures.rememberDraggableState
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
import com.vanbrusselgames.mindmix.BaseLayout.Companion.activeOverlapUI
import com.vanbrusselgames.mindmix.Logger
import com.vanbrusselgames.mindmix.SceneManager
import kotlinx.coroutines.launch
import kotlin.math.round
import kotlin.math.roundToInt

const val radius = 250f

data class GameWheel(val gameCount: Int) {
    val withDupsFactor = if (MenuManager.ADD_DUPLICATES) 2 else 1
    val wheelItemCount = gameCount * withDupsFactor
    val angleStep = 360f / wheelItemCount

    var selectedId =
        MenuManager.games.filter { g -> g.value == MenuManager.selectedGame }.keys.first()
    var rotationAngle = selectedId * angleStep
}

@Composable
fun GameWheel(model: GameWheel) {
    val anim = remember { Animatable(model.rotationAngle) }
    val coroutineScope = rememberCoroutineScope()
    val draggableState = rememberDraggableState(onDelta = {
        model.rotationAngle += it / 8f
        coroutineScope.launch {
            anim.animateTo(
                model.rotationAngle, animationSpec = spring(
                    dampingRatio = Spring.DampingRatioLowBouncy, stiffness = Spring.StiffnessVeryLow
                )
            )
        }
    })

    val items =
        rememberWheelItems(model.wheelItemCount, model.gameCount, model.selectedId, model.angleStep)

    val onDragEnd = {
        coroutineScope.launch {
            model.rotationAngle = round(model.rotationAngle / model.angleStep) * model.angleStep
            val index =
                (((model.rotationAngle / model.angleStep) % model.gameCount + model.gameCount) % model.gameCount).roundToInt()
            MenuManager.selectedGame = MenuManager.games[index]!!

            val realAngle = (model.rotationAngle % 360 + 360) % 360
            val selectedItem = (realAngle / model.angleStep).roundToInt()
            items[selectedItem].isSelected.value = true
            model.selectedId = selectedItem

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
                enabled = !activeOverlapUI.value,
                onDragStarted = {
                    val realAngle = (model.rotationAngle % 360 + 360) % 360
                    val selectedItem = (realAngle / model.angleStep).roundToInt()
                    items[selectedItem].isSelected.value = false
                },
                onDragStopped = {
                    onDragEnd()
                })
    ) {
        Box(contentAlignment = Alignment.Center,
            modifier = Modifier
                .height(350.dp)
                .offset(0.dp, 250.dp)
                .graphicsLayer {
                    transformOrigin = TransformOrigin.Center
                    rotationZ = anim.value
                }) {
            for (i in items) {
                WheelItem(i)
            }
        }
    }
}

@Composable
fun rememberWheelItems(
    wheelItemCount: Int, gameCount: Int, selectedId: Int, angleStep: Float
): List<WheelItem> {
    val localConfig = LocalConfiguration.current
    return remember(localConfig) {
        val screenHeight = localConfig.screenHeightDp
        val screenWidth = localConfig.screenWidthDp
        val minScreenSize = screenHeight.coerceAtMost(screenWidth) - 300f
        val growthFactor = 100f.coerceAtMost(minScreenSize)

        List(wheelItemCount) { i ->
            val item = WheelItem(MenuManager.games[i % gameCount]!!)
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
    MenuManager.selectedGame = SceneManager.Scene.SUDOKU
    GameWheel(GameWheel(3))
}