package com.vanbrusselgames.mindmix.solitaire

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
import androidx.compose.material3.Card
import androidx.compose.material3.Icon
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.alpha
import androidx.compose.ui.draw.blur
import androidx.compose.ui.platform.LocalDensity
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.IntOffset
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.times
import com.vanbrusselgames.mindmix.BaseLayout
import com.vanbrusselgames.mindmix.BaseUIHandler
import com.vanbrusselgames.mindmix.R
import com.vanbrusselgames.mindmix.solitaire.SolitaireManager.Instance.solitaireFinished
import kotlin.math.min
import kotlin.math.roundToInt

class SolitaireLayout : BaseLayout() {
    companion object {
        private const val cardPixelHeight = 819f
        private const val cardPixelWidth = 566f
        private const val cardAspectRatio = cardPixelWidth / cardPixelHeight
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

    override var uiHandler: BaseUIHandler = SolitaireUIHandler()
    private val arr = arrayOf(0, 1, 2, 3, 4, 5, 6)

    @Composable
    fun BaseScene() {
        super.BaseScene(isMenu = false, sceneSpecific = {
            val finished = solitaireFinished.value
            SolitaireSpecificLayout(finished)
            if (finished) GameFinishedPopUp()
        })
    }

    @Composable
    fun SolitaireSpecificLayout(isBlurred: Boolean) {
        Box(
            Modifier
                .fillMaxSize()
                .blur(if (isBlurred) 4.dp else 0.dp),
            contentAlignment = Alignment.Center
        ) {
            Box(Modifier.fillMaxSize(0.95f), contentAlignment = Alignment.TopCenter) {
                BoxWithConstraints {
                    val maxHeight = constraints.maxHeight
                    val maxWidth = constraints.maxWidth
                    val maxCardHeight = maxHeight / 4.2375f
                    cardHeight =
                        if (maxWidth / maxHeight > cardAspectRatio) maxCardHeight else maxWidth / 7f / cardAspectRatio
                    cardWidth = cardHeight * cardAspectRatio
                    distanceBetweenCards = min(maxCardHeight / cardHeight, 2f) * 0.175f
                    val cardHeightInDp = with(LocalDensity.current) { cardHeight.toDp() }
                    val modifier = Modifier
                        .width(cardHeightInDp * cardAspectRatio)
                        .height(cardHeightInDp)
                        .aspectRatio(cardAspectRatio)

                    Background(modifier = modifier, cardHeight = cardHeightInDp)

                    SolitaireManager.cards.forEach { it.Composable(modifier) }
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
                    6 -> Card({ SolitaireManager.resetRestStack() }, modifier.alpha(0.75f)) {
                        Box(Modifier.fillMaxSize()) {
                            Icon(
                                painterResource(id = R.drawable.outline_autorenew_24),
                                "Stock",
                                Modifier
                                    .align(Alignment.Center).fillMaxSize()
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
        val title = "Congrats"
        val desc = """You did great and solved puzzle in ${0} seconds!!
                        |That's Awesome!
                        |Share with your friends and challenge them to beat your time!""".trimMargin()
        //Logger.logEvent(FirebaseAnalytics.Event.EARN_VIRTUAL_CURRENCY)
        val reward = 10
        val onClickShare = {}
        val onClickPlayAgain = {
            SolitaireManager.reset()
            SolitaireManager.loadPuzzle()
        }
        val onClickReturnToMenu = {
            uiHandler.backToMenu()
            SolitaireManager.reset()
        }
        BaseGameFinishedPopUp(
            title, desc, reward, onClickShare, onClickPlayAgain, onClickReturnToMenu
        )
    }
}