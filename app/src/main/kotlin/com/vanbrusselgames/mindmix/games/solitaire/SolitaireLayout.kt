package com.vanbrusselgames.mindmix.games.solitaire

import androidx.compose.foundation.Image
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.BoxWithConstraints
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.width
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.Card
import androidx.compose.material3.Icon
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.alpha
import androidx.compose.ui.draw.blur
import androidx.compose.ui.platform.LocalDensity
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.IntOffset
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.times
import com.vanbrusselgames.mindmix.BaseLayout
import com.vanbrusselgames.mindmix.BaseUIHandler
import com.vanbrusselgames.mindmix.R
import com.vanbrusselgames.mindmix.games.GameHelp
import com.vanbrusselgames.mindmix.games.GameMenu
import com.vanbrusselgames.mindmix.games.solitaire.SolitaireManager.Instance.cards
import com.vanbrusselgames.mindmix.games.solitaire.SolitaireManager.Instance.couldGetFinished
import com.vanbrusselgames.mindmix.games.solitaire.SolitaireManager.Instance.solitaireFinished
import com.vanbrusselgames.mindmix.games.solitaire.SolitaireManager.Instance.timer
import kotlin.math.floor
import kotlin.math.max
import kotlin.math.min
import kotlin.math.roundToInt

class SolitaireLayout : BaseLayout() {
    companion object {
        private const val CARD_PIXEL_HEIGHT = 819f
        private const val CARD_PIXEL_WIDTH = 566f
        private const val CARD_ASPECT_RATIO = CARD_PIXEL_WIDTH / CARD_PIXEL_HEIGHT
        var distanceBetweenCards = 0.175f
        var cardHeight = 0f
        var cardWidth = 0f

        fun calculateBaseOffsetByStackData(stackId: Int, stackIndex: Int): IntOffset {
            val x = (stackId % 7) * cardWidth
            val y =
                if (stackId < 7) 0f else (1 + (0.5f + stackIndex) * distanceBetweenCards) * cardHeight
            return IntOffset(x.roundToInt(), y.roundToInt())
        }
    }

    private val arr = arrayOf(0, 1, 2, 3, 4, 5, 6)

    @Composable
    fun Scene() {
        BaseScene {
            SolitaireSpecificLayout(disableTopRowButtons.value)
            GameHelp.Screen(R.string.solitaire_name, R.string.solitaire_desc, timer)
            GameMenu.Screen(R.string.solitaire_name, timer) { SolitaireManager.startNewGame() }
            SolitaireSettings()
            if (solitaireFinished.value) GameFinishedPopUp()
            Box(
                Modifier
                    .fillMaxSize()
                    .blur(if (disableTopRowButtons.value) blurStrength else 0.dp),
                Alignment.BottomCenter
            ) {
                timer.Timer()
            }
        }
    }

    @Composable
    fun SolitaireSpecificLayout(isBlurred: Boolean) {
        Box(
            Modifier
                .fillMaxSize()
                .blur(if (isBlurred) blurStrength else 0.dp), Alignment.Center
        ) {
            Box(Modifier.fillMaxSize(0.95f), contentAlignment = Alignment.TopCenter) {
                BoxWithConstraints {
                    val maxHeight = constraints.maxHeight
                    val maxWidth = constraints.maxWidth
                    val maxCardHeight = maxHeight / 4.2375f
                    cardHeight =
                        if (maxWidth / maxHeight > CARD_ASPECT_RATIO) maxCardHeight else maxWidth / 7f / CARD_ASPECT_RATIO
                    cardWidth = cardHeight * CARD_ASPECT_RATIO
                    distanceBetweenCards = min(maxCardHeight / cardHeight, 2f) * 0.175f
                    val cardHeightInDp = with(LocalDensity.current) { cardHeight.toDp() }
                    val modifier = Modifier
                        .width(cardHeightInDp * CARD_ASPECT_RATIO)
                        .height(cardHeightInDp)
                        .aspectRatio(CARD_ASPECT_RATIO)

                    Background(modifier = modifier, cardHeight = cardHeightInDp)

                    cards.forEach { it.Composable(modifier) }

                    if (couldGetFinished.value && !solitaireFinished.value) {
                        var colors = ButtonDefaults.buttonColors()
                        colors = colors.copy(
                            disabledContainerColor = colors.containerColor,
                            disabledContentColor = colors.contentColor
                        )
                        Button(
                            onClick = {
                                cards.forEach {
                                    it.currentStackId = it.type.ordinal
                                    it.currentStackIndex = it.index.ordinal
                                    it.offset = IntOffset.Zero
                                    it.isLast = true
                                    it.frontVisible = true
                                }
                                SolitaireManager.checkFinished()
                            },
                            Modifier.align(Alignment.BottomCenter),
                            enabled = !disableTopRowButtons.value,
                            colors = colors
                        ) {
                            Icon(painterResource(R.drawable.baseline_flag_24), "Finish flag")
                            Spacer(Modifier.width(2.dp))
                            Text(stringResource(R.string.finish_game))
                        }
                    }
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
        Row {
            for (i in arr) {
                when (i) {
                    4 -> Box(modifier)
                    5 -> Box(modifier)
                    6 -> Card(
                        { SolitaireManager.resetRestStack() },
                        modifier.alpha(0.75f),
                        !disableTopRowButtons.value
                    ) {
                        Box(Modifier.fillMaxSize()) {
                            Icon(
                                painterResource(id = R.drawable.outline_autorenew_24),
                                "Stock",
                                Modifier
                                    .align(Alignment.Center)
                                    .fillMaxSize()
                            )
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
                        FoundationBackground(resourceId, i)
                    }
                }
            }
        }
    }

    @Composable
    fun BackgroundLowerRow(modifier: Modifier, cardHeight: Dp) {
        Row(Modifier.height(18 * distanceBetweenCards * cardHeight)) {
            for (i in arr) {
                Card(modifier.alpha(0.5f)) {}
            }
        }
    }

    @Composable
    fun FoundationBackground(resourceId: Int, stackIndex: Int) {
        Box(Modifier.fillMaxSize()) {
            if (resourceId != 0) {
                Image(
                    painterResource(id = resourceId),
                    "pile $stackIndex",
                    Modifier
                        .fillMaxSize(0.9f)
                        .align(Alignment.Center)
                )
            }
        }
    }

    @Composable
    fun GameFinishedPopUp() {
        val title = stringResource(R.string.solitaire_name)//"Congrats / Smart / Well done"
        val desc = stringResource(R.string.solitaire_success)
        val minutes = max(1f, timer.currentMillis / 1000f / 60f)
        val reward = max(1, floor(MAX_REWARD / minutes).toInt())
        //val onClickShare = {}
        val onClickPlayAgain = {
            SolitaireManager.startNewGame()
        }
        BaseGameFinishedPopUp(
            title, desc, reward, null, onClickPlayAgain, null
        ) {
            Text(text = "${stringResource(R.string.time)}${timer.formatTime(true)}")
            //"""You did great and solved puzzle in ${0} seconds!!
            //     |That's Awesome!
            //     |Share with your friends and challenge them to beat your time!""".trimMargin()
        }
    }
}