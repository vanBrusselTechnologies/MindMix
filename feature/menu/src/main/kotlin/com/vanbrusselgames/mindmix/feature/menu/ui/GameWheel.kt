package com.vanbrusselgames.mindmix.feature.menu.ui

import androidx.compose.animation.AnimatedContent
import androidx.compose.animation.AnimatedContentScope
import androidx.compose.animation.ExperimentalSharedTransitionApi
import androidx.compose.animation.SharedTransitionLayout
import androidx.compose.animation.SharedTransitionScope
import androidx.compose.foundation.gestures.Orientation
import androidx.compose.foundation.gestures.draggable
import androidx.compose.foundation.gestures.rememberDraggableState
import androidx.compose.foundation.interaction.MutableInteractionSource
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.BoxWithConstraints
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.layout.size
import androidx.compose.runtime.Composable
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.TransformOrigin
import androidx.compose.ui.graphics.graphicsLayer
import androidx.compose.ui.semantics.CollectionInfo
import androidx.compose.ui.semantics.Role
import androidx.compose.ui.semantics.ScrollAxisRange
import androidx.compose.ui.semantics.collectionInfo
import androidx.compose.ui.semantics.contentDescription
import androidx.compose.ui.semantics.horizontalScrollAxisRange
import androidx.compose.ui.semantics.role
import androidx.compose.ui.semantics.scrollBy
import androidx.compose.ui.semantics.semantics
import androidx.compose.ui.semantics.stateDescription
import androidx.compose.ui.tooling.preview.PreviewScreenSizes
import androidx.compose.ui.unit.dp
import androidx.navigation.NavController
import androidx.navigation.compose.rememberNavController
import com.vanbrusselgames.mindmix.core.model.SceneRegistry
import com.vanbrusselgames.mindmix.feature.menu.model.GameWheel
import com.vanbrusselgames.mindmix.feature.menu.viewmodel.IMenuScreenViewModel
import com.vanbrusselgames.mindmix.feature.menu.viewmodel.MockMenuScreenViewModel
import kotlinx.coroutines.launch

@OptIn(ExperimentalSharedTransitionApi::class)
@Composable
fun SharedTransitionScope.GameWheel(
    viewModel: IMenuScreenViewModel,
    navController: NavController,
    animatedContentScope: AnimatedContentScope,
    model: GameWheel
) {
    val coroutineScope = rememberCoroutineScope()
    val interactionSource = remember { MutableInteractionSource() }

    BoxWithConstraints(
        contentAlignment = Alignment.BottomCenter,
        modifier = Modifier
            .fillMaxSize(1f)
            .semantics {
                this.contentDescription = "Horizontal Game Selection Wheel"
                // TODO: stringResource(R.string.accessibility_content_description_horizontal_game_selection_wheel)
                val selectedGameName = viewModel.selectedGame.value.name
                this.stateDescription = "Selected $selectedGameName"
                // TODO: stringResource(R.string.accessibility_state_selected_game, selectedGameName)
                this.collectionInfo = CollectionInfo(rowCount = 1, columnCount = model.gameCount)
                this.role = Role.ValuePicker
                this.horizontalScrollAxisRange = ScrollAxisRange(
                    value = { model.selectedIndex.toFloat() },
                    maxValue = { Float.POSITIVE_INFINITY })
                this.scrollBy { x, y -> true }
            }
            .draggable(
                rememberDraggableState { coroutineScope.launch { model.rotate(it) } },
                Orientation.Horizontal,
                true,
                interactionSource,
                onDragStarted = { model.startRotate() },
                onDragStopped = { coroutineScope.launch { model.updateSelectedIndex() } })
    ) {
        val wheelOffsetY = if (this.maxHeight > model.radius.dp * 2f) {
            model.radius.dp * 2f
        } else {
            this.maxHeight / 2f + model.radius.dp
        }
        val playButtonOffsetY = (48 + 5 * 2).dp
        val maxWheelItemHeight = this.maxHeight - playButtonOffsetY
        Box(
            contentAlignment = Alignment.Center,
            modifier = Modifier
                .size(model.radius.dp * 2)
                .offset(0.dp, wheelOffsetY - playButtonOffsetY)
                .graphicsLayer {
                    transformOrigin = TransformOrigin.Center
                    rotationZ = model.anim.value
                }) {
            for (wheelItem in model.items) {
                WheelItem(
                    viewModel, navController, animatedContentScope, wheelItem, maxWheelItemHeight
                )
            }
        }
    }
}

@OptIn(ExperimentalSharedTransitionApi::class)
@PreviewScreenSizes
@Composable
private fun PrevWheel() {
    SharedTransitionLayout {
        AnimatedContent(null) {
            it
            val vm = remember { MockMenuScreenViewModel() }
            vm.selectedGame.value = SceneRegistry.Sudoku
            GameWheel(vm, rememberNavController(), this@AnimatedContent, GameWheel(vm, 3))
        }
    }
}