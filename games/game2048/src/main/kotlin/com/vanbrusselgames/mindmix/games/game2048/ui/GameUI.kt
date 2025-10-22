package com.vanbrusselgames.mindmix.games.game2048.ui

import androidx.compose.animation.animateColorAsState
import androidx.compose.animation.core.FastOutSlowInEasing
import androidx.compose.animation.core.LinearEasing
import androidx.compose.animation.core.tween
import androidx.compose.foundation.background
import androidx.compose.foundation.gestures.detectDragGestures
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.heightIn
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.layout.widthIn
import androidx.compose.foundation.lazy.grid.GridCells
import androidx.compose.foundation.lazy.grid.LazyGridItemScope
import androidx.compose.foundation.lazy.grid.LazyHorizontalGrid
import androidx.compose.foundation.lazy.grid.items
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.text.TextAutoSize
import androidx.compose.material3.LocalTextStyle
import androidx.compose.material3.MaterialTheme
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
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.TextStyle
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.tooling.preview.PreviewLightDark
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.compose.ui.util.fastRoundToInt
import androidx.navigation.NavController
import androidx.navigation.compose.rememberNavController
import com.vanbrusselgames.mindmix.core.common.ui.BaseScene
import com.vanbrusselgames.mindmix.core.designsystem.theme.MindMixTheme
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.games.game2048.R
import com.vanbrusselgames.mindmix.games.game2048.model.GridCell2048
import com.vanbrusselgames.mindmix.games.game2048.navigation.navigateToGame2048GameHelp
import com.vanbrusselgames.mindmix.games.game2048.navigation.navigateToGame2048GameMenu
import com.vanbrusselgames.mindmix.games.game2048.navigation.navigateToGame2048Settings
import com.vanbrusselgames.mindmix.games.game2048.viewmodel.IGame2048ViewModel
import com.vanbrusselgames.mindmix.games.game2048.viewmodel.MockGame2048ViewModel
import kotlin.math.sqrt

@Composable
fun GameUI(viewModel: IGame2048ViewModel, navController: NavController) {
    BaseScene(
        viewModel,
        { navController.navigateToGame2048GameHelp() },
        { navController.navigateToGame2048GameMenu() },
        { navController.navigateToGame2048Settings() }) {
        BlurBox(viewModel.blurStrength) {
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
fun BlurBox(blurStrength: Dp, content: @Composable () -> Unit) {
    Box(
        Modifier
            .fillMaxSize()
            .blur(if (SceneManager.dialogActiveState.value) blurStrength else 0.dp),
        Alignment.Center
    ) {
        content()
    }
}

@Composable
fun Grid2048(viewModel: IGame2048ViewModel, navController: NavController) {
    var totalDragOffset = remember { Offset.Zero }
    Box(
        Modifier
            .aspectRatio(1f)
            .pointerInput(Unit) {
                detectDragGestures(onDragStart = {
                    totalDragOffset = Offset.Zero
                }, onDragEnd = {
                    viewModel.handleDragGestures(navController, totalDragOffset)
                }, onDrag = { change, dragAmount ->
                    if (viewModel.finished) return@detectDragGestures
                    change.consume()
                    totalDragOffset += dragAmount
                    //Show effect what will happen when swipe will get finished -- Just effects
                })
            }) {

        val modifier = Modifier
            .aspectRatio(1f)
            .padding(2.dp)
            .clip(RoundedCornerShape(5.dp))
        LazyHorizontalGrid(
            GridCells.Fixed(sqrt(viewModel.cellList.size.toFloat()).fastRoundToInt()),
            Modifier
                .background(
                    MaterialTheme.colorScheme.secondaryContainer, RoundedCornerShape(5.dp)
                )
                .padding(3.dp),
            userScrollEnabled = false
        ) {
            items(viewModel.cellList, key = { it.id }) {
                GridCell2048(it, modifier)
            }
        }
    }
}

@Composable
fun LazyGridItemScope.GridCell2048(cell: GridCell2048, modifier: Modifier = Modifier) {
    val value = remember { mutableLongStateOf(cell.value) }
    val background by animateColorAsState(
        cell.background.value,
        label = "cell_${cell.id} background",
        finishedListener = { _ -> value.longValue = cell.value })
    Box(
        if (cell.value == 0L) modifier else modifier
            .animateItem(
                fadeInSpec = tween(200, 100, FastOutSlowInEasing),
                placementSpec = tween(300, 0, FastOutSlowInEasing),
                fadeOutSpec = tween(75, 25, LinearEasing)
            )
            .drawBehind { if (cell.value != 0L) drawRect(background) }, Alignment.Center
    ) {
        if (cell.value != 0L) {
            Text(
                text = value.longValue.toString(),
                autoSize = TextAutoSize.StepBased(maxFontSize = 1000.sp, minFontSize = 1.sp),
                style = LocalTextStyle.current.merge(
                    TextStyle(
                        color = if (value.longValue < 256) Color.Black else Color(220, 220, 220),
                        fontWeight = FontWeight.ExtraBold,
                        fontFeatureSettings = "tnum"
                    )
                ),
                maxLines = 1
            )
        }
    }
}

@PreviewLightDark
@Composable
private fun Preview() {
    MindMixTheme {
        Surface {
            val vm = remember { MockGame2048ViewModel() }
            GameUI(vm, rememberNavController())
        }
    }
}