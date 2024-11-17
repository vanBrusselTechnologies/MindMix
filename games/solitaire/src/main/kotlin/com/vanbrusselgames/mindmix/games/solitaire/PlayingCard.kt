package com.vanbrusselgames.mindmix.games.solitaire

import androidx.compose.animation.core.Animatable
import androidx.compose.animation.core.VectorConverter
import androidx.compose.animation.core.animateDpAsState
import androidx.compose.foundation.Image
import androidx.compose.foundation.clickable
import androidx.compose.foundation.gestures.detectDragGestures
import androidx.compose.foundation.interaction.MutableInteractionSource
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.IntrinsicSize
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.heightIn
import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.layout.widthIn
import androidx.compose.material3.LocalTextStyle
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.MutableState
import androidx.compose.runtime.mutableFloatStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.input.pointer.pointerInput
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.platform.LocalDensity
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.TextStyle
import androidx.compose.ui.text.style.LineHeightStyle
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.IntOffset
import androidx.compose.ui.unit.min
import androidx.compose.ui.unit.round
import androidx.compose.ui.unit.times
import androidx.compose.ui.zIndex
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.core.utils.PixelHelper
import com.vanbrusselgames.mindmix.core.utils.constants.StringEnum
import com.vanbrusselgames.mindmix.games.solitaire.PlayingCard.CardVisualType
import kotlin.math.roundToInt

data class PlayingCard(
    val viewModel: GameViewModel, val type: CardType, val index: CardIndex, val drawableResId: Int
) {
    val id = type.ordinal * 13 + index.ordinal

    var isMoving = false
    var stackId = 6
    var stackIndex = -1
    var baseOffset = IntOffset.Zero
    var offset = IntOffset.Zero

    val animOffset = Animatable(baseOffset, IntOffset.VectorConverter)

    val zIndex = mutableFloatStateOf(0f)
    val visible = mutableStateOf(true)
    var frontVisible = mutableStateOf(false)
    val isLast = mutableStateOf(false)

    val icon = when (type) {
        CardType.CLOVERS -> R.drawable.clover
        CardType.DIAMONDS -> R.drawable.diamonds
        CardType.HEARTS -> R.drawable.hearts
        CardType.SPADES -> R.drawable.spades
    }
    val indexString = when (index) {
        CardIndex.A -> "A"
        CardIndex.J -> "J"
        CardIndex.Q -> "Q"
        CardIndex.K -> "K"
        else -> (index.ordinal + 1).toString()
    }
    val indexColor = when (type) {
        CardType.CLOVERS -> Color.Black
        CardType.DIAMONDS -> Color.Red
        CardType.HEARTS -> Color.Red
        CardType.SPADES -> Color.Black
    }

    fun recalculateZIndex() {
        val stackBonus = if (stackId == 6) 0 else if (stackId == 5) 1 else if (stackId < 5) 2 else 3
        val movingBonus = if (isMoving && frontVisible.value) 10 else 0
        zIndex.floatValue = stackIndex * 0.01f + movingBonus + stackBonus
    }

    fun calculateBaseOffset() {
        val x = (stackId % 7) * viewModel.cardWidth
        val y =
            if (stackId < 7) 0f else (1 + (0.5f + stackIndex) * viewModel.distanceBetweenCards) * viewModel.cardHeight
        baseOffset = IntOffset(x.roundToInt(), y.roundToInt())
    }

    enum class CardType {
        CLOVERS, DIAMONDS, HEARTS, SPADES
    }

    enum class CardIndex {
        A, TWO, THREE, FOUR, FIVE, SIX, SEVEN, EIGHT, NINE, TEN, J, Q, K
    }

    enum class CardVisualType : StringEnum {
        DETAILED, SIMPLE;

        override fun getStringResource(): Int {
            return when (this) {
                DETAILED -> R.string.card_type_detailed
                SIMPLE -> R.string.card_type_simple
            }
        }
    }
}

