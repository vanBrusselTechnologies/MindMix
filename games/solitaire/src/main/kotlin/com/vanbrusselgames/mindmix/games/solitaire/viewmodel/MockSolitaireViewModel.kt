package com.vanbrusselgames.mindmix.games.solitaire.viewmodel

import android.app.Activity
import androidx.compose.runtime.MutableState
import androidx.compose.runtime.mutableStateOf
import androidx.compose.ui.geometry.Offset
import androidx.compose.ui.unit.Density
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.DpSize
import androidx.compose.ui.unit.IntOffset
import androidx.compose.ui.unit.dp
import androidx.core.math.MathUtils
import androidx.lifecycle.viewModelScope
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.common.BaseGameViewModel
import com.vanbrusselgames.mindmix.core.common.GameTimer
import com.vanbrusselgames.mindmix.core.common.ITimerVM
import com.vanbrusselgames.mindmix.games.solitaire.R
import com.vanbrusselgames.mindmix.games.solitaire.model.CardIndex
import com.vanbrusselgames.mindmix.games.solitaire.model.CardType
import com.vanbrusselgames.mindmix.games.solitaire.model.CardVisualType
import com.vanbrusselgames.mindmix.games.solitaire.model.FinishedGame
import com.vanbrusselgames.mindmix.games.solitaire.model.PlayingCard
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import kotlin.math.ceil
import kotlin.math.floor
import kotlin.math.min
import kotlin.math.roundToInt

class MockSolitaireViewModel : BaseGameViewModel(), ISolitaireViewModel, ITimerVM {
    companion object {
        private const val CARD_PIXEL_HEIGHT = 819f
        private const val CARD_PIXEL_WIDTH = 566f
        const val CARD_ASPECT_RATIO = CARD_PIXEL_WIDTH / CARD_PIXEL_HEIGHT
    }

    override val timer = GameTimer()

    override val finishedGame = mutableStateOf(FinishedGame())
    override val cardVisualType = mutableStateOf(CardVisualType.SIMPLE)
    override val couldGetFinished = mutableStateOf(false)
    override val restStackEnabled = mutableStateOf(true)

    override val preferencesLoaded = MutableStateFlow(false).asStateFlow()
    override val puzzleLoaded = MutableStateFlow(false).asStateFlow()

