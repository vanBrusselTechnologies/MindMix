package com.vanbrusselgames.mindmix.menu

import androidx.compose.animation.core.Spring
import androidx.compose.animation.core.animateFloatAsState
import androidx.compose.animation.core.spring
import androidx.compose.foundation.Image
import androidx.compose.foundation.gestures.detectDragGestures
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.heightIn
import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.layout.widthIn
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Person
import androidx.compose.material3.Icon
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableFloatStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.rotate
import androidx.compose.ui.graphics.TransformOrigin
import androidx.compose.ui.graphics.graphicsLayer
import androidx.compose.ui.input.pointer.pointerInput
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.AnnotatedString
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.times
import com.vanbrusselgames.mindmix.AutoSizeText
import com.vanbrusselgames.mindmix.R
import com.vanbrusselgames.mindmix.SceneManager
import kotlin.math.cos
import kotlin.math.min
import kotlin.math.round
import kotlin.math.roundToInt
import kotlin.math.sin

@Composable
fun GameWheel(gameCount: Int, screenWidth: Dp, screenHeight: Dp) {
    val withDupsFactor = if (MenuManager.withDuplicates) 2 else 1
    val wheelItemCount = gameCount * withDupsFactor
    val angleStep = 360f / wheelItemCount
    val gameWheelSize = min(screenHeight.value / 250f, screenWidth.value / 150f)
    val radius = gameWheelSize * 30f
    val id = MenuManager.games.filter { g -> g.value == MenuManager.selectedGame }.keys.first()
    var rotationAngle by remember { mutableFloatStateOf(id * angleStep) }
    var finishedRotate by remember { mutableStateOf(true) }
    val rotationAnimation by animateFloatAsState(
        targetValue = rotationAngle, animationSpec = spring(
            dampingRatio = Spring.DampingRatioLowBouncy, stiffness = Spring.StiffnessVeryLow
        ), label = "gameWheelTurn"
    )
    val scale = 5f

    Box(
        Modifier
            .fillMaxSize()
            .pointerInput(Unit) {
                detectDragGestures(onDrag = { change, dragAmount ->
                    change.consume()
                    rotationAngle += dragAmount.x / 8f
                    finishedRotate = false
                }, onDragEnd = {
                    rotationAngle = round(rotationAngle / angleStep) * angleStep
                    val index =
                        (((rotationAngle / angleStep) % gameCount + gameCount) % gameCount).roundToInt()
                    MenuManager.selectedGame = MenuManager.games[index]!!
                    finishedRotate = true
                })
            }) {
        Box(
            contentAlignment = Alignment.Center,
            modifier = Modifier
                .fillMaxSize()
                .offset(0.dp, (1f - 2f / scale) * screenHeight)
                .graphicsLayer {
                    transformOrigin = TransformOrigin.Center
                    rotationZ = rotationAnimation
                }
        ) {
            if (gameCount == 0) return
            val realAngle = (rotationAngle % 360 + 360) % 360
            val selectedItem = (realAngle / angleStep).roundToInt()
            for (i in 0 until wheelItemCount) {
                val isSelected = if (finishedRotate) i == selectedItem else false
                WheelItem(
                    i, isSelected, gameCount, radius, angleStep, screenHeight, screenWidth, scale
                )
            }
        }
    }
}

@Composable
fun WheelItem(
    index: Int,
    isSelected: Boolean,
    gameCount: Int,
    radius: Float,
    angleStep: Float,
    h: Dp,
    w: Dp,
    s: Float
) {
    val gameIndex = (index + 10 * gameCount) % gameCount
    val game = MenuManager.games[gameIndex]
    val angle = index * angleStep
    val selectedFactor by animateFloatAsState(
        targetValue = if (isSelected) 1f else 0f, label = "selectedFactor"
    )
    val offsetX = sin(angle * Math.PI / 180f) * radius * s
    val offsetY = cos(angle * Math.PI / 180f) * radius * s
    val maxHeight = h * 0.4f * (1 + 0.75f * selectedFactor)
    val maxWidth = w * 0.5f * (1 + 0.375f * selectedFactor)
    Box(
        modifier = Modifier
            .offset(-offsetX.dp, -offsetY.dp)
            .heightIn(max = maxHeight)
            .widthIn(max = maxWidth)
            .aspectRatio(3f / 5f)
            .rotate(-index * angleStep)
            .offset(0.dp, -0.1f * h * selectedFactor)
    ) {
        when (game) {
            SceneManager.Scene.MINESWEEPER -> WheelItemImage(
                "Minesweeper", R.drawable.game_icon_minesweeper
            )

            SceneManager.Scene.SOLITAIRE -> WheelItemImage(
                "Solitaire", R.drawable.playingcards_detailed_clovers_a
            )

            SceneManager.Scene.SUDOKU -> WheelItemImage(
                "Sudoku", R.drawable.game_icon_sudoku
            )

            else -> Icon(Icons.Filled.Person, "Game", Modifier.fillMaxSize())
        }
    }
}

@Composable
fun WheelItemImage(name: String, imageResourceId: Int) {
    Column(
        Modifier.fillMaxSize(), horizontalAlignment = Alignment.CenterHorizontally
    ) {
        AutoSizeText(
            text = AnnotatedString(name),
            modifier = Modifier
                .fillMaxHeight(0.25f)
                .fillMaxWidth(),
            textAlign = TextAlign.Center,
            maxLines = 1
        )

        Image(painterResource(imageResourceId), name, modifier = Modifier.fillMaxSize())
    }
}