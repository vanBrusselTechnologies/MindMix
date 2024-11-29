package com.vanbrusselgames.mindmix.games.game2048

import androidx.compose.animation.animateColorAsState
import androidx.compose.animation.core.FastOutSlowInEasing
import androidx.compose.animation.core.LinearEasing
import androidx.compose.animation.core.tween
import androidx.compose.foundation.background
import androidx.compose.foundation.gestures.detectDragGestures
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.BoxWithConstraints
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.heightIn
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.layout.widthIn
import androidx.compose.foundation.lazy.grid.GridCells
import androidx.compose.foundation.lazy.grid.LazyHorizontalGrid
import androidx.compose.foundation.lazy.grid.items
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.LocalTextStyle
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.SnackbarHostState
import androidx.compose.material3.Surface
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableLongStateOf
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.blur
import androidx.compose.ui.draw.clip
import androidx.compose.ui.draw.drawBehind
import androidx.compose.ui.geometry.Offset
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.input.pointer.pointerInput
import androidx.compose.ui.platform.LocalDensity
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.tooling.preview.PreviewLightDark
import androidx.compose.ui.unit.IntOffset
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.min
import androidx.compose.ui.util.fastRoundToInt
import androidx.navigation.NavController
import androidx.navigation.compose.rememberNavController
import com.vanbrusselgames.mindmix.core.common.BaseScene
import com.vanbrusselgames.mindmix.core.designsystem.theme.MindMixTheme
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import kotlin.math.max
import kotlin.math.sqrt

const val threshold = 100

@Composable
fun GameUI(
    viewModel: Game2048ViewModel, navController: NavController, snackbarHostState: SnackbarHostState
) {
    BaseScene(viewModel, navController, snackbarHostState) {
        Box(
            Modifier
                .fillMaxSize()
                .blur(if (SceneManager.dialogActiveState.value) viewModel.blurStrength else 0.dp),
            contentAlignment = Alignment.Center
        ) {
            Column(horizontalAlignment = Alignment.CenterHorizontally) {
                Text(stringResource(R.string.game_2048_score_label))
                Text(viewModel.score.longValue.toString())
                Spacer(modifier = Modifier.size(16.dp))
                Box(
                    Modifier
                        .widthIn(100.dp, 400.dp)
                        .heightIn(100.dp, 400.dp)
                ) { Grid2048(viewModel, navController) }
            }
        }
    }
}

@Composable
fun Grid2048(viewModel: Game2048ViewModel, navController: NavController) {
    var totalDragOffset = remember { Offset.Zero }
    BoxWithConstraints(
        Modifier
            .aspectRatio(1f)
            .pointerInput(Unit) {
                detectDragGestures(onDragStart = {
                    totalDragOffset = Offset.Zero
                }, onDragEnd = {
                    viewModel.handleDragGestures(navController, totalDragOffset)
                }, onDrag = { change, dragAmount ->
                    if (SceneManager.dialogActiveState.value) return@detectDragGestures
                    if (viewModel.isStuck || viewModel.delayedDialog) return@detectDragGestures
                    change.consume()
                    totalDragOffset += dragAmount
                    //Show effect what will happen when swipe will get finished -- Just effects
                })
            }) {
        val currentSpace = remember(this.maxWidth, this.maxHeight, viewModel.sideSize) {
            min(this.maxWidth, this.maxHeight) / viewModel.sideSize
        }
        val localDensity = LocalDensity.current
        val fontSize = remember(currentSpace) { with(localDensity) { currentSpace.toSp() * 1.25f } }

        val modifier = Modifier
            .aspectRatio(1f)
            .padding(2.dp)
            .clip(RoundedCornerShape(5.dp))
        LazyHorizontalGrid(
            GridCells.Fixed(sqrt(viewModel.cellList.size.toFloat()).fastRoundToInt()),
            Modifier.background(
                MaterialTheme.colorScheme.secondaryContainer,
                RoundedCornerShape(5.dp)
            ).padding(3.dp),
            userScrollEnabled = false
        ) {
            items(viewModel.cellList, key = { item -> item.id }) {
                val value = remember { mutableLongStateOf(it.value) }
                val background by animateColorAsState(it.background.value,
                    label = "cell_${it.id} background",
                    finishedListener = { c -> value.longValue = it.value })
                Box(
                    if (it.value == 0L) modifier else modifier
                        .animateItem(
                            fadeInSpec = tween(200, 100, FastOutSlowInEasing),
                            placementSpec = tween<IntOffset>(300, 0, FastOutSlowInEasing),
                            fadeOutSpec = tween(75, 25, LinearEasing)
                        )
                        .drawBehind { if (it.value != 0L) drawRect(background) }, Alignment.Center
                ) {
                    if (it.value != 0L) {
                        val letters = remember(value.longValue) {
                            max(1.5f, value.longValue.toString().length.toFloat())
                        }
                        Text(
                            value.longValue.toString(),
                            color = if (value.longValue < 256) Color.Black else Color(
                                220, 220, 220
                            ),
                            fontSize = fontSize / letters,
                            fontWeight = FontWeight.ExtraBold,
                            style = LocalTextStyle.current.copy(fontFeatureSettings = "tnum")
                        )
                    }
                }
            }
        }
    }
}

@PreviewLightDark
//@PreviewScreenSizes
@Composable
private fun Preview() {
    MindMixTheme {
        Surface {
            GameUI(Game2048ViewModel(), rememberNavController(), SnackbarHostState())
        }
    }
}