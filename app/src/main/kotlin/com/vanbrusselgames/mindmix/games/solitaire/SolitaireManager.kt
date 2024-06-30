package com.vanbrusselgames.mindmix.games.solitaire

import androidx.compose.runtime.mutableStateOf
import androidx.compose.ui.geometry.Offset
import androidx.compose.ui.unit.IntOffset
import androidx.core.math.MathUtils.clamp
import com.google.firebase.analytics.FirebaseAnalytics
import com.vanbrusselgames.mindmix.BaseLayout
import com.vanbrusselgames.mindmix.Logger
import com.vanbrusselgames.mindmix.R
import com.vanbrusselgames.mindmix.games.GameFinished
import com.vanbrusselgames.mindmix.games.GameTimer
import com.vanbrusselgames.mindmix.games.solitaire.SolitaireLayout.Companion.cardHeight
import com.vanbrusselgames.mindmix.games.solitaire.SolitaireLayout.Companion.cardWidth
import com.vanbrusselgames.mindmix.games.solitaire.SolitaireLayout.Companion.distanceBetweenCards
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.launch
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json
import kotlin.math.floor
import kotlin.math.max
import kotlin.math.roundToInt

class SolitaireManager {
    companion object Instance {
        val timer = GameTimer()

        val cards = arrayOf(
            PlayingCard(
                PlayingCard.CardType.CLOVERS,
                PlayingCard.CardIndex.A,
                R.drawable.playingcards_detailed_clovers_a
            ),
            PlayingCard(
                PlayingCard.CardType.CLOVERS,
                PlayingCard.CardIndex.TWO,
                R.drawable.playingcards_detailed_clovers_2
            ),
            PlayingCard(
                PlayingCard.CardType.CLOVERS,
                PlayingCard.CardIndex.THREE,
                R.drawable.playingcards_detailed_clovers_3
            ),
            PlayingCard(
                PlayingCard.CardType.CLOVERS,
                PlayingCard.CardIndex.FOUR,
                R.drawable.playingcards_detailed_clovers_4
            ),
            PlayingCard(
                PlayingCard.CardType.CLOVERS,
                PlayingCard.CardIndex.FIVE,
                R.drawable.playingcards_detailed_clovers_5
            ),
            PlayingCard(
                PlayingCard.CardType.CLOVERS,
                PlayingCard.CardIndex.SIX,
                R.drawable.playingcards_detailed_clovers_6
            ),
            PlayingCard(
                PlayingCard.CardType.CLOVERS,
                PlayingCard.CardIndex.SEVEN,
                R.drawable.playingcards_detailed_clovers_7
            ),
            PlayingCard(
                PlayingCard.CardType.CLOVERS,
                PlayingCard.CardIndex.EIGHT,
                R.drawable.playingcards_detailed_clovers_8
            ),
            PlayingCard(
                PlayingCard.CardType.CLOVERS,
                PlayingCard.CardIndex.NINE,
                R.drawable.playingcards_detailed_clovers_9
            ),
            PlayingCard(
                PlayingCard.CardType.CLOVERS,
                PlayingCard.CardIndex.TEN,
                R.drawable.playingcards_detailed_clovers_10
            ),
            PlayingCard(
                PlayingCard.CardType.CLOVERS,
                PlayingCard.CardIndex.J,
                R.drawable.playingcards_detailed_clovers_j
            ),
            PlayingCard(
                PlayingCard.CardType.CLOVERS,
                PlayingCard.CardIndex.Q,
                R.drawable.playingcards_detailed_clovers_q
            ),
            PlayingCard(
                PlayingCard.CardType.CLOVERS,
                PlayingCard.CardIndex.K,
                R.drawable.playingcards_detailed_clovers_k
            ),
            PlayingCard(
                PlayingCard.CardType.DIAMONDS,
                PlayingCard.CardIndex.A,
                R.drawable.playingcards_detailed_diamonds_a
            ),
            PlayingCard(
                PlayingCard.CardType.DIAMONDS,
                PlayingCard.CardIndex.TWO,
                R.drawable.playingcards_detailed_diamonds_2
            ),
            PlayingCard(
                PlayingCard.CardType.DIAMONDS,
                PlayingCard.CardIndex.THREE,
                R.drawable.playingcards_detailed_diamonds_3
            ),
            PlayingCard(
                PlayingCard.CardType.DIAMONDS,
                PlayingCard.CardIndex.FOUR,
                R.drawable.playingcards_detailed_diamonds_4
            ),
            PlayingCard(
                PlayingCard.CardType.DIAMONDS,
                PlayingCard.CardIndex.FIVE,
                R.drawable.playingcards_detailed_diamonds_5
            ),
            PlayingCard(
                PlayingCard.CardType.DIAMONDS,
                PlayingCard.CardIndex.SIX,
                R.drawable.playingcards_detailed_diamonds_6
            ),
            PlayingCard(
                PlayingCard.CardType.DIAMONDS,
                PlayingCard.CardIndex.SEVEN,
                R.drawable.playingcards_detailed_diamonds_7
            ),
            PlayingCard(
                PlayingCard.CardType.DIAMONDS,
                PlayingCard.CardIndex.EIGHT,
                R.drawable.playingcards_detailed_diamonds_8
            ),
            PlayingCard(
                PlayingCard.CardType.DIAMONDS,
                PlayingCard.CardIndex.NINE,
                R.drawable.playingcards_detailed_diamonds_9
            ),
            PlayingCard(
                PlayingCard.CardType.DIAMONDS,
                PlayingCard.CardIndex.TEN,
                R.drawable.playingcards_detailed_diamonds_10
            ),
            PlayingCard(
                PlayingCard.CardType.DIAMONDS,
                PlayingCard.CardIndex.J,
                R.drawable.playingcards_detailed_diamonds_j
            ),
            PlayingCard(
                PlayingCard.CardType.DIAMONDS,
                PlayingCard.CardIndex.Q,
                R.drawable.playingcards_detailed_diamonds_q
            ),
            PlayingCard(
                PlayingCard.CardType.DIAMONDS,
                PlayingCard.CardIndex.K,
                R.drawable.playingcards_detailed_diamonds_k
            ),
            PlayingCard(
                PlayingCard.CardType.HEARTS,
                PlayingCard.CardIndex.A,
                R.drawable.playingcards_detailed_hearts_a
            ),
            PlayingCard(
                PlayingCard.CardType.HEARTS,
                PlayingCard.CardIndex.TWO,
                R.drawable.playingcards_detailed_hearts_2
            ),
            PlayingCard(
                PlayingCard.CardType.HEARTS,
                PlayingCard.CardIndex.THREE,
                R.drawable.playingcards_detailed_hearts_3
            ),
            PlayingCard(
                PlayingCard.CardType.HEARTS,
                PlayingCard.CardIndex.FOUR,
                R.drawable.playingcards_detailed_hearts_4
            ),
            PlayingCard(
                PlayingCard.CardType.HEARTS,
                PlayingCard.CardIndex.FIVE,
                R.drawable.playingcards_detailed_hearts_5
            ),
            PlayingCard(
                PlayingCard.CardType.HEARTS,
                PlayingCard.CardIndex.SIX,
                R.drawable.playingcards_detailed_hearts_6
            ),
            PlayingCard(
                PlayingCard.CardType.HEARTS,
                PlayingCard.CardIndex.SEVEN,
                R.drawable.playingcards_detailed_hearts_7
            ),
            PlayingCard(
                PlayingCard.CardType.HEARTS,
                PlayingCard.CardIndex.EIGHT,
                R.drawable.playingcards_detailed_hearts_8
            ),
            PlayingCard(
                PlayingCard.CardType.HEARTS,
                PlayingCard.CardIndex.NINE,
                R.drawable.playingcards_detailed_hearts_9
            ),
            PlayingCard(
                PlayingCard.CardType.HEARTS,
                PlayingCard.CardIndex.TEN,
                R.drawable.playingcards_detailed_hearts_10
            ),
            PlayingCard(
                PlayingCard.CardType.HEARTS,
                PlayingCard.CardIndex.J,
                R.drawable.playingcards_detailed_hearts_j
            ),
            PlayingCard(
                PlayingCard.CardType.HEARTS,
                PlayingCard.CardIndex.Q,
                R.drawable.playingcards_detailed_hearts_q
            ),
            PlayingCard(
                PlayingCard.CardType.HEARTS,
                PlayingCard.CardIndex.K,
                R.drawable.playingcards_detailed_hearts_k
            ),
            PlayingCard(
                PlayingCard.CardType.SPADES,
                PlayingCard.CardIndex.A,
                R.drawable.playingcards_detailed_spades_a
            ),
            PlayingCard(
                PlayingCard.CardType.SPADES,
                PlayingCard.CardIndex.TWO,
                R.drawable.playingcards_detailed_spades_2
            ),
            PlayingCard(
                PlayingCard.CardType.SPADES,
                PlayingCard.CardIndex.THREE,
                R.drawable.playingcards_detailed_spades_3
            ),
            PlayingCard(
                PlayingCard.CardType.SPADES,
                PlayingCard.CardIndex.FOUR,
                R.drawable.playingcards_detailed_spades_4
            ),
            PlayingCard(
                PlayingCard.CardType.SPADES,
                PlayingCard.CardIndex.FIVE,
                R.drawable.playingcards_detailed_spades_5
            ),
            PlayingCard(
                PlayingCard.CardType.SPADES,
                PlayingCard.CardIndex.SIX,
                R.drawable.playingcards_detailed_spades_6
            ),
            PlayingCard(
                PlayingCard.CardType.SPADES,
                PlayingCard.CardIndex.SEVEN,
                R.drawable.playingcards_detailed_spades_7
            ),
            PlayingCard(
                PlayingCard.CardType.SPADES,
                PlayingCard.CardIndex.EIGHT,
                R.drawable.playingcards_detailed_spades_8
            ),
            PlayingCard(
                PlayingCard.CardType.SPADES,
                PlayingCard.CardIndex.NINE,
                R.drawable.playingcards_detailed_spades_9
            ),
            PlayingCard(
                PlayingCard.CardType.SPADES,
                PlayingCard.CardIndex.TEN,
                R.drawable.playingcards_detailed_spades_10
            ),
            PlayingCard(
                PlayingCard.CardType.SPADES,
                PlayingCard.CardIndex.J,
                R.drawable.playingcards_detailed_spades_j
            ),
            PlayingCard(
                PlayingCard.CardType.SPADES,
                PlayingCard.CardIndex.Q,
                R.drawable.playingcards_detailed_spades_q
            ),
            PlayingCard(
                PlayingCard.CardType.SPADES,
                PlayingCard.CardIndex.K,
                R.drawable.playingcards_detailed_spades_k
            ),
        )

        private val movingCards: MutableList<PlayingCard> = mutableListOf()
        private val cardStacks: Array<MutableList<PlayingCard>> = Array(14) { mutableListOf() }

        var finished = false

        val couldGetFinished = mutableStateOf(false)
        val restStackEnabled = mutableStateOf(true)

        val cardVisualType = mutableStateOf(PlayingCard.CardVisualType.SIMPLE)

        fun loadFromFile(data: SolitaireData) {
            if (data.finished) {
                startNewGame()
                return
            }

            if (!checkValid(data.cardStacks)) {
                Logger.e("Saved Solitaire puzzle is not valid, loading new puzzle")
                startNewGame()
                return
            }

            for (i in data.cardStacks.indices) {
                data.cardStacks[i].forEachIndexed { j, k ->
                    var index = k
                    var frontVisible = true
                    if (index < 0) {
                        index = -1 * index - 1
                        frontVisible = false
                    }
                    val c = cards[index]
                    c.stackId = i
                    c.stackIndex = j
                    c.visible.value = !(i == 6 && j != 0)
                    c.frontVisible.value = frontVisible
                    if (i < 7 || (frontVisible && j + 1 == data.cardStacks[i].size)) c.isLast.value =
                        true
                    c.recalculateZIndex()
                    c.calculateBaseOffset()
                    cardStacks[i].add(c)
                }
            }
            couldGetFinished.value = couldGetFinished()
            timer.set(data.millis)
            timer.addMillis(data.penaltyMillis)

            if (cardStacks[6].size == 0 && cardStacks[5].size <= 1) restStackEnabled.value = false
        }

        fun saveToFile(): String {
            val stacks = Array(14) { listOf<Int>() }
            for (i in cardStacks.indices) {
                stacks[i] =
                    cardStacks[i].map { c -> if (c.frontVisible.value) c.id else -1 * c.id - 1 }
            }
            return Json.encodeToString(
                SolitaireData(stacks.asList(), finished, timer.currentMillis, timer.penaltyMillis)
            )
        }

        private fun checkValid(cardStacks: List<List<Int>>): Boolean {
            for (i in cardStacks.indices) {
                if (i < 7) continue
                val visibleStack = mutableListOf<Int>()
                var visibleCount = 0
                cardStacks[i].forEach { k ->
                    if (k < 0 && visibleCount != 0) return false
                    if (k >= 0) {
                        visibleStack.add(k + visibleCount)
                        visibleCount += 1
                    }
                }
                if (visibleCount <= 1) continue
                var color = (visibleStack[0] / 13)
                if (color >= 2) color = 3 - color
                visibleStack.forEachIndexed { j, v ->
                    if (j != 0) {
                        var c = v / 13
                        if (c >= 2) c = 3 - c
                        if (c != color) color = c
                        else return false
                    }
                }
            }
            return true
        }

        fun loadPuzzle() {
            if (finished) reset()
            else timer.resume()
            if (!cardStacks.all { cs -> cs.isEmpty() }) return
            val dupCards = cards.copyOf()
            dupCards.shuffle()
            var j = 0
            for (i in 0 until 7) {
                cardStacks[7 + i] = dupCards.copyOfRange(j, j + i + 1).toMutableList()
                val lastCard = cardStacks[7 + i].last()
                lastCard.frontVisible.value = true
                lastCard.isLast.value = true
                cardStacks[7 + i].forEachIndexed { index, card ->
                    card.stackIndex = index
                    card.stackId = 7 + i
                    card.visible.value = true
                    card.recalculateZIndex()
                    card.calculateBaseOffset()
                }
                j += i + 1
            }
            cardStacks[6] = dupCards.copyOfRange(j, dupCards.size).toMutableList()
            cardStacks[6].forEachIndexed { index, card ->
                card.stackIndex = index
                card.stackId = 6
                card.visible.value = index == 0
                card.recalculateZIndex()
                card.calculateBaseOffset()
            }
            val stacks = Array(14) { listOf<Int>() }
            for (i in cardStacks.indices) {
                stacks[i] =
                    cardStacks[i].map { c -> if (c.frontVisible.value) c.id else -1 * c.id - 1 }
            }
            couldGetFinished.value = couldGetFinished()
            restStackEnabled.value = true
            timer.start()
            Logger.logEvent(FirebaseAnalytics.Event.LEVEL_START) {
                param(FirebaseAnalytics.Param.LEVEL_NAME, GAME_NAME)
            }
        }

        private fun reset() {
            finished = false
            BaseLayout.activeOverlapUI.value = false
            cardStacks.forEach { s -> s.clear() }
            cards.forEach {
                it.frontVisible.value = false
                it.isLast.value = false
                it.stackId = 6
                it.stackIndex = 0
                it.isMoving = false
                it.recalculateZIndex()
                it.calculateBaseOffset()
            }
        }

        fun startNewGame() {
            reset()
            timer.start()
            loadPuzzle()
        }

        fun resetRestStack(coroutineScope: CoroutineScope) {
            if (restStackEnabled.value && cardStacks[6].size != 0) return
            cardStacks[6] = cardStacks[5].asReversed().map { card -> card }.toMutableList()
            cardStacks[5].clear()
            cardStacks[6].forEachIndexed { i, card ->
                card.stackIndex = i
                card.stackId = 6
                card.frontVisible.value = false
                card.isLast.value = false
                card.visible.value = false
                card.isMoving = false
                card.recalculateZIndex()
                card.calculateBaseOffset()
                coroutineScope.launch {
                    card.animOffset.animateTo(card.baseOffset)
                }
            }
            cardStacks[6][0].visible.value = true
        }

        fun turnFromRestStack(coroutineScope: CoroutineScope) {
            if (cardStacks[6].size == 0) return
            val card = cardStacks[6].last()
            card.isMoving = true
            card.visible.value = true
            val index = cardStacks[5].size
            card.stackIndex = index
            card.stackId = 5
            card.frontVisible.value = true
            card.isLast.value = true
            card.recalculateZIndex()
            card.calculateBaseOffset()
            cardStacks[5].add(card)
            cardStacks[6].removeLast()
            coroutineScope.launch {
                card.animOffset.animateTo(card.baseOffset)
                card.isMoving = false
                card.recalculateZIndex()
            }
            if (cardStacks[6].size == 0 && cardStacks[5].size <= 1) restStackEnabled.value = false
        }

        fun startMoveCard(card: PlayingCard) {
            if (movingCards.size != 0) return
            if (!card.frontVisible.value) return
            val stackId = card.stackId
            if (stackId <= 5) {
                val c = cardStacks[stackId].last()
                c.isMoving = true
                c.recalculateZIndex()
                movingCards.add(c)
                return
            }
            val stack = cardStacks[stackId]
            val index = card.stackIndex
            for (i in index until stack.size) {
                val c = stack[i]
                c.isMoving = true
                c.recalculateZIndex()
                movingCards.add(c)
            }
            if (index != 0 && stackId >= 7) {
                stack[index - 1].isLast.value = true
            }
        }

        fun moveCards(offset: Offset, coroutineScope: CoroutineScope) {
            movingCards.forEach { card ->
                card.offset = IntOffset(
                    card.offset.x + offset.x.roundToInt(), card.offset.y + offset.y.roundToInt()
                )
                val baseOffset = card.baseOffset
                coroutineScope.launch {
                    card.animOffset.animateTo(baseOffset + card.offset)
                }
            }
        }

        fun onReleaseMovingCards(coroutineScope: CoroutineScope) {
            if (movingCards.size == 0) return
            val firstCard = movingCards[0]
            val firstCardStackId = firstCard.stackId
            val firstCardStackIndex = firstCard.stackIndex
            val oldStack = cardStacks[firstCardStackId]
            val offset = firstCard.baseOffset + firstCard.offset

            val heightBorder = ((1 + 1.5f * distanceBetweenCards) * cardHeight) / 2f
            val selectedStackByOffset = clamp(((offset.x - 0) / cardWidth).roundToInt(), 0, 6)

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
                card.offset = IntOffset.Zero
                val baseOffset = card.baseOffset
                coroutineScope.launch {
                    card.animOffset.animateTo(baseOffset)
                    card.isMoving = false
                    card.recalculateZIndex()
                }
            }
            movingCards.clear()

            if (firstCardStackId >= 7 && oldStack.size >= 1) oldStack.last().frontVisible.value =
                true

            if (foundNewStack) checkFinished()
            else timer.addMillis(15000)

            if (cardStacks[6].size == 0 && cardStacks[5].size <= 1) restStackEnabled.value = false
        }

