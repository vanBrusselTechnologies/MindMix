package com.vanbrusselgames.mindmix.menu

import androidx.compose.animation.core.Spring
import androidx.compose.animation.core.animateFloatAsState
import androidx.compose.animation.core.spring
import androidx.compose.foundation.Image
import androidx.compose.foundation.gestures.detectDragGestures
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.layout.size
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Person
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.rotate
import androidx.compose.ui.draw.scale
import androidx.compose.ui.draw.shadow
import androidx.compose.ui.graphics.BlendMode
import androidx.compose.ui.graphics.ColorFilter
import androidx.compose.ui.graphics.RectangleShape
import androidx.compose.ui.graphics.TransformOrigin
import androidx.compose.ui.graphics.graphicsLayer
import androidx.compose.ui.input.pointer.pointerInput
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.times
import com.vanbrusselgames.mindmix.ColorHelper
import com.vanbrusselgames.mindmix.R
import com.vanbrusselgames.mindmix.SceneManager
import kotlin.math.cos
import kotlin.math.min
import kotlin.math.round
import kotlin.math.roundToInt
import kotlin.math.sin
import kotlin.math.sqrt

@Composable
fun GameWheel(
    isInnerWheel: Boolean, gameCount: Int, screenWidth: Dp, screenHeight: Dp
) {
    val withDupsFactor = if (isInnerWheel && MenuManager.withDuplicates) 2 else 1
    val angleStep = if (isInnerWheel) 360f / gameCount / withDupsFactor else 360f / 22f
    val gameWheelSize = min(screenHeight.value / 425f, screenWidth.value / 225f)
    val wheelSizeFactor = if (isInnerWheel) 1f else 1.625f
    var canRotate by remember { mutableStateOf(false) }
    var rotationAngle by remember { mutableStateOf(0f) }
    val rotationAnimation by animateFloatAsState(
        targetValue = rotationAngle, animationSpec = spring(
            dampingRatio = Spring.DampingRatioLowBouncy, stiffness = Spring.StiffnessVeryLow
        ), label = "gameWheelTurn"
    )
    Box(
        Modifier.fillMaxSize()
    ) {
        val scale = 5f * wheelSizeFactor
        Box(modifier = Modifier
            .size(
                100.dp * gameWheelSize, 100.dp * gameWheelSize
            )
            .offset(0.dp, (1f - 2f / scale) * screenHeight)
            .scale(scale)
            .align(Alignment.Center)
            .pointerInput(Unit) {
                detectDragGestures(onDragStart = { offset ->
                    val distX = offset.x - size.width / 2f
                    val distY = offset.y - size.height / 2f
                    val totalDistance = sqrt(distX * distX + distY * distY)
                    canRotate = totalDistance < size.width / 2f
                }, onDrag = { change, dragAmount ->
                    if (canRotate) {
                        change.consume()
                        rotationAngle += dragAmount.x / 2f
                    }
                }, onDragEnd = {
                    if (canRotate) {
                        rotationAngle = round(rotationAngle / angleStep) * angleStep
                        if (isInnerWheel) MenuManager.setSelectedGameIndex(-(rotationAngle / angleStep).roundToInt())
                        else MenuManager.setSelectedGameModeIndex(-(rotationAngle / angleStep).roundToInt())
                    }
                })
            }
            .graphicsLayer(
                rotationZ = rotationAnimation, transformOrigin = TransformOrigin.Center
            )) {
            if (gameCount == 0) return
            val radius = gameWheelSize * 40f * if (isInnerWheel) 1f else 1.125f
            for (i in 0 until gameCount * withDupsFactor) {
                if (i >= gameCount) {
                    //Select gameImage 1: we are placing dups now
                }
                val angle = i * angleStep
                val offsetX = sin(angle * Math.PI / 180f) * radius
                val offsetY = cos(angle * Math.PI / 180f) * radius
                Box(
                    modifier = Modifier
                        .align(Alignment.Center)
                        .offset(-offsetX.dp, -offsetY.dp)
                        .rotate(-i * angleStep)
                        .fillMaxHeight(0.225f / if (isInnerWheel) 1f else 1.75f)
                ) {
                    val yOffset = -radius.dp * if (isInnerWheel) 0.625f else 0.225f
                    when ((-i + 10 * gameCount) % gameCount) {
                        SceneManager.Scene.MINESWEEPER.ordinal ->
                            WheelItemImage("Minesweeper", R.drawable.game_icon_minesweeper, yOffset)
                        SceneManager.Scene.SOLITAIRE.ordinal ->
                            WheelItemImage("Solitaire", R.drawable.playingcards_detailed_clovers_a, yOffset)
                        SceneManager.Scene.SUDOKU.ordinal ->
                            WheelItemImage("Sudoku", R.drawable.game_icon_sudoku, yOffset)

                        else -> Icon(Icons.Filled.Person, "Game", Modifier.fillMaxSize())
                    }
                }
            }
        }
    }
}

@Composable
fun WheelItemImage(name: String, imageResourceId: Int, yOffset: Dp) {
    Box(Modifier.fillMaxSize()) {
        val shadowElevation = 10.dp
        Image(
            painterResource(imageResourceId),
            name,
            modifier = Modifier
                .align(Alignment.Center)
                .aspectRatio(1f)
                .fillMaxSize()
                .shadow(
                    clip = true, elevation = shadowElevation, shape = RectangleShape
                ),
            colorFilter = ColorFilter.tint(
                ColorHelper.getLightestColor(
                    MaterialTheme.colorScheme.onBackground, MaterialTheme.colorScheme.background
                ), blendMode = BlendMode.Multiply
            )
        )


        Text(
            name, modifier = Modifier
                .align(Alignment.Center)
                .offset(0.dp, yOffset)
        )
    }
}