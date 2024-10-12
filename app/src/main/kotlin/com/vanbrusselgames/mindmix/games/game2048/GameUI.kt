package com.vanbrusselgames.mindmix.games.game2048

import androidx.compose.animation.animateColorAsState
import androidx.compose.animation.core.FastOutSlowInEasing
import androidx.compose.animation.core.LinearEasing
import androidx.compose.animation.core.tween
import androidx.compose.foundation.gestures.detectDragGestures
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.heightIn
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.widthIn
import androidx.compose.foundation.lazy.grid.GridCells
import androidx.compose.foundation.lazy.grid.LazyHorizontalGrid
import androidx.compose.foundation.lazy.grid.items
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.LocalTextStyle
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableFloatStateOf
import androidx.compose.runtime.mutableLongStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.draw.drawBehind
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.input.pointer.pointerInput
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.IntOffset
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import kotlin.math.min

const val threshold = 100

@Composable
fun GameUI(viewModel: GameViewModel) {
    Box(Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
        Box(
            Modifier
                .widthIn(100.dp, 400.dp)
                .heightIn(100.dp, 400.dp)
        ) {
            Grid2048(viewModel)
        }
    }
}

@Composable
fun Grid2048(viewModel: GameViewModel) {
    var offsetX by remember { mutableFloatStateOf(0f) }
    var offsetY by remember { mutableFloatStateOf(0f) }

    Box(
        Modifier
            .aspectRatio(1f)
            .pointerInput(Unit) {
                detectDragGestures(onDragStart = {
                    offsetX = 0f
                    offsetY = 0f
                }, onDragEnd = {
                    if (viewModel.isStuck.value) return@detectDragGestures
                    if (kotlin.math.abs(offsetX) < threshold && kotlin.math.abs(offsetY) < threshold) return@detectDragGestures
                    if (kotlin.math.abs(offsetX) > kotlin.math.abs(offsetY)) {
                        if (offsetX > 0) viewModel.swipeRight() else viewModel.swipeLeft()
                    } else {
                        if (offsetY > 0) viewModel.swipeDown() else viewModel.swipeUp()
                    }
                }, onDrag = { change, dragAmount ->
                    if (viewModel.isStuck.value) return@detectDragGestures
                    change.consume()

                    offsetX += dragAmount.x
                    offsetY += dragAmount.y

                    //Show effect what will happen when swipe will get finished -- Just effects
                })
            }) {
        LazyHorizontalGrid(GridCells.Fixed(viewModel.sideSize), userScrollEnabled = false) {
            items(viewModel.cellList, key = { item -> item.id }) {
                val value = remember { mutableLongStateOf(it.value) }
                val background by animateColorAsState(it.background.value,
                    label = "cell_${it.id} background",
                    finishedListener = { c -> value.longValue = it.value })
                Box(
                    Modifier
                        .aspectRatio(1f)
                        .animateItem(
                            fadeInSpec = tween(275, 125, FastOutSlowInEasing),
                            placementSpec = tween<IntOffset>(400, 0, FastOutSlowInEasing),
                            fadeOutSpec = tween(125, 50, LinearEasing)
                        )
                        .padding(2.dp)
                        .clip(RoundedCornerShape(5.dp))
                        .drawBehind {
                            if (it.value != 0L) drawRect(background)
                        }, contentAlignment = Alignment.Center
                ) {
                    Text(
                        if (it.value == 0L) "" else value.longValue.toString(),
                        fontSize = min(200f / viewModel.sideSize, (550f / viewModel.sideSize) / value.longValue.toString().length).sp,
                        color = if (value.longValue < 256) Color.Black else Color(220, 220, 220),
                        fontWeight = FontWeight.ExtraBold,
                        style = LocalTextStyle.current.copy(fontFeatureSettings = "tnum")
                    )
                }
            }
        }
    }
}