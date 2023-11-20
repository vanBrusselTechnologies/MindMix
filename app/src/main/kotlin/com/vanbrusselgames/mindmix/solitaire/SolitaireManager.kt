package com.vanbrusselgames.mindmix.solitaire

import androidx.compose.ui.geometry.Offset
import androidx.compose.ui.unit.IntOffset
import androidx.core.math.MathUtils.clamp
import com.vanbrusselgames.mindmix.R
import kotlin.math.floor
import kotlin.math.roundToInt

class SolitaireManager {
    companion object Instance {
        private val cards = arrayOf(
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
            //todo: Add other cards drawableResId
            PlayingCard(
                PlayingCard.CardType.HEARTS,
                PlayingCard.CardIndex.A,
                R.drawable.playingcards_detailed_clovers_a
            ),
            PlayingCard(
                PlayingCard.CardType.HEARTS,
                PlayingCard.CardIndex.TWO,
                R.drawable.playingcards_detailed_clovers_a
            ),
            PlayingCard(
                PlayingCard.CardType.HEARTS,
                PlayingCard.CardIndex.THREE,
                R.drawable.playingcards_detailed_clovers_a
            ),
            PlayingCard(
                PlayingCard.CardType.HEARTS,
                PlayingCard.CardIndex.FOUR,
                R.drawable.playingcards_detailed_clovers_a
            ),
            PlayingCard(
                PlayingCard.CardType.HEARTS,
                PlayingCard.CardIndex.FIVE,
                R.drawable.playingcards_detailed_clovers_a
            ),
            PlayingCard(
                PlayingCard.CardType.HEARTS,
                PlayingCard.CardIndex.SIX,
                R.drawable.playingcards_detailed_clovers_a
            ),
            PlayingCard(
                PlayingCard.CardType.HEARTS,
                PlayingCard.CardIndex.SEVEN,
                R.drawable.playingcards_detailed_clovers_a
            ),
            PlayingCard(
                PlayingCard.CardType.HEARTS,
                PlayingCard.CardIndex.EIGHT,
                R.drawable.playingcards_detailed_clovers_a
            ),
            PlayingCard(
                PlayingCard.CardType.HEARTS,
                PlayingCard.CardIndex.NINE,
                R.drawable.playingcards_detailed_clovers_a
            ),
            PlayingCard(
                PlayingCard.CardType.HEARTS,
                PlayingCard.CardIndex.TEN,
                R.drawable.playingcards_detailed_clovers_a
            ),
            PlayingCard(
                PlayingCard.CardType.HEARTS,
                PlayingCard.CardIndex.J,
                R.drawable.playingcards_detailed_clovers_a
            ),
            PlayingCard(
                PlayingCard.CardType.HEARTS,
                PlayingCard.CardIndex.Q,
                R.drawable.playingcards_detailed_clovers_a
            ),
            PlayingCard(
                PlayingCard.CardType.HEARTS,
                PlayingCard.CardIndex.K,
                R.drawable.playingcards_detailed_clovers_a
            ),
            PlayingCard(
                PlayingCard.CardType.SPADES,
                PlayingCard.CardIndex.A,
                R.drawable.playingcards_detailed_clovers_a
            ),
            PlayingCard(
                PlayingCard.CardType.SPADES,
                PlayingCard.CardIndex.TWO,
                R.drawable.playingcards_detailed_clovers_a
            ),
            PlayingCard(
                PlayingCard.CardType.SPADES,
                PlayingCard.CardIndex.THREE,
                R.drawable.playingcards_detailed_clovers_a
            ),
            PlayingCard(
                PlayingCard.CardType.SPADES,
                PlayingCard.CardIndex.FOUR,
                R.drawable.playingcards_detailed_clovers_a
            ),
            PlayingCard(
                PlayingCard.CardType.SPADES,
                PlayingCard.CardIndex.FIVE,
                R.drawable.playingcards_detailed_clovers_a
            ),
            PlayingCard(
                PlayingCard.CardType.SPADES,
                PlayingCard.CardIndex.SIX,
                R.drawable.playingcards_detailed_clovers_a
            ),
            PlayingCard(
                PlayingCard.CardType.SPADES,
                PlayingCard.CardIndex.SEVEN,
                R.drawable.playingcards_detailed_clovers_a
            ),
            PlayingCard(
                PlayingCard.CardType.SPADES,
                PlayingCard.CardIndex.EIGHT,
                R.drawable.playingcards_detailed_clovers_a
            ),
            PlayingCard(
                PlayingCard.CardType.SPADES,
                PlayingCard.CardIndex.NINE,
                R.drawable.playingcards_detailed_clovers_a
            ),
            PlayingCard(
                PlayingCard.CardType.SPADES,
                PlayingCard.CardIndex.TEN,
                R.drawable.playingcards_detailed_clovers_a
            ),
            PlayingCard(
                PlayingCard.CardType.SPADES,
                PlayingCard.CardIndex.J,
                R.drawable.playingcards_detailed_clovers_a
            ),
            PlayingCard(
                PlayingCard.CardType.SPADES,
                PlayingCard.CardIndex.Q,
                R.drawable.playingcards_detailed_clovers_a
            ),
            PlayingCard(
                PlayingCard.CardType.SPADES,
                PlayingCard.CardIndex.K,
                R.drawable.playingcards_detailed_clovers_a
            ),
        )

        private val movingCards: MutableList<PlayingCard> = mutableListOf()
        val cardStacks: Array<MutableList<PlayingCard>> = Array(14) { mutableListOf() }

        fun loadPuzzle() {/* This should be moved to 'reset()'
            movingCards.clear()
            for (cardStack in cardStacks) {
                cardStack.clear()
            }
            for (card in cards) {
                card.frontVisible = false
            }
            */
            if (cards[0].currentStackIndex == -1) {
                cards.shuffle()
                var j = 0
                for (i in 0 until 7) {
                    cardStacks[7 + i] = cards.copyOfRange(j, j + i + 1).toMutableList()
                    cardStacks[7 + i].last().frontVisible = true
                    cardStacks[7 + i].forEachIndexed { index, card ->
                        card.currentStackIndex = index
                        card.currentStackId = 7 + i
                    }
                    j += i + 1
                }
                cardStacks[6] = cards.copyOfRange(j, cards.size).toMutableList()
                cardStacks[6].forEachIndexed { index, card ->
                    card.currentStackIndex = index
                    card.currentStackId = 6
                }
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
            }
        }

        fun turnFromRestStack(card: PlayingCard) {
            if (cardStacks[6].size == 0) return
            if (cardStacks[6].last().id == card.id) {
                card.currentStackIndex = cardStacks[5].size
                card.currentStackId = 5
                card.frontVisible = true
                cardStacks[5].add(card)
                cardStacks[6].removeAt(cardStacks[6].size - 1)
            }
        }

        fun startMoveCard(card: PlayingCard) {
            if (movingCards.size != 0) return
            if (!card.frontVisible) return
            val stackId = card.currentStackId
            if (stackId == 5) {
                movingCards.add(cardStacks[5].last())
                return
            }
            val stack = cardStacks[stackId]
            val index = card.currentStackIndex
            for (i in index until stack.size) {
                val c = stack[i]
                movingCards.add(c)
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
            val oldStack = cardStacks[firstCardStackId]
            val currentOffset = SolitaireLayout.calculateBaseOffsetByStackData(
                firstCardStackId, firstCard.currentStackIndex
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

            movingCards.forEach { card ->
                card.offset = IntOffset(0, 0)
            }
            movingCards.clear()

            if (firstCardStackId >= 7 && oldStack.size >= 1) {
                oldStack.last().frontVisible = true
            }
        }

        private fun moveCardToFoundation(card: PlayingCard): Boolean {
            val foundationStackId = floor(card.id / 13f).toInt()
            val foundation = cardStacks[foundationStackId]
            if (card.id % 13 == 0 || (foundation.size != 0 && foundation.last().id % 13 == card.id % 13 - 1)) {
                foundation.add(card)
                cardStacks[card.currentStackId].removeLast()
                card.currentStackId = foundationStackId
                card.currentStackIndex = foundation.size - 1
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
            }
            newStack.addAll(movingCards)
            val lastIndex = card.currentStackIndex
            val lastStack = cardStacks[card.currentStackId]
            while (lastStack.size > lastIndex) {
                lastStack.removeAt(lastIndex)
            }
            var i = 0
            movingCards.forEach { c ->
                c.currentStackIndex = newStackSize + i
                c.currentStackId = newStackId
                i++
            }
            return true
        }

        fun checkFinished() {
            //MinesweeperData.Finised = isFinished()
            //solitaireFinished.value = MinesweeperData.Finised
        }

        private fun isFinished(): Boolean {
            for (card in cards) {
                if (!card.frontVisible || card.currentStackId >= 4) return false
            }
            return true
        }
    }
}