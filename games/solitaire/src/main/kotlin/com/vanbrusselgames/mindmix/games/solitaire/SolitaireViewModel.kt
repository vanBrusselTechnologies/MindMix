package com.vanbrusselgames.mindmix.games.solitaire

import androidx.compose.runtime.mutableStateOf
import androidx.compose.ui.unit.IntOffset
import androidx.core.math.MathUtils.clamp
import androidx.datastore.preferences.core.Preferences
import androidx.navigation.NavController
import com.google.firebase.analytics.FirebaseAnalytics
import com.vanbrusselgames.mindmix.core.common.BaseGameViewModel
import com.vanbrusselgames.mindmix.core.common.GameTimer
import com.vanbrusselgames.mindmix.core.common.ITimerVM
import com.vanbrusselgames.mindmix.core.logging.Logger
import com.vanbrusselgames.mindmix.feature.gamefinished.navigation.navigateToGameFinished
import com.vanbrusselgames.mindmix.games.solitaire.PlayingCard.CardVisualType
import com.vanbrusselgames.mindmix.games.solitaire.data.PREF_KEY_CARD_TYPE
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.async
import kotlinx.coroutines.awaitAll
import kotlinx.coroutines.launch
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json
import kotlin.math.floor
import kotlin.math.max
import kotlin.math.roundToInt

class SolitaireViewModel : BaseGameViewModel(), ITimerVM {
    companion object {
        private const val CARD_PIXEL_HEIGHT = 819f
        private const val CARD_PIXEL_WIDTH = 566f
        const val CARD_ASPECT_RATIO = CARD_PIXEL_WIDTH / CARD_PIXEL_HEIGHT
    }

    private lateinit var coroutineScope: CoroutineScope

    override val nameResId = Solitaire.NAME_RES_ID
    override val descResId = R.string.solitaire_desc

    override val timer = GameTimer()
    private var fastestTime = -1L
    private var moves = 0

    var distanceBetweenCards = 0.175f
    var cardHeight = 0f
    var cardWidth = 0f

