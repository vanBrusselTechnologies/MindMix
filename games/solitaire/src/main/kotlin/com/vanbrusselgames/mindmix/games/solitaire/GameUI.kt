package com.vanbrusselgames.mindmix.games.solitaire

import androidx.compose.foundation.Image
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.BoxWithConstraints
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.width
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.Card
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.alpha
import androidx.compose.ui.draw.blur
import androidx.compose.ui.platform.LocalDensity
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.times
import androidx.compose.ui.zIndex
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.common.BaseScene
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import kotlinx.coroutines.launch
import kotlin.math.min

@Composable
fun GameUI(viewModel: GameViewModel, navController: NavController) {
    BaseScene(viewModel, navController) {
        SolitaireSpecificLayout(viewModel, navController)
        /*GameFinished_Old.Screen(
            viewModel,
            onClickPlayAgain = { viewModel.startNewGame() }) {
            Text(text = "${stringResource(R.string.time)}${viewModel.timer.formatTime(true)}")
            //"""You did great and solved puzzle in ${0} seconds!!
            //     |That's Awesome!
            //     |Share with your friends and challenge them to beat your time!""".trimMargin()
        }*/
        Box(
            Modifier
                .fillMaxSize()
                .blur(if (SceneManager.dialogActiveState.value) viewModel.blurStrength else 0.dp),
            Alignment.BottomCenter
        ) {
            viewModel.timer.Timer(viewModel)
        }
    }
}

@Composable
fun SolitaireSpecificLayout(viewModel: GameViewModel, navController: NavController) {
    Box(
        Modifier
            .fillMaxSize()
            .blur(if (SceneManager.dialogActiveState.value) viewModel.blurStrength else 0.dp),
        Alignment.Center
    ) {
        Box(Modifier.fillMaxSize(0.95f), contentAlignment = Alignment.TopCenter) {
            BoxWithConstraints {
                val maxHeight = this.constraints.maxHeight
                val maxWidth = this.constraints.maxWidth
                val maxCardHeight = maxHeight / 4.2375f
                viewModel.cardHeight =
                    if (maxWidth / maxHeight > GameViewModel.CARD_ASPECT_RATIO) maxCardHeight else maxWidth / 7f / GameViewModel.CARD_ASPECT_RATIO
                viewModel.cardWidth = viewModel.cardHeight * GameViewModel.CARD_ASPECT_RATIO
                viewModel.distanceBetweenCards =
                    min(maxCardHeight / viewModel.cardHeight, 2f) * 0.175f
                val cardHeightInDp = with(LocalDensity.current) { viewModel.cardHeight.toDp() }
                val modifier = Modifier
                    .width(cardHeightInDp * GameViewModel.CARD_ASPECT_RATIO)
                    .height(cardHeightInDp)
                    .aspectRatio(GameViewModel.CARD_ASPECT_RATIO)

                Background(viewModel, modifier, cardHeightInDp)

                val coroutineScope = rememberCoroutineScope()
                viewModel.cards.forEach {
                    coroutineScope.launch {
                        it.calculateBaseOffset()
                        it.animOffset.animateTo(it.baseOffset)
                        it.isMoving = false
                        it.recalculateZIndex()
                    }
                    PlayingCard(it, modifier, navController)
                }

                if (viewModel.couldGetFinished.value) {
                    Button(
                        onClick = {
                            viewModel.cards.forEach {
                                it.stackId = it.type.ordinal
                                it.stackIndex = it.index.ordinal
                                it.isMoving = false
                                it.isLast.value = true
                                it.frontVisible.value = true
                                it.recalculateZIndex()
                                it.calculateBaseOffset()
                            }
                            viewModel.checkFinished(navController)
                        },
                        Modifier
                            .align(Alignment.BottomCenter)
                            .zIndex(20f),
                        enabled = !SceneManager.dialogActiveState.value,
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
        }
    }
}

@Composable
fun Background(viewModel: GameViewModel, modifier: Modifier, cardHeight: Dp) {
    Column {
        BackgroundTopRow(viewModel, modifier)
        Spacer(Modifier.height(0.5f * viewModel.distanceBetweenCards * cardHeight))
        BackgroundLowerRow(viewModel, modifier, cardHeight)
    }
}

@Composable
fun BackgroundTopRow(viewModel: GameViewModel, modifier: Modifier) {
    val coroutineScope = rememberCoroutineScope()
    Row {
        for (i in 0 until 7) {
            when (i) {
                4 -> Box(modifier)
                5 -> Box(modifier)
                6 -> if (!viewModel.restStackEnabled.value) Box(modifier) else Card(
                    { viewModel.resetRestStack(coroutineScope) },
                    modifier.alpha(0.75f),
                    !SceneManager.dialogActiveState.value
                ) {
                    Box(Modifier.fillMaxSize()) {
                        Icon(
                            painterResource(id = R.drawable.outline_autorenew_24),
                            "Stock",
                            Modifier
                                .align(Alignment.Center)
                                .fillMaxSize()
                        )
                    }
                }

                else -> Card(modifier.alpha(0.75f)) {
                    val resourceId = when (i) {
                        0 -> R.drawable.clover
                        1 -> R.drawable.diamonds
                        2 -> R.drawable.hearts
                        3 -> R.drawable.spades
                        else -> 0
                    }
                    FoundationBackground(resourceId, i)
                }
            }
        }
    }
}

@Composable
fun BackgroundLowerRow(viewModel: GameViewModel, modifier: Modifier, cardHeight: Dp) {
    Row(Modifier.height(18 * viewModel.distanceBetweenCards * cardHeight)) {
        for (i in 0 until 7) {
            Card(modifier.alpha(0.5f)) {}
        }
    }
}

@Composable
fun FoundationBackground(resourceId: Int, stackIndex: Int) {
    Box(Modifier.fillMaxSize()) {
        if (resourceId != 0) {
            Image(
                painterResource(id = resourceId),
                "pile $stackIndex",
                Modifier
                    .fillMaxSize(0.9f)
                    .align(Alignment.Center)
            )
        }
    }
}