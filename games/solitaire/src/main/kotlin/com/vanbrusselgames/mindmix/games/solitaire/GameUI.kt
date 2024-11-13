package com.vanbrusselgames.mindmix.games.solitaire

import androidx.compose.foundation.Image
import androidx.compose.foundation.layout.Box
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
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.alpha
import androidx.compose.ui.draw.blur
import androidx.compose.ui.platform.LocalConfiguration
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
import kotlin.math.min

@Composable
fun GameUI(viewModel: GameViewModel, navController: NavController) {
    BaseScene(viewModel, navController) {
        SolitaireSpecificLayout(viewModel, navController)
        Box(
            Modifier
                .align(Alignment.BottomCenter)
                .blur(if (SceneManager.dialogActiveState.value) viewModel.blurStrength else 0.dp)
        ) {
            viewModel.timer.Timer()
        }
    }
}

@Composable
fun SolitaireSpecificLayout(viewModel: GameViewModel, navController: NavController) {
    Box(
        Modifier
            .fillMaxSize()
            .blur(if (SceneManager.dialogActiveState.value) viewModel.blurStrength else 0.dp),
        Alignment.TopCenter
    ) {
        val localConfig = LocalConfiguration.current
        val localDensity = LocalDensity.current
        val maxWidthDp = localConfig.screenWidthDp.dp * 0.95f
        val maxHeightDp = localConfig.screenHeightDp.dp * 0.95f
        val cardHeightInDp = remember(maxWidthDp, maxHeightDp) {
            val maxCardHeightDp = maxHeightDp / 4.2375f
            val cardHeightDp = if (maxWidthDp / maxHeightDp > GameViewModel.CARD_ASPECT_RATIO) {
                maxCardHeightDp
            } else maxWidthDp / 7f / GameViewModel.CARD_ASPECT_RATIO

            viewModel.cardHeight = with(localDensity) { (cardHeightDp).toPx() }
            viewModel.cardWidth = viewModel.cardHeight * GameViewModel.CARD_ASPECT_RATIO
            viewModel.distanceBetweenCards = min(maxCardHeightDp / cardHeightDp, 2f) * 0.175f

            cardHeightDp
        }
        val modifier = Modifier
            .width(cardHeightInDp * GameViewModel.CARD_ASPECT_RATIO)
            .height(cardHeightInDp)
            .aspectRatio(GameViewModel.CARD_ASPECT_RATIO)
        Background(viewModel, modifier, cardHeightInDp)

        Box(Modifier.width(cardHeightInDp * GameViewModel.CARD_ASPECT_RATIO * 7)) {
            viewModel.cards.forEach {
                LaunchedEffect(it.id) {
                    it.calculateBaseOffset()
                    it.recalculateZIndex()
                    it.isMoving = false
                    it.animOffset.animateTo(it.baseOffset)
                }
                PlayingCard(it, modifier, navController)
            }
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
    Row {
        Card(modifier.alpha(0.75f)) { FoundationBackground(R.drawable.clover, 0) }
        Card(modifier.alpha(0.75f)) { FoundationBackground(R.drawable.diamonds, 1) }
        Card(modifier.alpha(0.75f)) { FoundationBackground(R.drawable.hearts, 2) }
        Card(modifier.alpha(0.75f)) { FoundationBackground(R.drawable.spades, 3) }
        Box(modifier)
        Box(modifier)

        if (viewModel.restStackEnabled.value) {
            val coroutineScope = rememberCoroutineScope()
            Card(
                { viewModel.resetRestStack(coroutineScope) },
                modifier.alpha(0.75f),
                !SceneManager.dialogActiveState.value
            ) {
                Icon(
                    painterResource(id = R.drawable.outline_autorenew_24),
                    "Stock",
                    Modifier.fillMaxSize()
                )
            }
        } else Box(modifier)
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
    Box(Modifier.fillMaxSize(), Alignment.Center) {
        Image(painterResource(resourceId), "pile $stackIndex", Modifier.fillMaxSize(0.9f))
    }
}