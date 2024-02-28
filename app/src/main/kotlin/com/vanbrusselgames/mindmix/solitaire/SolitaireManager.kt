package com.vanbrusselgames.mindmix.solitaire

import androidx.compose.runtime.mutableStateOf
import androidx.compose.ui.geometry.Offset
import androidx.compose.ui.unit.IntOffset
import androidx.core.math.MathUtils.clamp
import com.google.firebase.analytics.FirebaseAnalytics
import com.vanbrusselgames.mindmix.Logger
import com.vanbrusselgames.mindmix.R
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json
import kotlin.math.floor
import kotlin.math.roundToInt

class SolitaireManager {
    companion object Instance {
        private const val gameName = "Solitaire"

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
        val solitaireFinished = mutableStateOf(finished)

        var couldGetFinished = mutableStateOf(false)

        fun loadFromFile(data: SolitaireData) {
            if (!cardStacks.all { cs -> cs.isEmpty() }) return

            if (!checkValid(data.cardStacks)) {
                Logger.e("Saved Solitaire puzzle is not valid, loading new puzzle")
                reset()
                loadPuzzle()
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
                    c.currentStackId = i
                    c.currentStackIndex = j
                    c.frontVisible = frontVisible
                    if (i < 7 || (frontVisible && j + 1 == data.cardStacks[i].size)) c.isLast = true
                    cardStacks[i].add(c)
                }
            }
            finished = data.finished
            solitaireFinished.value = finished
            if (!finished) couldGetFinished.value = couldGetFinished()
        }

        fun saveToFile(): String {
            val stacks = Array(14) { listOf<Int>() }
            for (i in cardStacks.indices) {
                stacks[i] = cardStacks[i].map { c -> if (c.frontVisible) c.id else -1 * c.id - 1 }
            }
            return Json.encodeToString(SolitaireData(stacks.asList(), finished))
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
            if (!cardStacks.all { cs -> cs.isEmpty() }) return
            Logger.logEvent(FirebaseAnalytics.Event.LEVEL_START) {
                param(FirebaseAnalytics.Param.LEVEL_NAME, gameName)
            }
            val dupCards = cards.copyOf()
            dupCards.shuffle()
            var j = 0
            for (i in 0 until 7) {
                cardStacks[7 + i] = dupCards.copyOfRange(j, j + i + 1).toMutableList()
                val lastCard = cardStacks[7 + i].last()
                lastCard.frontVisible = true
                lastCard.isLast = true
                cardStacks[7 + i].forEachIndexed { index, card ->
                    card.currentStackIndex = index
                    card.currentStackId = 7 + i
                }
                j += i + 1
            }
            cardStacks[6] = dupCards.copyOfRange(j, dupCards.size).toMutableList()
            cardStacks[6].forEachIndexed { index, card ->
                card.currentStackIndex = index
                card.currentStackId = 6
            }
            val stacks = Array(14) { listOf<Int>() }
            for (i in cardStacks.indices) {
                stacks[i] = cardStacks[i].map { c -> if (c.frontVisible) c.id else -1 * c.id - 1 }
            }
        }

        fun reset() {
            finished = false
            solitaireFinished.value = finished
            cardStacks.forEach { s -> s.clear() }
            cards.forEach {
                it.frontVisible = false
                it.isLast = false
                it.currentStackId = 6
                it.currentStackIndex = 0
                it.offset = IntOffset.Zero
            }
        }

        fun resetRestStack() {
            if (cardStacks[5].size == 0 || cardStacks[6].size != 0) return
            cardStacks[6] = cardStacks[5].asReversed().map { card -> card }.toMutableList()
            cardStacks[5].clear()
            cardStacks[6].forEachIndexed { i, card ->
                card.currentStackIndex = i
                card.currentStackId = 6
                card.frontVisible = false
                card.isLast = false
            }
        }

        fun turnFromRestStack(card: PlayingCard) {
            if (cardStacks[6].size == 0) return
            if (cardStacks[6].last().id == card.id) {
                card.currentStackIndex = cardStacks[5].size
                card.currentStackId = 5
                card.frontVisible = true
                card.isLast = true
                cardStacks[5].add(card)
                cardStacks[6].removeAt(cardStacks[6].size - 1)
            }
        }