@Composable
fun PlayingCard(model: PlayingCard, modifier: Modifier, navController: NavController) {
    val interactionSource = remember { MutableInteractionSource() }
    val onClickLabel =
        remember { "${model.type.name.replaceFirstChar { it.titlecase() }} ${model.indexString}" }
    val mod = modifier
        .zIndex(model.zIndex.floatValue)
        .offset { model.animOffset.value }
        .clickable(
            interactionSource,
            indication = null,
            onClickLabel = onClickLabel,
        ) {
            if (SceneManager.dialogActiveState.value) return@clickable
            if (model.stackId == 6) {
                model.viewModel.turnFromRestStack()
            } else {
                if (!model.frontVisible.value || model.animOffset.targetValue != model.baseOffset) return@clickable
                model.viewModel.startMoveCard(model)
                model.viewModel.onReleaseMovingCards(navController)
            }
        }
        .pointerInput(Unit) {
            detectDragGestures(onDragStart = {
                if (model.viewModel.finished || !model.frontVisible.value || SceneManager.dialogActiveState.value) return@detectDragGestures
                model.viewModel.startMoveCard(model)
            }, onDragCancel = {
                model.viewModel.onReleaseMovingCards(navController)
            }, onDragEnd = {
                model.viewModel.onReleaseMovingCards(navController)
            }, onDrag = { change, offset ->
                change.consume()
                model.viewModel.moveCards(offset.round())
            })
        }
    if (!model.visible.value) return Box(mod) {}
    when (model.viewModel.cardVisualType.value) {
        CardVisualType.DETAILED -> DetailedCard(model, mod)
        CardVisualType.SIMPLE -> SimpleCard(model, mod)
    }
}

@Composable
private fun DetailedCard(model: PlayingCard, modifier: Modifier) {
    Image(
        painterResource(if (model.frontVisible.value) model.drawableResId else R.drawable.playingcards_detailed_back),
        "Playing card",
        modifier
    )
}

@Composable
private fun SimpleCard(model: PlayingCard, modifier: Modifier) {
    Box(modifier, contentAlignment = Alignment.TopCenter) {
        SimpleCardBackground(model.frontVisible)
        if (!model.frontVisible.value) return@Box
        SimpleCardContent(model)
    }
}

@Composable
fun SimpleCardBackground(frontVisible: MutableState<Boolean>) {
    Image(
        painterResource(if (frontVisible.value) R.drawable.playingcards_base else R.drawable.playingcards_detailed_back),
        "Playing card",
        Modifier.fillMaxSize()
    )
}

@Composable
fun SimpleCardContent(model: PlayingCard) {
    val localDensity = LocalDensity.current
    val cardHeightInDpTarget = remember(model.isLast.value) {
        with(localDensity) {
            (model.viewModel.cardHeight * (if (model.isLast.value) 1f else model.viewModel.distanceBetweenCards)).toDp()
        }
    }
    val cardHeightInDp = animateDpAsState(cardHeightInDpTarget, label = "selectedFactor").value
    val cardWidthInDp = remember { with(localDensity) { model.viewModel.cardWidth.toDp() } }
    Row(
        Modifier
            .height(cardHeightInDp)
            .widthIn(max = cardWidthInDp),
        verticalAlignment = Alignment.CenterVertically,
        horizontalArrangement = Arrangement.Center
    ) {
        val maxWidth = remember(cardWidthInDp) {
            with(localDensity) { (cardWidthInDp * 0.45f).toPx() }
        }
        val maxHeight = remember(cardHeightInDp, cardWidthInDp) {
            with(localDensity) { min(cardHeightInDp * 0.8f, cardWidthInDp * 0.45f).toPx() }
        }
        val res = LocalContext.current.resources
        val fontSize = remember(maxWidth, maxHeight) {
            PixelHelper.pxToSp(
                res, minOf(
                    (maxWidth * 1.7f / ((model.indexString.length - 1) * 1.1f + 1)).roundToInt(),
                    (maxHeight * 0.95f).roundToInt()
                )
            )
        }
        Text(
            model.indexString,
            Modifier.width(IntrinsicSize.Max),
            model.indexColor,
            fontSize,
            textAlign = TextAlign.End,
            style = LocalTextStyle.current.merge(
                TextStyle(
                    lineHeightStyle = LineHeightStyle(
                        alignment = LineHeightStyle.Alignment.Center,
                        trim = LineHeightStyle.Trim.Both
                    ), fontSize = fontSize
                ),
            ),
            maxLines = 1,
        )
        Spacer(Modifier.width(0.05f * cardWidthInDp))
        Image(
            painterResource(model.icon),
            "type ${model.type.ordinal}",
            Modifier.heightIn(max = min(cardHeightInDp * 0.8f, cardWidthInDp * 0.4f))
        )
    }
}