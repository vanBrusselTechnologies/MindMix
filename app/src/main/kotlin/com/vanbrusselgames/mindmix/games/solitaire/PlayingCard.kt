package com.vanbrusselgames.mindmix.games.solitaire

import androidx.compose.animation.core.animateFloatAsState
import androidx.compose.animation.core.animateIntOffsetAsState
import androidx.compose.foundation.Image
import androidx.compose.foundation.clickable
import androidx.compose.foundation.gestures.detectDragGestures
import androidx.compose.foundation.gestures.detectTapGestures
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.BoxWithConstraints
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.heightIn
import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.layout.width
import androidx.compose.material3.LocalTextStyle
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.input.pointer.pointerInput
import androidx.compose.ui.platform.LocalDensity
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.TextStyle
import androidx.compose.ui.text.style.LineHeightStyle
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.IntOffset
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.min
import androidx.compose.ui.unit.times
import androidx.compose.ui.zIndex
import com.vanbrusselgames.mindmix.BaseLayout
import com.vanbrusselgames.mindmix.PixelHelper
import com.vanbrusselgames.mindmix.R
import kotlin.math.roundToInt

data class PlayingCard(val type: CardType, val index: CardIndex, val drawableResId: Int) {
    val id = type.ordinal * 13 + index.ordinal

    private val mutableFrontVisible = mutableStateOf(false)
    var frontVisible = false
        set(value) {
            field = value
            mutableFrontVisible.value = value
        }

    private val mutableCurrentStackId = mutableIntStateOf(6)
    var currentStackId = 6
        set(value) {
            field = value
            mutableCurrentStackId.intValue = value
        }
    var currentStackIndex = -1

    private val mutableOffset = mutableStateOf(IntOffset.Zero)
    var offset = IntOffset.Zero
        set(value) {
            field = value
            mutableOffset.value = value
        }

    private val mutableIsLast = mutableStateOf(false)
    var isLast = false
        set(value) {
            field = value
            mutableIsLast.value = value
        }

    private val icon = when (type) {
        CardType.CLOVERS -> R.drawable.clover
        CardType.DIAMONDS -> R.drawable.diamonds
        CardType.HEARTS -> R.drawable.hearts
        CardType.SPADES -> R.drawable.spades
    }

    private val indexString = when (index) {
        CardIndex.A -> "A"
        CardIndex.J -> "J"
        CardIndex.Q -> "Q"
        CardIndex.K -> "K"
        else -> (index.ordinal + 1).toString()
    }

    private val indexColor = when (type) {
        CardType.CLOVERS -> Color.Black
        CardType.DIAMONDS -> Color.Red
        CardType.HEARTS -> Color.Red
        CardType.SPADES -> Color.Black
    }

    enum class CardType {
        CLOVERS, DIAMONDS, HEARTS, SPADES
    }

    enum class CardIndex {
        A, TWO, THREE, FOUR, FIVE, SIX, SEVEN, EIGHT, NINE, TEN, J, Q, K
    }

    enum class CardVisualType {
        DETAILED, SIMPLE
    }