    override val cards = arrayOf(
        PlayingCard(CardType.CLOVERS, CardIndex.A, R.drawable.playingcards_detailed_clovers_a),
        PlayingCard(CardType.CLOVERS, CardIndex.TWO, R.drawable.playingcards_detailed_clovers_2),
        PlayingCard(CardType.CLOVERS, CardIndex.THREE, R.drawable.playingcards_detailed_clovers_3),
        PlayingCard(CardType.CLOVERS, CardIndex.FOUR, R.drawable.playingcards_detailed_clovers_4),
        PlayingCard(CardType.CLOVERS, CardIndex.FIVE, R.drawable.playingcards_detailed_clovers_5),
        PlayingCard(CardType.CLOVERS, CardIndex.SIX, R.drawable.playingcards_detailed_clovers_6),
        PlayingCard(CardType.CLOVERS, CardIndex.SEVEN, R.drawable.playingcards_detailed_clovers_7),
        PlayingCard(CardType.CLOVERS, CardIndex.EIGHT, R.drawable.playingcards_detailed_clovers_8),
        PlayingCard(CardType.CLOVERS, CardIndex.NINE, R.drawable.playingcards_detailed_clovers_9),
        PlayingCard(CardType.CLOVERS, CardIndex.TEN, R.drawable.playingcards_detailed_clovers_10),
        PlayingCard(CardType.CLOVERS, CardIndex.J, R.drawable.playingcards_detailed_clovers_j),
        PlayingCard(CardType.CLOVERS, CardIndex.Q, R.drawable.playingcards_detailed_clovers_q),
        PlayingCard(CardType.CLOVERS, CardIndex.K, R.drawable.playingcards_detailed_clovers_k),
        PlayingCard(CardType.DIAMONDS, CardIndex.A, R.drawable.playingcards_detailed_diamonds_a),
        PlayingCard(CardType.DIAMONDS, CardIndex.TWO, R.drawable.playingcards_detailed_diamonds_2),
        PlayingCard(
            CardType.DIAMONDS, CardIndex.THREE, R.drawable.playingcards_detailed_diamonds_3
        ),
        PlayingCard(CardType.DIAMONDS, CardIndex.FOUR, R.drawable.playingcards_detailed_diamonds_4),
        PlayingCard(CardType.DIAMONDS, CardIndex.FIVE, R.drawable.playingcards_detailed_diamonds_5),
        PlayingCard(CardType.DIAMONDS, CardIndex.SIX, R.drawable.playingcards_detailed_diamonds_6),
        PlayingCard(
            CardType.DIAMONDS, CardIndex.SEVEN, R.drawable.playingcards_detailed_diamonds_7
        ),
        PlayingCard(
            CardType.DIAMONDS, CardIndex.EIGHT, R.drawable.playingcards_detailed_diamonds_8
        ),
        PlayingCard(CardType.DIAMONDS, CardIndex.NINE, R.drawable.playingcards_detailed_diamonds_9),
        PlayingCard(CardType.DIAMONDS, CardIndex.TEN, R.drawable.playingcards_detailed_diamonds_10),
        PlayingCard(CardType.DIAMONDS, CardIndex.J, R.drawable.playingcards_detailed_diamonds_j),
        PlayingCard(CardType.DIAMONDS, CardIndex.Q, R.drawable.playingcards_detailed_diamonds_q),
        PlayingCard(CardType.DIAMONDS, CardIndex.K, R.drawable.playingcards_detailed_diamonds_k),
        PlayingCard(CardType.HEARTS, CardIndex.A, R.drawable.playingcards_detailed_hearts_a),
        PlayingCard(CardType.HEARTS, CardIndex.TWO, R.drawable.playingcards_detailed_hearts_2),
        PlayingCard(CardType.HEARTS, CardIndex.THREE, R.drawable.playingcards_detailed_hearts_3),
        PlayingCard(CardType.HEARTS, CardIndex.FOUR, R.drawable.playingcards_detailed_hearts_4),
        PlayingCard(CardType.HEARTS, CardIndex.FIVE, R.drawable.playingcards_detailed_hearts_5),
        PlayingCard(CardType.HEARTS, CardIndex.SIX, R.drawable.playingcards_detailed_hearts_6),
        PlayingCard(CardType.HEARTS, CardIndex.SEVEN, R.drawable.playingcards_detailed_hearts_7),
        PlayingCard(CardType.HEARTS, CardIndex.EIGHT, R.drawable.playingcards_detailed_hearts_8),
        PlayingCard(CardType.HEARTS, CardIndex.NINE, R.drawable.playingcards_detailed_hearts_9),
        PlayingCard(CardType.HEARTS, CardIndex.TEN, R.drawable.playingcards_detailed_hearts_10),
        PlayingCard(CardType.HEARTS, CardIndex.J, R.drawable.playingcards_detailed_hearts_j),
        PlayingCard(CardType.HEARTS, CardIndex.Q, R.drawable.playingcards_detailed_hearts_q),
        PlayingCard(CardType.HEARTS, CardIndex.K, R.drawable.playingcards_detailed_hearts_k),
        PlayingCard(CardType.SPADES, CardIndex.A, R.drawable.playingcards_detailed_spades_a),
        PlayingCard(CardType.SPADES, CardIndex.TWO, R.drawable.playingcards_detailed_spades_2),
        PlayingCard(CardType.SPADES, CardIndex.THREE, R.drawable.playingcards_detailed_spades_3),
        PlayingCard(CardType.SPADES, CardIndex.FOUR, R.drawable.playingcards_detailed_spades_4),
        PlayingCard(CardType.SPADES, CardIndex.FIVE, R.drawable.playingcards_detailed_spades_5),
        PlayingCard(CardType.SPADES, CardIndex.SIX, R.drawable.playingcards_detailed_spades_6),
        PlayingCard(CardType.SPADES, CardIndex.SEVEN, R.drawable.playingcards_detailed_spades_7),
        PlayingCard(CardType.SPADES, CardIndex.EIGHT, R.drawable.playingcards_detailed_spades_8),
        PlayingCard(CardType.SPADES, CardIndex.NINE, R.drawable.playingcards_detailed_spades_9),
        PlayingCard(CardType.SPADES, CardIndex.TEN, R.drawable.playingcards_detailed_spades_10),
        PlayingCard(CardType.SPADES, CardIndex.J, R.drawable.playingcards_detailed_spades_j),
        PlayingCard(CardType.SPADES, CardIndex.Q, R.drawable.playingcards_detailed_spades_q),
        PlayingCard(CardType.SPADES, CardIndex.K, R.drawable.playingcards_detailed_spades_k),
    )

