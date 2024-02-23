package com.vanbrusselgames.mindmix

import androidx.compose.foundation.layout.BoxWithConstraints
import androidx.compose.foundation.layout.BoxWithConstraintsScope
import androidx.compose.foundation.text.InlineTextContent
import androidx.compose.foundation.text.InternalFoundationTextApi
import androidx.compose.foundation.text.TextDelegate
import androidx.compose.material3.LocalTextStyle
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.CompositionLocalProvider
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalDensity
import androidx.compose.ui.platform.LocalFontFamilyResolver
import androidx.compose.ui.platform.LocalLayoutDirection
import androidx.compose.ui.text.AnnotatedString
import androidx.compose.ui.text.TextLayoutResult
import androidx.compose.ui.text.TextStyle
import androidx.compose.ui.text.font.FontFamily
import androidx.compose.ui.text.font.FontStyle
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.text.style.TextDecoration
import androidx.compose.ui.text.style.TextOverflow
import androidx.compose.ui.unit.Density
import androidx.compose.ui.unit.TextUnit
import androidx.compose.ui.unit.sp
import kotlin.math.abs
import kotlin.math.ceil

@Composable
fun AutoSizeText(
    text: AnnotatedString,
    modifier: Modifier = Modifier,
    color: Color = Color.Unspecified,
    suggestedFontSizes: List<TextUnit> = emptyList(),
    minTextSize: TextUnit = TextUnit.Unspecified,
    maxTextSize: TextUnit = TextUnit.Unspecified,
    stepGranularityTextSize: TextUnit = TextUnit.Unspecified,
    fontStyle: FontStyle? = null,
    fontWeight: FontWeight? = null,
    fontFamily: FontFamily? = null,
    letterSpacing: TextUnit = TextUnit.Unspecified,
    textDecoration: TextDecoration? = null,
    textAlign: TextAlign? = null,
    lineHeight: TextUnit = TextUnit.Unspecified,
    maxLines: Int = Int.MAX_VALUE,
    inlineContent: Map<String, InlineTextContent> = mapOf(),
    onTextLayout: (TextLayoutResult) -> Unit = {},
    style: TextStyle = LocalTextStyle.current,
) {
    val density = LocalDensity.current.density
    // Change font scale to 1
    CompositionLocalProvider(LocalDensity provides Density(density = density, fontScale = 1F)) {
        BoxWithConstraints(
            modifier = modifier,
            contentAlignment = when (textAlign) {
                TextAlign.Start -> Alignment.CenterStart
                TextAlign.End -> Alignment.CenterEnd
                else -> Alignment.Center
            },
        ) {
            // (1 / density).sp represents 1px when font scale equals 1
            val step = (1 / density).run {
                if (stepGranularityTextSize.isSp)
                    stepGranularityTextSize.value.coerceAtLeast(this).sp
                else this.sp
            }
            val max = minOf(maxWidth, maxHeight).value.run {
                if (maxTextSize.isSp)
                    maxTextSize.value.coerceAtMost(this).sp
                else this.sp
            }
            val min = minTextSize.takeIf { it.isSp && it <= max } ?: step

            val possibleFontSizes = remember(suggestedFontSizes, min, max, step) {
                suggestedFontSizes.filter {
                    it.value in min.value..max.value
                }.sortedBy {
                    it.value
                }.reversed().ifEmpty {
                    val first = ceil(min.value / step.value).toInt()
                    val iteration = ((max.value - (step * first).value) / step.value).toInt() + 1
                    MutableList(iteration) {
                        step * (first + iteration - 1 - it)
                    }
                }
            }

            // Dichotomous search
            var currentIndex = 0
            // we start at the middle
            var nextIndex = (possibleFontSizes.count() - 1) / 2
            var combinedTextStyle = LocalTextStyle.current + style
            // var counter = 0
            while (true) {
                // counter ++
                val diff = abs(currentIndex - nextIndex)
                if (diff < 2) break // diff < 2 means no change because diff = 1 and diff/2 = 0
                currentIndex = nextIndex
                combinedTextStyle = combinedTextStyle.copy(fontSize = possibleFontSizes[nextIndex])
                nextIndex =
                    if (shouldShrink(text, combinedTextStyle, maxLines))
                        nextIndex + diff / 2
                    else
                        nextIndex - diff / 2
            }

            var index = minOf(currentIndex, nextIndex)
            combinedTextStyle = combinedTextStyle.copy(fontSize = possibleFontSizes[index])
            while (shouldShrink(text, combinedTextStyle, maxLines)) {
                // counter ++
                try {
                    combinedTextStyle =
                        combinedTextStyle.copy(fontSize = possibleFontSizes[++index])
                } catch (_: Exception) {
                    Logger.w("AutoSizeText: Text cannot be shrunk further")
                    break
                }
            }

            // To see produced list values and the number of iterations, uncomment the next line
            // Logger.d("counter = $counter, step = $step, [$max, $min] [${possibleFontSizes.first()}, ${possibleFontSizes.last()}]")
            // Example:
            // counter = 4, step = 1.0.sp, [27.636364.sp, 1.0.sp] [27.0.sp, 1.0.sp]
            // counter = 7, step = 1.0.sp, [70.181816.sp, 1.0.sp] [70.0.sp, 1.0.sp]
            // counter = 4, step = 7.0.sp, [70.181816.sp, 7.0.sp] [70.0.sp, 7.0.sp]
            // counter = 2, step = 9.0.sp, [70.181816.sp, 9.0.sp] [63.0.sp, 9.0.sp]
            // counter = 5, step = 0.36363637.sp, [16.363636.sp, 0.36363637.sp] [16.363636.sp, 0.36363637.sp]
            // counter = 4, step = 0.36363637.sp, [10.909091.sp, 0.36363637.sp] [10.909091.sp, 0.36363637.sp]

            Text(
                text = text,
                modifier = Modifier,
                color = color,
                fontSize = TextUnit.Unspecified,
                fontStyle = fontStyle,
                fontWeight = fontWeight,
                fontFamily = fontFamily,
                letterSpacing = letterSpacing,
                textDecoration = textDecoration,
                textAlign = textAlign,
                lineHeight = lineHeight,
                overflow = TextOverflow.Clip,
                softWrap = true,
                maxLines = maxLines,
                inlineContent = inlineContent,
                onTextLayout = onTextLayout,
                style = combinedTextStyle,
            )
        }
    }
}

@OptIn(InternalFoundationTextApi::class)
@Composable
private fun BoxWithConstraintsScope.shouldShrink(
    text: AnnotatedString,
    textStyle: TextStyle,
    maxLines: Int,
): Boolean {
    val textDelegate = TextDelegate(
        text = text,
        style = textStyle,
        maxLines = maxLines,
        softWrap = true,
        overflow = TextOverflow.Clip,
        density = LocalDensity.current,
        fontFamilyResolver = LocalFontFamilyResolver.current,
    )

    val textLayoutResult = textDelegate.layout(
        constraints,
        LocalLayoutDirection.current,
    )

    return textLayoutResult.hasVisualOverflow
}