    @Composable
    fun Composable(modifier: Modifier) {
        val currentStackId by mutableCurrentStackId
        val baseOffset =
            SolitaireLayout.calculateBaseOffsetByStackData(currentStackId, currentStackIndex)
        val offset by animateIntOffsetAsState(
            targetValue = baseOffset + mutableOffset.value, label = "moveCardToStack"
        )
        val zIndex =
            currentStackIndex * 0.01f + if (currentStackId == 6 || offset == baseOffset) 0 else if (currentStackId == 5) 10 else 20
        val mod = modifier
            .zIndex(zIndex)
            .offset { offset }
            .clickable(onClickLabel = "${type.name.replaceFirstChar { it.titlecase() }} $indexString") {
                if (SolitaireManager.solitaireFinished.value || BaseLayout.disableTopRowButtons.value) return@clickable
                if (currentStackId == 6) {
                    SolitaireManager.turnFromRestStack(this)
                } else {
                    if (!frontVisible || zIndex >= 20f) return@clickable
                    SolitaireManager.startMoveCard(this)
                    SolitaireManager.onReleaseMovingCards()
                }
            }
            .pointerInput(Unit) {
                //todo: .clickable{} to .pointerInput{detectTapGestures {  }}
                detectDragGestures(onDragStart = {
                    if (SolitaireManager.finished || !frontVisible || BaseLayout.disableTopRowButtons.value) return@detectDragGestures
                    SolitaireManager.startMoveCard(this@PlayingCard)
                }, onDragCancel = {
                    SolitaireManager.onReleaseMovingCards()
                }, onDragEnd = {
                    SolitaireManager.onReleaseMovingCards()
                }, onDrag = { change, offset ->
                    change.consume()
                    SolitaireManager.moveCards(offset)
                })
            }
        if (SolitaireManager.cardVisualType.value == CardVisualType.DETAILED) DetailedCard(mod)
        else SimpleCard(mod)
    }

    @Composable
    private fun DetailedCard(modifier: Modifier) {
        Image(
            painterResource(if (mutableFrontVisible.value) drawableResId else R.drawable.playingcards_detailed_back),
            "Playing card",
            modifier
        )
    }

    @Composable
    private fun SimpleCard(modifier: Modifier) {
        Box(modifier, contentAlignment = Alignment.TopCenter) {
            Image(
                painterResource(if (mutableFrontVisible.value) R.drawable.playingcards_base else R.drawable.playingcards_detailed_back),
                "Playing card",
                Modifier.fillMaxSize()
            )
            if (!mutableFrontVisible.value) return@Box
            val cardHeightInDpTarget = with(LocalDensity.current) {
                if (mutableIsLast.value) SolitaireLayout.cardHeight.toDp()
                else (SolitaireLayout.distanceBetweenCards * SolitaireLayout.cardHeight).toDp()
            }
            val cardHeightInDp = (animateFloatAsState(
                targetValue = cardHeightInDpTarget.value, label = "selectedFactor"
            )).value.dp
            val cardWidthInDp = with(LocalDensity.current) {
                SolitaireLayout.cardWidth.toDp()
            }
            Box(
                modifier = Modifier
                    .height(cardHeightInDp)
                    .width(cardWidthInDp),
                contentAlignment = Alignment.Center
            ) {
                Row(
                    verticalAlignment = Alignment.CenterVertically,
                    horizontalArrangement = Arrangement.Center
                ) {
                    BoxWithConstraints(
                        Modifier
                            .width(0.45f * cardWidthInDp)
                            .heightIn(max = min(cardHeightInDp * 0.8f, cardWidthInDp * 0.45f))
                    ) {
                        val c = constraints
                        val fontSize = PixelHelper.pxToSp(
                            minOf(
                                (c.maxWidth * 1.7f / indexString.length).roundToInt(),
                                (c.maxHeight * 0.95f).roundToInt()
                            )
                        )
                        Text(
                            indexString,
                            fontSize = fontSize,
                            color = indexColor,
                            textAlign = TextAlign.End,
                            modifier = Modifier.fillMaxSize(),
                            style = LocalTextStyle.current.merge(
                                TextStyle(
                                    lineHeightStyle = LineHeightStyle(
                                        alignment = LineHeightStyle.Alignment.Center,
                                        trim = LineHeightStyle.Trim.Both
                                    )
                                ),
                            )
                        )
                    }
                    Spacer(Modifier.width(0.05f * cardWidthInDp))
                    Box(
                        Modifier
                            .width(0.5f * cardWidthInDp)
                            .heightIn(max = min(cardHeightInDp * 0.8f, cardWidthInDp * 0.4f)),
                        contentAlignment = Alignment.CenterStart
                    ) {
                        Image(
                            painter = painterResource(icon), "type ${type.ordinal}"
                        )
                    }
                }
            }
        }
    }
}