    override var cardHeight = 0f
    override var cardHeightDp = 0.dp
    override var cardWidth = 0f
    override var cardWidthDp = 0.dp
    override var distanceBetweenCards = 0.175f
    override var finished = false

    private val movingCards: MutableList<PlayingCard> = mutableListOf()
    private val cardStacks: Array<MutableList<PlayingCard>> = Array(14) { mutableListOf() }
    private var moves = 0

    override fun onOpenDialog() {
        super.onOpenDialog()
        timer.pause()
    }

    override fun startNewGame() {
    }

    private fun setCardBaseOffset(card: PlayingCard) {
        val x = (card.stackId % 7) * cardWidth
        val y =
            if (card.stackId < 7) 0f else (1 + (0.5f + card.stackIndex) * distanceBetweenCards) * cardHeight
        card.baseOffset = IntOffset(x.roundToInt(), y.roundToInt())
    }

    override fun onTap(offset: Offset, onReleaseMovingCards: () -> Unit) {
        val indices = getStackIndicesByPosition(offset)
        if (indices.first == -1) return
        if (indices.second == -1) {
            if (indices.first == 6) resetRestStack()
            return
        }
        if (indices.first == 6) return turnFromRestStack()
        val card = cardStacks[indices.first][indices.second]
        startMoveCard(card, canMove(card))
        onReleaseMovingCards()
    }

    private fun canMove(card: PlayingCard): Boolean {
        return canMoveCardToFoundation(card) || canMoveCardToStack(card)
    }

    override fun onDragStart(offset: Offset) {
        val indices = getStackIndicesByPosition(offset)
        if (indices.first == -1 || indices.second == -1) return
        val card = cardStacks[indices.first][indices.second]
        startMoveCard(card)
    }

    private fun getStackIndicesByPosition(offset: Offset): Pair<Int, Int> {
        val column = floor(offset.x / cardWidth).roundToInt()
        val row =
            if (offset.y < cardHeight) 0 else if (offset.y < cardHeight + 0.5 * cardHeight * distanceBetweenCards) -1 else ceil(
                (offset.y - cardHeight - 0.5f * distanceBetweenCards * cardHeight) / (distanceBetweenCards * cardHeight)
            ).roundToInt()
        if (row == -1) return Pair(-1, -1)
        val rowLastCard =
            if (row == 0) 0 else if (offset.y < cardHeight * 2 + 0.5 * cardHeight * distanceBetweenCards) 1 else 1 + ceil(
                (offset.y - cardHeight * 2 - 0.5f * distanceBetweenCards * cardHeight) / (distanceBetweenCards * cardHeight)
            ).roundToInt()
        val i1 = if (row == 0) column else 7 + column
        val stack = cardStacks[i1]
        if (stack.isEmpty()) return Pair(i1, -1)
        if (stack.size < rowLastCard) return Pair(i1, -1)
        val i2 =
            if (row == 0) stack.size - 1 else if (row <= stack.size) row - 1 else stack.size - 1
        return Pair(i1, i2)
    }

    override fun resetRestStack() {
        if (!restStackEnabled.value || cardStacks[6].isNotEmpty()) return
        cardStacks[6] = cardStacks[5].asReversed().map { card -> card }.toMutableList()
        cardStacks[5].clear()
        cardStacks[6].forEachIndexed { i, card ->
            viewModelScope.launch {
                card.stackIndex = i
                card.stackId = 6
                card.frontVisible.value = false
                card.isLast.value = false
                card.visible.value = i == 0
                card.isMoving = false
                setCardBaseOffset(card)
                card.targetOffset.value = card.baseOffset
            }
        }
    }