    val cards = arrayOf(
        PlayingCard(
            this,
            PlayingCard.CardType.CLOVERS,
            PlayingCard.CardIndex.A,
            R.drawable.playingcards_detailed_clovers_a
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.CLOVERS,
            PlayingCard.CardIndex.TWO,
            R.drawable.playingcards_detailed_clovers_2
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.CLOVERS,
            PlayingCard.CardIndex.THREE,
            R.drawable.playingcards_detailed_clovers_3
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.CLOVERS,
            PlayingCard.CardIndex.FOUR,
            R.drawable.playingcards_detailed_clovers_4
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.CLOVERS,
            PlayingCard.CardIndex.FIVE,
            R.drawable.playingcards_detailed_clovers_5
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.CLOVERS,
            PlayingCard.CardIndex.SIX,
            R.drawable.playingcards_detailed_clovers_6
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.CLOVERS,
            PlayingCard.CardIndex.SEVEN,
            R.drawable.playingcards_detailed_clovers_7
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.CLOVERS,
            PlayingCard.CardIndex.EIGHT,
            R.drawable.playingcards_detailed_clovers_8
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.CLOVERS,
            PlayingCard.CardIndex.NINE,
            R.drawable.playingcards_detailed_clovers_9
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.CLOVERS,
            PlayingCard.CardIndex.TEN,
            R.drawable.playingcards_detailed_clovers_10
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.CLOVERS,
            PlayingCard.CardIndex.J,
            R.drawable.playingcards_detailed_clovers_j
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.CLOVERS,
            PlayingCard.CardIndex.Q,
            R.drawable.playingcards_detailed_clovers_q
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.CLOVERS,
            PlayingCard.CardIndex.K,
            R.drawable.playingcards_detailed_clovers_k
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.DIAMONDS,
            PlayingCard.CardIndex.A,
            R.drawable.playingcards_detailed_diamonds_a
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.DIAMONDS,
            PlayingCard.CardIndex.TWO,
            R.drawable.playingcards_detailed_diamonds_2
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.DIAMONDS,
            PlayingCard.CardIndex.THREE,
            R.drawable.playingcards_detailed_diamonds_3
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.DIAMONDS,
            PlayingCard.CardIndex.FOUR,
            R.drawable.playingcards_detailed_diamonds_4
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.DIAMONDS,
            PlayingCard.CardIndex.FIVE,
            R.drawable.playingcards_detailed_diamonds_5
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.DIAMONDS,
            PlayingCard.CardIndex.SIX,
            R.drawable.playingcards_detailed_diamonds_6
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.DIAMONDS,
            PlayingCard.CardIndex.SEVEN,
            R.drawable.playingcards_detailed_diamonds_7
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.DIAMONDS,
            PlayingCard.CardIndex.EIGHT,
            R.drawable.playingcards_detailed_diamonds_8
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.DIAMONDS,
            PlayingCard.CardIndex.NINE,
            R.drawable.playingcards_detailed_diamonds_9
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.DIAMONDS,
            PlayingCard.CardIndex.TEN,
            R.drawable.playingcards_detailed_diamonds_10
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.DIAMONDS,
            PlayingCard.CardIndex.J,
            R.drawable.playingcards_detailed_diamonds_j
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.DIAMONDS,
            PlayingCard.CardIndex.Q,
            R.drawable.playingcards_detailed_diamonds_q
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.DIAMONDS,
            PlayingCard.CardIndex.K,
            R.drawable.playingcards_detailed_diamonds_k
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.HEARTS,
            PlayingCard.CardIndex.A,
            R.drawable.playingcards_detailed_hearts_a
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.HEARTS,
            PlayingCard.CardIndex.TWO,
            R.drawable.playingcards_detailed_hearts_2
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.HEARTS,
            PlayingCard.CardIndex.THREE,
            R.drawable.playingcards_detailed_hearts_3
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.HEARTS,
            PlayingCard.CardIndex.FOUR,
            R.drawable.playingcards_detailed_hearts_4
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.HEARTS,
            PlayingCard.CardIndex.FIVE,
            R.drawable.playingcards_detailed_hearts_5
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.HEARTS,
            PlayingCard.CardIndex.SIX,
            R.drawable.playingcards_detailed_hearts_6
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.HEARTS,
            PlayingCard.CardIndex.SEVEN,
            R.drawable.playingcards_detailed_hearts_7
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.HEARTS,
            PlayingCard.CardIndex.EIGHT,
            R.drawable.playingcards_detailed_hearts_8
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.HEARTS,
            PlayingCard.CardIndex.NINE,
            R.drawable.playingcards_detailed_hearts_9
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.HEARTS,
            PlayingCard.CardIndex.TEN,
            R.drawable.playingcards_detailed_hearts_10
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.HEARTS,
            PlayingCard.CardIndex.J,
            R.drawable.playingcards_detailed_hearts_j
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.HEARTS,
            PlayingCard.CardIndex.Q,
            R.drawable.playingcards_detailed_hearts_q
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.HEARTS,
            PlayingCard.CardIndex.K,
            R.drawable.playingcards_detailed_hearts_k
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.SPADES,
            PlayingCard.CardIndex.A,
            R.drawable.playingcards_detailed_spades_a
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.SPADES,
            PlayingCard.CardIndex.TWO,
            R.drawable.playingcards_detailed_spades_2
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.SPADES,
            PlayingCard.CardIndex.THREE,
            R.drawable.playingcards_detailed_spades_3
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.SPADES,
            PlayingCard.CardIndex.FOUR,
            R.drawable.playingcards_detailed_spades_4
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.SPADES,
            PlayingCard.CardIndex.FIVE,
            R.drawable.playingcards_detailed_spades_5
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.SPADES,
            PlayingCard.CardIndex.SIX,
            R.drawable.playingcards_detailed_spades_6
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.SPADES,
            PlayingCard.CardIndex.SEVEN,
            R.drawable.playingcards_detailed_spades_7
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.SPADES,
            PlayingCard.CardIndex.EIGHT,
            R.drawable.playingcards_detailed_spades_8
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.SPADES,
            PlayingCard.CardIndex.NINE,
            R.drawable.playingcards_detailed_spades_9
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.SPADES,
            PlayingCard.CardIndex.TEN,
            R.drawable.playingcards_detailed_spades_10
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.SPADES,
            PlayingCard.CardIndex.J,
            R.drawable.playingcards_detailed_spades_j
        ),
        PlayingCard(
            this,
            PlayingCard.CardType.SPADES,
            PlayingCard.CardIndex.Q,
            R.drawable.playingcards_detailed_spades_q
        ),
        PlayingCard(
            this,
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

    fun setCoroutineScope(scope: CoroutineScope) {
        coroutineScope = scope
    }

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
                c.isMoving = false
                c.calculateBaseOffset()
                c.recalculateZIndex()
                cardStacks[i].add(c)
            }
        }
        couldGetFinished.value = couldGetFinished()
        timer.set(data.millis)
        timer.addMillis(data.penaltyMillis)
        fastestTime = data.fastestMillis
        moves = data.moves

