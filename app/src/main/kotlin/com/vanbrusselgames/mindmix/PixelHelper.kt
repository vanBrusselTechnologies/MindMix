package com.vanbrusselgames.mindmix

import android.content.res.Resources
import androidx.compose.ui.unit.TextUnit
import androidx.compose.ui.unit.sp

class PixelHelper {
    companion object{
        private lateinit var res: Resources
        fun setResources(resources: Resources){
            res = resources
        }
        fun pxToSp(px: Float): TextUnit {
            return (px / res.displayMetrics.scaledDensity).sp
        }
        fun pxToSp(px: Int): TextUnit {
            return (px / res.displayMetrics.scaledDensity).sp
        }
    }
}