        fun startMoveCard(card: PlayingCard) {
            if (movingCards.size != 0) return
            if (!card.frontVisible) return
            val stackId = card.currentStackId
            if (stackId <= 5) {
                movingCards.add(cardStacks[stackId].last())
                return
            }
            val stack = cardStacks[stackId]
            val index = card.currentStackIndex
            for (i in index until stack.size) {
                val c = stack[i]
                movingCards.add(c)
            }
            if (index != 0 && stackId >= 7) {
                stack[index - 1].isLast = true
            }
        }

        fun moveCards(offset: Offset) {
            movingCards.forEach { card ->
                card.offset = IntOffset(
                    card.offset.x + offset.x.roundToInt(), card.offset.y + offset.y.roundToInt()
                )
            }
        }

        fun onReleaseMovingCards() {
            if (movingCards.size == 0) return
            val firstCard = movingCards[0]
            val firstCardStackId = firstCard.currentStackId
            val firstCardStackIndex = firstCard.currentStackIndex
            val oldStack = cardStacks[firstCardStackId]
            val currentOffset = SolitaireLayout.calculateBaseOffsetByStackData(
                firstCardStackId, firstCardStackIndex
            ) + firstCard.offset

            val baseOffsetUpperLeft = SolitaireLayout.calculateBaseOffsetByStackData(0, 1)
            val baseOffsetLowerRight = SolitaireLayout.calculateBaseOffsetByStackData(7, 1)
            val heightBorder = baseOffsetLowerRight.y / 2f
            val selectedStackByOffset = clamp(
                ((currentOffset.x - baseOffsetUpperLeft.x) / SolitaireLayout.cardWidth).roundToInt(),
                0,
                6
            )

            var foundNewStack = false

            if (movingCards.size == 1 && (currentOffset.y < heightBorder || firstCardStackId % 7 == selectedStackByOffset)) {
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
                oldStack[firstCardStackIndex - 1].isLast = false
            }

            movingCards.forEach { card ->
                card.offset = IntOffset.Zero
            }
            movingCards.clear()

            if (firstCardStackId >= 7 && oldStack.size >= 1) oldStack.last().frontVisible = true

            if (foundNewStack) checkFinished()
        }

        private fun moveCardToFoundation(card: PlayingCard): Boolean {
            val foundationStackId = floor(card.id / 13f).toInt()
            val foundation = cardStacks[foundationStackId]
            if (card.id % 13 == 0 || (foundation.size != 0 && foundation.last().id % 13 == card.id % 13 - 1)) {
                cardStacks[card.currentStackId].removeLast()
                foundation.add(card)
                card.currentStackId = foundationStackId
                card.currentStackIndex = foundation.size - 1
                card.isLast = true
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
                lastCardNewStack.isLast = false
            }
            newStack.addAll(movingCards)
            val lastIndex = card.currentStackIndex
            val lastStack = cardStacks[card.currentStackId]
            while (lastStack.size > lastIndex) {
                lastStack.removeAt(lastIndex)
            }
            movingCards.forEachIndexed { i, c ->
                c.currentStackIndex = newStackSize + i
                c.currentStackId = newStackId
            }
            return true
        }

        fun checkFinished() {
            finished = isFinished()
            solitaireFinished.value = finished
            if (!finished) {
                couldGetFinished.value = couldGetFinished()
            } else {
                Logger.logEvent(FirebaseAnalytics.Event.LEVEL_END) {
                    param(FirebaseAnalytics.Param.LEVEL_NAME, gameName)
                    param(FirebaseAnalytics.Param.SUCCESS, 1)
                }
            }
        }

        private fun isFinished(): Boolean {
            for (card in cards) {
                if (!card.frontVisible || card.currentStackId >= 4) return false
            }
            return true
        }

        private fun couldGetFinished(): Boolean {
            return !cards.any { !it.frontVisible && it.currentStackId >= 7 }
        }
    }
}