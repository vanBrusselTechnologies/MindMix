package com.vanbrusselgames.mindmix.games.solitaire.model

import androidx.compose.animation.core.Animatable
import androidx.compose.animation.core.VectorConverter
import androidx.compose.runtime.mutableFloatStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.unit.IntOffset
import kotlin.math.roundToInt

data class PlayingCard(val type: CardType, val index: CardIndex, val drawableResId: Int) {
    val id = type.ordinal * 13 + index.ordinal

    var isMoving = false
    var stackId = 6
    var stackIndex = -1
    var baseOffset = IntOffset.Zero
    var offset = IntOffset.Zero

    val targetOffset = mutableStateOf(baseOffset + offset)
    val animOffset = Animatable(targetOffset.value, IntOffset.VectorConverter)

    val zIndex = mutableFloatStateOf(0f)
    val visible = mutableStateOf(true)
    var frontVisible = mutableStateOf(false)
    val isLast = mutableStateOf(false)

    private val _iconString = when (type) {
        CardType.CLOVERS -> "♣"
        CardType.DIAMONDS -> "♦"
        CardType.HEARTS -> "♥"
        CardType.SPADES -> "♠"
    }
    private val _indexString = when (index) {
        CardIndex.A -> "A"
        CardIndex.J -> "J"
        CardIndex.Q -> "Q"
        CardIndex.K -> "K"
        else -> (index.ordinal + 1).toString()
    }
    val contentString = _indexString + _iconString
    val contentLength = contentString.length

    val color = when (type) {
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

    fun reset(cardWidth: Float) {
        frontVisible.value = false
        isLast.value = false
        stackId = 6
        stackIndex = 0
        isMoving = false
        baseOffset = IntOffset((6 * cardWidth).roundToInt(), 0)
        targetOffset.value = baseOffset
    }
}