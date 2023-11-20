package com.vanbrusselgames.mindmix.solitaire

import androidx.compose.runtime.mutableStateOf
import androidx.compose.ui.unit.IntOffset

data class PlayingCard(val type: CardType, val index: CardIndex, val drawableResId: Int) {
    val id = type.ordinal * 13 + index.ordinal

    private var _frontVisible = false
    val mutableFrontVisible = mutableStateOf(_frontVisible)
    var frontVisible
        get() = _frontVisible
        set(value) {
            _frontVisible = value
            mutableFrontVisible.value = value
        }

    private var _currentStackId = -1
    val mutableCurrentStackId = mutableStateOf(_currentStackId)
    var currentStackId
        get() = _currentStackId
        set(value) {
            _currentStackId = value
            mutableCurrentStackId.value = value
        }
    var currentStackIndex = -1

    private var _offset = IntOffset(0, 0)
    val mutableOffset = mutableStateOf(_offset)
    var offset
        get() = _offset
        set(value) {
            _offset = value
            mutableOffset.value = value
        }

    //val drawableResId = R.drawable::class.java.getDeclaredField(getDrawableResourceName()).getInt(null)
    // maybe possibility when in asset folder instead of resources. Because of detailed/simple/... types of cards
    /*private fun getDrawableResourceName(): String {
        val index = when (index) {
            CardIndex.A -> "a"
            CardIndex.J -> "j"
            CardIndex.Q -> "q"
            CardIndex.K -> "k"
            else -> (index.ordinal + 1).toString()
        }
        return "playingcards_detailed_${type.name.lowercase()}_${index}"
    }*/

    enum class CardType {
        CLOVERS, DIAMONDS, HEARTS, SPADES
    }

    enum class CardIndex {
        A, TWO, THREE, FOUR, FIVE, SIX, SEVEN, EIGHT, NINE, TEN, J, Q, K
    }
}