        if (cardStacks[6].isEmpty() && cardStacks[5].size <= 1) restStackEnabled.value = false
    }

    override fun onLoadPreferences(preferences: Preferences) {
        if (preferences[PREF_KEY_CARD_TYPE] != null) {
            cardVisualType.value =
                CardVisualType.entries.first { it.ordinal == preferences[PREF_KEY_CARD_TYPE] }
        }
    }

    fun saveToFile(): String {
        val stacks = Array(14) { listOf<Int>() }
        for (i in cardStacks.indices) {
            stacks[i] = cardStacks[i].map { c -> if (c.frontVisible.value) c.id else -1 * c.id - 1 }
        }
        return Json.encodeToString(
            SolitaireData(
                stacks.asList(),
                finished,
                timer.currentMillis,
                timer.addedMillis,
                fastestTime,
                moves
            )
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
        if (finished) return startNewGame()
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
                card.isMoving = false
                card.calculateBaseOffset()
                card.recalculateZIndex()
            }
            j += i + 1
        }
        cardStacks[6] = dupCards.copyOfRange(j, dupCards.size).toMutableList()
        cardStacks[6].forEachIndexed { index, card ->
            card.stackIndex = index
            card.stackId = 6
            card.visible.value = index == 0
            card.isMoving = false
            card.calculateBaseOffset()
            card.recalculateZIndex()
        }
        val stacks = Array(14) { listOf<Int>() }
        for (i in cardStacks.indices) {
            stacks[i] = cardStacks[i].map { c -> if (c.frontVisible.value) c.id else -1 * c.id - 1 }
        }
        couldGetFinished.value = couldGetFinished()
        restStackEnabled.value = true
        Logger.logEvent(FirebaseAnalytics.Event.LEVEL_START) {
            param(FirebaseAnalytics.Param.LEVEL_NAME, Solitaire.GAME_NAME)
        }
        coroutineScope.launch {
            cards.map { async { it.animOffset.animateTo(it.baseOffset) } }.awaitAll()
            timer.start()
        }
    }

    private fun reset() {
        finished = false
        cardStacks.forEach { s -> s.clear() }
        cards.forEach {
            it.frontVisible.value = false
            it.isLast.value = false
            it.stackId = 6
            it.stackIndex = 0
            it.isMoving = false
            it.calculateBaseOffset()
            it.recalculateZIndex()
        }
        moves = 0
        timer.reset()
    }

    override fun startNewGame() {
        reset()
        loadPuzzle()
    }

    fun resetRestStack() {
        if (restStackEnabled.value && cardStacks[6].isNotEmpty()) return
        cardStacks[6] = cardStacks[5].asReversed().map { card -> card }.toMutableList()
        cardStacks[5].clear()
        cardStacks[6].forEachIndexed { i, card ->
            coroutineScope.launch {
                card.stackIndex = i
                card.stackId = 6
                card.frontVisible.value = false
                card.isLast.value = false
                card.visible.value = i == 0
                card.isMoving = false
                card.calculateBaseOffset()
                card.recalculateZIndex()
                card.animOffset.animateTo(card.baseOffset)
            }
        }
    }

    fun turnFromRestStack() {
        if (cardStacks[6].isEmpty()) return
        val card = cardStacks[6].last()
        val index = cardStacks[5].size
        card.stackIndex = index
        card.stackId = 5
        card.visible.value = true
        card.frontVisible.value = true
        card.isLast.value = true
        card.isMoving = false
        coroutineScope.launch {
            card.calculateBaseOffset()
            card.recalculateZIndex()
            card.animOffset.animateTo(card.baseOffset)
        }
        cardStacks[5].add(card)
        cardStacks[6].removeAt(cardStacks[6].lastIndex)
        if (cardStacks[6].isEmpty() && cardStacks[5].size <= 1) restStackEnabled.value = false
    }

    fun startMoveCard(card: PlayingCard) {
        if (movingCards.isNotEmpty()) return
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

    fun moveCards(intOffset: IntOffset) {
        movingCards.forEach { card ->
            coroutineScope.launch {
                card.offset += intOffset
                card.animOffset.animateTo(card.baseOffset + card.offset)
            }
        }
    }

    fun onReleaseMovingCards(navController: NavController) {
        if (movingCards.isEmpty()) return
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
            card.isMoving = false
            coroutineScope.launch {
                card.animOffset.animateTo(card.baseOffset)
                card.recalculateZIndex()
            }
        }
        movingCards.clear()

        if (firstCardStackId >= 7 && oldStack.isNotEmpty()) oldStack.last().frontVisible.value =
            true

        moves++

        if (foundNewStack) checkFinished(navController)
        else timer.addMillis(15000)

        if (cardStacks[6].isEmpty() && cardStacks[5].size <= 1) restStackEnabled.value = false
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

    fun onClickFinishGame(navController: NavController) {
        cards.forEach {
            it.stackId = it.type.ordinal
            it.stackIndex = it.index.ordinal
            it.isMoving = false
            it.isLast.value = true
            it.frontVisible.value = true
            coroutineScope.launch {
                it.calculateBaseOffset()
                it.recalculateZIndex()
                it.animOffset.animateTo(it.baseOffset)
            }
        }
        checkFinished(navController)
    }

    fun checkFinished(navController: NavController) {
        finished = isFinished()
        if (finished) {
            timer.stop()
            Logger.logEvent(FirebaseAnalytics.Event.LEVEL_END) {
                param(FirebaseAnalytics.Param.LEVEL_NAME, Solitaire.GAME_NAME)
                param(FirebaseAnalytics.Param.SUCCESS, 1)
            }
            onGameFinished(navController)
        }
        couldGetFinished.value = !finished && couldGetFinished()
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

    private fun onGameFinished(navController: NavController) {
        FinishedGame.titleResId = Solitaire.NAME_RES_ID// "Congrats / Smart / Well done"
        FinishedGame.textResId = R.string.solitaire_success
        // """You did great and solved puzzle in ${0} seconds!!
        //     |That's Awesome!
        //     |Share with your friends and challenge them to beat your time!""".trimMargin()

        FinishedGame.moves = moves
        FinishedGame.usedMillis = timer.currentMillis
        FinishedGame.penaltyMillis = timer.addedMillis
        FinishedGame.lastRecordMillis = fastestTime

        val totalUsedTime = timer.currentMillis + timer.addedMillis
        val isNewRecord = fastestTime == -1L || fastestTime > totalUsedTime
        if (isNewRecord) fastestTime = totalUsedTime
        FinishedGame.isNewRecord = isNewRecord

        val minutes = max(1f, timer.currentMillis / 1000f / 60f)
        FinishedGame.reward =
            max(1, floor(MAX_REWARD / minutes).toInt()) + if (isNewRecord) 2 else 0

        navController.navigateToGameFinished()
    }

    override fun onOpenDialog() {
        super.onOpenDialog()
        timer.pause()
    }
}