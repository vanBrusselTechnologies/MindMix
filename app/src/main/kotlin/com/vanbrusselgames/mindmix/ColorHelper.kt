package com.vanbrusselgames.mindmix

import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.luminance

class ColorHelper {
    companion object{
        fun getLightestColor(color1: Color, color2: Color): Color {
            val color1Luminance = color1.luminance()
            val color2Luminance = color2.luminance()
            return if (color1Luminance > color2Luminance) color1 else color2
        }
        fun getDarkestColor(color1: Color, color2: Color): Color {
            val color1Luminance = color1.luminance()
            val color2Luminance = color2.luminance()
            return if (color1Luminance < color2Luminance) color1 else color2
        }
    }
}