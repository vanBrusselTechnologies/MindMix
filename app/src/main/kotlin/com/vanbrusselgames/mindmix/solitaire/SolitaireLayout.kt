package com.vanbrusselgames.mindmix.solitaire

import androidx.compose.animation.core.animateIntOffsetAsState
import androidx.compose.foundation.Image
import androidx.compose.foundation.clickable
import androidx.compose.foundation.gestures.detectDragGestures
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.BoxWithConstraints
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.lazy.LazyRow
import androidx.compose.foundation.lazy.items
import androidx.compose.material3.Card
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.alpha
import androidx.compose.ui.input.pointer.pointerInput
import androidx.compose.ui.platform.LocalDensity
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.IntOffset
import androidx.compose.ui.unit.times
import androidx.compose.ui.zIndex
import com.vanbrusselgames.mindmix.BaseLayout
import com.vanbrusselgames.mindmix.BaseUIHandler
import com.vanbrusselgames.mindmix.R
import kotlin.math.roundToInt

class SolitaireLayout : BaseLayout() {
    companion object {
        private const val cardPixelHeight = 819f
        private const val cardPixelWidth = 566f
        private const val cardAspectRatio = cardPixelWidth / cardPixelHeight
        private const val distanceBetweenCards = 0.175f
        private var cardHeight = 0f
        var cardWidth = 0f

        fun calculateBaseOffsetByStackData(stackId: Int, stackIndex: Int): IntOffset {
            val x = (stackId % 7) * cardWidth
            val y =
                if (stackId < 7) 0f else (1 + (0.5f + stackIndex) * distanceBetweenCards) * cardHeight
            return IntOffset(x.roundToInt(), y.roundToInt())
        }
    }

    override var uiHandler: BaseUIHandler = SolitaireUIHandler()
    private val arr = arrayOf(0, 1, 2, 3, 4, 5, 6)

    @Composable
    fun BaseScene() {
        super.BaseScene(isMenu = false, sceneSpecific = {
            SolitaireSpecificLayout()
        })
    }

    @Composable
    fun SolitaireSpecificLayout() {
        Box(Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
            Box(Modifier.fillMaxSize(0.95f), contentAlignment = Alignment.TopCenter) {
                BoxWithConstraints {
                    val maxHeight = constraints.maxHeight
                    val maxWidth = constraints.maxWidth
                    cardHeight =
                        if (maxWidth / maxHeight > cardAspectRatio) maxHeight / 4.2375f else maxWidth / 7f / cardAspectRatio
                    cardWidth = cardHeight * cardAspectRatio
                    val cardHeightInDp = with(LocalDensity.current) { cardHeight.toDp() }
                    val modifier = Modifier
                        .width(cardHeightInDp * cardAspectRatio)
                        .height(cardHeightInDp)
                        .aspectRatio(cardAspectRatio)

                    Background(modifier = modifier, cardHeight = cardHeightInDp)

                    SolitaireManager.cards.forEach { PlayingCard(it, modifier) }
                }
            }
        }
    }

    @Composable
    fun Background(modifier: Modifier, cardHeight: Dp) {
        Column {
            BackgroundTopRow(modifier)
            Spacer(Modifier.height(0.5f * distanceBetweenCards * cardHeight))
            BackgroundLowerRow(modifier, cardHeight)
        }
    }

    @Composable
    fun BackgroundTopRow(modifier: Modifier) {
        LazyRow {
            items(items = arr) { i ->
                when (i) {
                    4 -> Box(modifier)
                    5 -> Box(modifier)
                    6 -> Card(modifier.alpha(0.75f)) {
                        Box(
                            Modifier
                                .fillMaxSize()
                                .clickable {
                                    SolitaireManager.resetRestStack()
                                }) {
                            StackBackground(R.drawable.reload_sign, i)
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
                        StackBackground(resourceId, i)
                    }
                }
            }
        }
    }

    @Composable
    fun BackgroundLowerRow(modifier: Modifier, cardHeight: Dp) {
        LazyRow(Modifier.height(18 * distanceBetweenCards * cardHeight)) {
            items(items = arr) { _ ->
                Card(modifier.alpha(0.5f)) {}
            }
        }
    }

    @Composable
    fun PlayingCard(card: PlayingCard, modifier: Modifier) {
        val currentStackId by card.mutableCurrentStackId
        val baseOffset = calculateBaseOffsetByStackData(currentStackId, card.currentStackIndex)
        val offset by animateIntOffsetAsState(
            targetValue = baseOffset + card.mutableOffset.value, label = "moveCardToStack"
        )
        val zIndex =
            card.currentStackIndex * 0.01f + if (currentStackId == 6 || offset == baseOffset) 0 else if(currentStackId == 5) 10 else 20
        Image(painterResource(if (card.mutableFrontVisible.value) card.drawableResId else R.drawable.playingcards_detailed_back),
            "Playing card",
            modifier
                .zIndex(zIndex)
                .offset { offset }
            .clickable(enabled = !SolitaireManager.solitaireFinished.value) {
                    if (currentStackId == 6) {
                        SolitaireManager.turnFromRestStack(card)
                    } else {
                        if (!card.frontVisible || zIndex >= 20f) return@clickable
                        SolitaireManager.startMoveCard(card)
                        SolitaireManager.onReleaseMovingCards()
                    }
                }
                .pointerInput(Unit) {
                    detectDragGestures(onDragStart = {
                        if (SolitaireManager.finished || !card.frontVisible) return@detectDragGestures
                        SolitaireManager.startMoveCard(card)
                    }, onDragCancel = {
                        SolitaireManager.onReleaseMovingCards()
                    }, onDragEnd = {
                        SolitaireManager.onReleaseMovingCards()
                    }, onDrag = { change, offset ->
                        change.consume()
                        SolitaireManager.moveCards(offset)
                    })
                })
    }

    @Composable
    fun StackBackground(resourceId: Int, stackIndex: Int) {
        Box(Modifier.fillMaxSize()) {
            if (resourceId != 0) {
                Image(
                    painterResource(id = resourceId),
                    "stack $stackIndex",
                    Modifier
                        .fillMaxSize(0.9f)
                        .align(Alignment.Center)
                )
            }
        }
    }
}