    override fun turnFromRestStack() {
        if (cardStacks[6].isEmpty()) return
        val card = cardStacks[6].last()
        val index = cardStacks[5].size
        card.stackIndex = index
        card.stackId = 5
        card.visible.value = true
        card.frontVisible.value = true
        card.isLast.value = true
        card.isMoving = false
        viewModelScope.launch {
            setCardBaseOffset(card)
            card.targetOffset.value = card.baseOffset
        }
        cardStacks[5].add(card)
        cardStacks[6].removeAt(cardStacks[6].lastIndex)
        if (cardStacks[6].isEmpty() && cardStacks[5].size <= 1) restStackEnabled.value = false
    }

    private fun startMoveCard(card: PlayingCard, withZIndexChange: Boolean = true) {
        if (movingCards.isNotEmpty()) return
        if (!card.frontVisible.value) return
        val stackId = card.stackId
        if (stackId <= 5) {
            val c = cardStacks[stackId].last()
            c.isMoving = true
            if (withZIndexChange) c.recalculateZIndex()
            movingCards.add(c)
            return
        }
        val stack = cardStacks[stackId]
        val index = card.stackIndex
        for (i in index until stack.size) {
            val c = stack[i]
            c.isMoving = true
            if (withZIndexChange) c.recalculateZIndex()
            movingCards.add(c)
        }
        if (index != 0 && stackId >= 7) {
            stack[index - 1].isLast.value = true
        }
    }

    override fun moveCards(intOffset: IntOffset, coroutineScope: CoroutineScope) {
        movingCards.forEach { card ->
            card.offset += intOffset
            card.targetOffset.value = card.baseOffset + card.offset
            coroutineScope.launch {
                card.animOffset.animateTo(card.baseOffset + card.offset)
            }
        }
    }

    override fun onReleaseMovingCards(navController: NavController) {
        if (movingCards.isEmpty()) return
        val firstCard = movingCards[0]
        val firstCardStackId = firstCard.stackId
        val firstCardStackIndex = firstCard.stackIndex
        val oldStack = cardStacks[firstCardStackId]
        val offset = firstCard.baseOffset + firstCard.offset

        val heightBorder = ((1 + 1.5f * distanceBetweenCards) * cardHeight) / 2f
        val selectedStackByOffset = MathUtils.clamp(((offset.x - 0) / cardWidth).roundToInt(), 0, 6)

        var foundNewStack = false

        if (movingCards.size == 1 && (offset.y < heightBorder || firstCardStackId % 7 == selectedStackByOffset)) {
            foundNewStack = moveCardToFoundation(firstCard)
        }
        if (!foundNewStack && firstCardStackId % 7 != selectedStackByOffset) {
            foundNewStack = moveCardsToStack(firstCard, 7 + selectedStackByOffset)
        }
        if (!foundNewStack && movingCards.size == 1) {
            foundNewStack = moveCardToFoundation(firstCard)
        }
        var stackId = 7
        while (!foundNewStack && stackId < 14) {
            foundNewStack = moveCardsToStack(firstCard, stackId)
            stackId++
        }
        if (firstCardStackId >= 7 && !foundNewStack && firstCardStackIndex != 0) {
            oldStack[firstCardStackIndex - 1].isLast.value = false
        }

        movingCards.forEach { card ->
            card.offset = IntOffset.Companion.Zero
            card.isMoving = false
            card.targetOffset.value = card.baseOffset
        }
        movingCards.clear()

        if (firstCardStackId >= 7 && oldStack.isNotEmpty()) oldStack.last().frontVisible.value =
            true

        moves++

        if (!foundNewStack) timer.addMillis(15000)

        if (cardStacks[6].isEmpty() && cardStacks[5].size <= 1) restStackEnabled.value = false
    }