        private fun moveCardToFoundation(card: PlayingCard): Boolean {
            val foundationStackId = floor(card.id / 13f).toInt()
            val foundation = cardStacks[foundationStackId]
            if (card.id % 13 == 0 || (foundation.size != 0 && foundation.last().id % 13 == card.id % 13 - 1)) {
                cardStacks[card.stackId].removeLast()
                foundation.add(card)
                card.stackId = foundationStackId
                val index = foundation.size - 1
                card.stackIndex = index
                card.isLast.value = true
                card.calculateBaseOffset()
                return true
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
                c.calculateBaseOffset()
            }
            return true
        }

        fun checkFinished() {
            finished = isFinished()
            if (!finished) {
                couldGetFinished.value = couldGetFinished()
            } else {
                timer.stop()
                Logger.logEvent(FirebaseAnalytics.Event.LEVEL_END) {
                    param(FirebaseAnalytics.Param.LEVEL_NAME, GAME_NAME)
                    param(FirebaseAnalytics.Param.SUCCESS, 1)
                }
                onGameFinished()
            }
        }

        private fun isFinished(): Boolean {
            for (card in cards) {
                if (!card.frontVisible.value || card.stackId >= 4) return false
            }
            return true
        }

        private fun couldGetFinished(): Boolean {
            return !cards.any { !it.frontVisible.value && it.stackId >= 7 }
        }

        private fun onGameFinished() {
            val titleId = R.string.solitaire_name//"Congrats / Smart / Well done"
            val descId = R.string.solitaire_success
            val minutes = max(1f, timer.currentMillis / 1000f / 60f)
            val reward = max(1, floor(MAX_REWARD / minutes).toInt())
            GameFinished.onGameFinished(titleId, descId, reward)
        }
    }
}