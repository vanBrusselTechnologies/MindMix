package com.vanbrusselgames.mindmix.games.solitaire.ui

import androidx.compose.animation.core.animateDpAsState
import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.LocalTextStyle
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.MutableState
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalDensity
import androidx.compose.ui.platform.LocalResources
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.TextStyle
import androidx.compose.ui.text.style.LineHeightStyle
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.text.style.TextMotion
import androidx.compose.ui.unit.dp
import androidx.compose.ui.zIndex
import com.vanbrusselgames.mindmix.core.utils.PixelHelper
import com.vanbrusselgames.mindmix.games.solitaire.R
import com.vanbrusselgames.mindmix.games.solitaire.model.CardVisualType
import com.vanbrusselgames.mindmix.games.solitaire.model.PlayingCard
import com.vanbrusselgames.mindmix.games.solitaire.viewmodel.ISolitaireViewModel
import kotlin.math.roundToInt

@Composable
fun PlayingCard(viewModel: ISolitaireViewModel, model: PlayingCard, modifier: Modifier) {
    LaunchedEffect(model.targetOffset.value) {
        model.animOffset.animateTo(model.targetOffset.value)
        model.recalculateZIndex()
    }
    if (!model.visible.value) return Box {}
    val mod = modifier
        .zIndex(model.zIndex.floatValue)
        .offset { model.animOffset.value }
    when (viewModel.cardVisualType.value) {
        CardVisualType.DETAILED -> DetailedCard(model, mod)
        CardVisualType.SIMPLE -> SimpleCard(viewModel, model, mod)
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
private fun SimpleCard(viewModel: ISolitaireViewModel, model: PlayingCard, modifier: Modifier) {
    Box(modifier, contentAlignment = Alignment.TopCenter) {
        SimpleCardBackground(model.frontVisible)
        if (!model.frontVisible.value) return@Box
        SimpleCardContent(viewModel, model)
    }
}

@Composable
fun SimpleCardBackground(frontVisible: MutableState<Boolean>) {
    if (frontVisible.value) {
        val shape = remember { RoundedCornerShape(3.dp) }
        Box(
            Modifier
                .fillMaxSize()
                .border(1.dp, Color.Black, shape)
                .background(Color.White, shape)
        ) {}
        return
    }
    Image(
        painterResource(R.drawable.playingcards_detailed_back),
        "Playing card",
        Modifier.fillMaxSize()
    )
}

@Composable
fun SimpleCardContent(viewModel: ISolitaireViewModel, model: PlayingCard) {
    val cardHeightDp = viewModel.cardSizeDp.value.height
    val cardHeightInDpTarget =
        cardHeightDp * (if (model.isLast.value) 1f else viewModel.distanceBetweenCards)
    val cardHeightInDp = animateDpAsState(cardHeightInDpTarget, label = "selectedFactor").value
    Box(Modifier.height(cardHeightInDp), Alignment.Center) {
        val localDensity = LocalDensity.current
        val resources = LocalResources.current
        val cardWidth = viewModel.cardSize.value.width
        val widthScale = if (model.contentLength == 2) 1.15f else 1f
        val maxWidth = (cardWidth * 0.33f * widthScale).roundToInt()
        val maxHeight = with(localDensity) { (cardHeightInDp * 0.75f).toPx() }.roundToInt()
        val fontSize = PixelHelper.pxToSp(resources, minOf(maxWidth, maxHeight))
        val localStyle = LocalTextStyle.current
        val style = remember {
            localStyle.merge(
                TextStyle(
                    model.color, lineHeightStyle = LineHeightStyle(
                        alignment = LineHeightStyle.Alignment.Center,
                        trim = LineHeightStyle.Trim.Both
                    ), textMotion = TextMotion.Animated
                )
            )
        }
        Text(
            model.contentString,
            fontSize = fontSize,
            textAlign = TextAlign.End,
            maxLines = 1,
            style = style
        )
    }
}