    private fun canMoveCardToFoundation(card: PlayingCard): Boolean {
        val foundationStackId = floor(card.id / 13f).toInt()
        val foundation = cardStacks[foundationStackId]
        return (foundation.isEmpty() && card.id % 13 == 0) || (foundation.isNotEmpty() && foundation.last().id % 13 == card.id % 13 - 1)
    }

    private fun moveCardToFoundation(card: PlayingCard): Boolean {
        val foundationStackId = floor(card.id / 13f).toInt()
        val foundation = cardStacks[foundationStackId]
        if (card.id % 13 == 0 || (foundation.isNotEmpty() && foundation.last().id % 13 == card.id % 13 - 1)) {
            cardStacks[card.stackId].removeAt(cardStacks[card.stackId].lastIndex)
            foundation.add(card)
            card.stackId = foundationStackId
            val index = foundation.size - 1
            card.stackIndex = index
            card.isLast.value = true
            setCardBaseOffset(card)
            return true
        }
        return false
    }

    private fun canMoveCardToStack(card: PlayingCard): Boolean {
        for (i in 7..13) {
            val newStack = cardStacks[i]
            val newStackSize = newStack.size
            if (newStackSize == 0) {
                if (card.id % 13 != 12) continue
            } else {
                if (card.id % 13 == 12) continue
                val lastCardNewStack = newStack.last()
                if (lastCardNewStack.id % 13 != card.id % 13 + 1) continue
                if (!((card.id in 13..38 && lastCardNewStack.id !in 13..38) || (card.id !in 13..38 && lastCardNewStack.id in 13..38))) continue
                return true
            }
        }
        return false
    }

    private fun moveCardsToStack(card: PlayingCard, newStackId: Int): Boolean {
        val newStack = cardStacks[newStackId]
        val newStackSize = newStack.size
        if (newStackSize == 0) {
            if (card.id % 13 != 12) return false
        } else {
            if (card.id % 13 == 12) return false
            val lastCardNewStack = newStack.last()
            if (lastCardNewStack.id % 13 != card.id % 13 + 1) return false
            if (!((card.id in 13..38 && lastCardNewStack.id !in 13..38) || (card.id !in 13..38 && lastCardNewStack.id in 13..38))) return false
            lastCardNewStack.isLast.value = false
        }
        newStack.addAll(movingCards)
        val lastIndex = card.stackIndex
        val lastStack = cardStacks[card.stackId]
        while (lastStack.size > lastIndex) {
            lastStack.removeAt(lastIndex)
        }
        movingCards.forEachIndexed { i, c ->
            c.stackIndex = newStackSize + i
            c.stackId = newStackId
            setCardBaseOffset(c)
        }
        return true
    }

    override fun onUpdateTableSize(width: Dp, height: Dp, density: Density): DpSize {
        val maxWidthDp = width * 0.95f
        val maxHeightDp = height * 0.95f
        val maxCardHeightDp = maxHeightDp / 4.2375f
        val cardHeightDp = min(maxCardHeightDp.value, maxWidthDp.value / 7f / CARD_ASPECT_RATIO).dp

        this.cardHeightDp = cardHeightDp
        cardWidthDp = cardHeightDp * CARD_ASPECT_RATIO
        cardHeight = with(density) { cardHeightDp.toPx() }
        cardWidth = cardHeight * CARD_ASPECT_RATIO
        distanceBetweenCards = min(maxCardHeightDp / cardHeightDp, 2f) * 0.175f

        return DpSize(cardHeightDp * CARD_ASPECT_RATIO, cardHeightDp)
    }

    override fun onClickFinishGame(navController: NavController) {
        cards.forEach {
            it.stackId = it.type.ordinal
            it.stackIndex = it.index.ordinal
            it.isMoving = false
            it.isLast.value = true
            it.frontVisible.value = true
            setCardBaseOffset(it)
            it.targetOffset.value = it.baseOffset
        }
    }

    override fun forceSave() {
    }

    override fun checkAdLoaded(activity: Activity, adLoaded: MutableState<Boolean>) {
    }

    override fun showAd(
        activity: Activity, adLoaded: MutableState<Boolean>, onAdWatched: (Int) -> Unit
    ) {
    }

    override fun setCardVisualType(value: CardVisualType) {
        cardVisualType.value = value
    }
}