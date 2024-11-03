package com.vanbrusselgames.mindmix.core.ui

import androidx.compose.runtime.Composable
import androidx.compose.ui.platform.LocalDensity
import androidx.compose.ui.text.TextStyle
import androidx.compose.ui.text.rememberTextMeasurer
import androidx.compose.ui.unit.Dp

@Composable
fun measureTextWidth(text: String, style: TextStyle, maxLines: Int = 1): Dp {
    val textMeasurer = rememberTextMeasurer()
    val widthInPixels = textMeasurer.measure(text, style, maxLines = maxLines).size.width
    return with(LocalDensity.current) { widthInPixels.toDp() }
}