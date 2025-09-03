package com.vanbrusselgames.mindmix.feature.menu.ui

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
import androidx.compose.ui.platform.LocalDensity
import androidx.compose.ui.platform.LocalWindowInfo
import androidx.compose.ui.tooling.preview.PreviewScreenSizes
import androidx.compose.ui.unit.dp
import androidx.navigation.NavController
import androidx.navigation.compose.rememberNavController
import com.vanbrusselgames.mindmix.core.model.SceneRegistry
import com.vanbrusselgames.mindmix.feature.menu.model.GameWheel
import com.vanbrusselgames.mindmix.feature.menu.viewmodel.IMenuScreenViewModel
import com.vanbrusselgames.mindmix.feature.menu.viewmodel.MockMenuScreenViewModel
import kotlinx.coroutines.launch

@Composable
fun GameWheel(viewModel: IMenuScreenViewModel, navController: NavController, model: GameWheel) {
    val coroutineScope = rememberCoroutineScope()
    val interactionSource = remember { MutableInteractionSource() }
    model.setGrowthFactor(LocalDensity.current, LocalWindowInfo.current)
    Box(
        contentAlignment = Alignment.BottomCenter, modifier = Modifier
            .fillMaxSize()
            .draggable(
                rememberDraggableState { coroutineScope.launch { model.rotate(it) } },
                Orientation.Horizontal,
                true,
                interactionSource,
                onDragStarted = { model.startRotate() },
                onDragStopped = { coroutineScope.launch { model.updateSelectedIndex() } })
    ) {
        Box(
            contentAlignment = Alignment.Center,
            modifier = Modifier
                .height(350.dp)
                .offset(0.dp, model.radius.dp)
                .graphicsLayer {
                    transformOrigin = TransformOrigin.Center
                    rotationZ = model.anim.value
                }) {
            for (wheelItem in model.items) {
                WheelItem(viewModel, navController, wheelItem)
            }
        }
    }
}

@PreviewScreenSizes
@Composable
private fun PrevWheel() {
    val vm = remember { MockMenuScreenViewModel() }
    vm.selectedGame = SceneRegistry.Sudoku
    GameWheel(vm, rememberNavController(), GameWheel(vm, 3))
}