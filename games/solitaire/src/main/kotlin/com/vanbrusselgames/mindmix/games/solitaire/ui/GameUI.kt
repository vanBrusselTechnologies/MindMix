package com.vanbrusselgames.mindmix.games.solitaire.ui

import androidx.compose.foundation.Image
import androidx.compose.foundation.gestures.detectDragGestures
import androidx.compose.foundation.gestures.detectTapGestures
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.BoxScope
import androidx.compose.foundation.layout.BoxWithConstraints
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.layout.width
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.Card
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.State
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.alpha
import androidx.compose.ui.draw.blur
import androidx.compose.ui.geometry.Rect
import androidx.compose.ui.geometry.Size
import androidx.compose.ui.input.pointer.pointerInput
import androidx.compose.ui.layout.onGloballyPositioned
import androidx.compose.ui.layout.positionInParent
import androidx.compose.ui.platform.LocalDensity
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.DpSize
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.round
import androidx.compose.ui.unit.times
import androidx.compose.ui.zIndex
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.common.ui.BaseScene
import com.vanbrusselgames.mindmix.core.common.ui.Timer
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.games.solitaire.R
import com.vanbrusselgames.mindmix.games.solitaire.navigation.navigateToSolitaireGameHelp
import com.vanbrusselgames.mindmix.games.solitaire.navigation.navigateToSolitaireGameMenu
import com.vanbrusselgames.mindmix.games.solitaire.navigation.navigateToSolitaireSettings
import com.vanbrusselgames.mindmix.games.solitaire.viewmodel.ISolitaireViewModel

@Composable
fun GameUI(viewModel: ISolitaireViewModel, navController: NavController) {
    BaseScene(
        viewModel,
        { navController.navigateToSolitaireGameHelp() },
        { navController.navigateToSolitaireGameMenu() },
        { navController.navigateToSolitaireSettings() }) {
        BlurBox(viewModel.blurStrength) { width, height ->
            val localDensity = LocalDensity.current
            val cardSize = remember(width, height, localDensity) {
                viewModel.onUpdateTableSize(width, height, localDensity)
            }

            SolitaireSpecificLayout(viewModel, cardSize) {
                viewModel.onReleaseMovingCards(navController)
            }
            FinishGameButton(viewModel.couldGetFinished) { viewModel.onClickFinishGame(navController) }
            Timer(viewModel.timer, Modifier.align(Alignment.BottomCenter))
        }
    }
}

@Composable
fun BlurBox(blurStrength: Dp, content: @Composable (maxWidth: Dp, maxHeight: Dp) -> Unit) {
    BoxWithConstraints(
        Modifier
            .fillMaxSize()
            .blur(if (SceneManager.dialogActiveState.value) blurStrength else 0.dp),
        Alignment.TopCenter
    ) {
        content(this.maxWidth, this.maxHeight)
    }
}

@Composable
fun SolitaireSpecificLayout(
    viewModel: ISolitaireViewModel, cardSize: DpSize, onReleaseMovingCards: () -> Unit
) {
    val modifier = Modifier.size(cardSize)
    Background(viewModel, modifier, cardSize.height)
    Foreground(viewModel, modifier, cardSize.width, onReleaseMovingCards)
}

@Composable
fun Foreground(
    viewModel: ISolitaireViewModel,
    modifier: Modifier,
    cardWidth: Dp,
    onReleaseMovingCards: () -> Unit
) {
    val coroutineScope = rememberCoroutineScope()
    var dragBounds = Rect.Zero
    Box(Modifier
        .width(cardWidth * 7)
        .fillMaxHeight()
        .onGloballyPositioned {
            val p = it.positionInParent()
            dragBounds = Rect(-p, Size(p.x * 2 + it.size.width, p.y * 2 + it.size.height))
        }
        .pointerInput(Unit) {
            detectDragGestures(onDragStart = {
                viewModel.onDragStart(it)
            }, onDragCancel = {
                onReleaseMovingCards()
            }, onDragEnd = {
                onReleaseMovingCards()
            }, onDrag = { change, offset ->
                change.consume()
                viewModel.moveCards(offset.round(), dragBounds, coroutineScope)
            })
        }
        .pointerInput(Unit) {
            detectTapGestures {
                viewModel.onTap(it) { onReleaseMovingCards() }
            }
        }) {
        viewModel.cards.forEach {
            PlayingCard(viewModel, it, modifier)
        }
    }
}

@Composable
fun Background(viewModel: ISolitaireViewModel, modifier: Modifier, cardHeight: Dp) {
    Column {
        Row {
            Card(modifier.alpha(0.75f)) { FoundationBackground(R.drawable.clover, 0) }
            Card(modifier.alpha(0.75f)) { FoundationBackground(R.drawable.diamonds, 1) }
            Card(modifier.alpha(0.75f)) { FoundationBackground(R.drawable.hearts, 2) }
            Card(modifier.alpha(0.75f)) { FoundationBackground(R.drawable.spades, 3) }
            Box(modifier)
            Box(modifier)

            if (viewModel.restStackEnabled.value) {
                Card(modifier.alpha(0.75f)) {
                    Icon(
                        painterResource(id = R.drawable.outline_autorenew_24),
                        "Stock",
                        Modifier.fillMaxSize()
                    )
                }
            } else Box(modifier)
        }

        Spacer(Modifier.height(0.5f * viewModel.distanceBetweenCards * cardHeight))

        Row { repeat(7) { Card(modifier.alpha(0.5f)) {} } }
    }
}

@Composable
fun FoundationBackground(resourceId: Int, stackIndex: Int) {
    Box(Modifier.fillMaxSize(), Alignment.Center) {
        Image(painterResource(resourceId), "pile $stackIndex", Modifier.fillMaxSize(0.9f))
    }
}

@Composable
fun BoxScope.FinishGameButton(
    couldGetFinished: State<Boolean>, onClickListener: () -> Unit
) {
    if (couldGetFinished.value) {
        Button(
            onClickListener,
            Modifier
                .align(Alignment.BottomCenter)
                .zIndex(20f),
            colors = ButtonDefaults.buttonColors(
                containerColor = MaterialTheme.colorScheme.primary,
                contentColor = MaterialTheme.colorScheme.onPrimary,
                disabledContainerColor = MaterialTheme.colorScheme.primary,
                disabledContentColor = MaterialTheme.colorScheme.onPrimary,
            )
        ) {
            Icon(painterResource(R.drawable.baseline_flag_24), "Finish flag")
            Spacer(Modifier.width(2.dp))
            Text(stringResource(R.string.finish_game))
        }
    }
}