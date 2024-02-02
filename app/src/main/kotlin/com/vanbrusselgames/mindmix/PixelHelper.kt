package com.vanbrusselgames.mindmix

import android.content.res.Resources
import android.os.Build
import android.util.TypedValue
import androidx.compose.ui.unit.TextUnit
import androidx.compose.ui.unit.sp

class PixelHelper {
    companion object {
        private lateinit var res: Resources
        fun setResources(resources: Resources) {
            res = resources
        }

        fun pxToSp(px: Int): TextUnit {
            return if (Build.VERSION.SDK_INT < Build.VERSION_CODES.UPSIDE_DOWN_CAKE) {
                (px / res.displayMetrics.scaledDensity).sp
            } else {
                TypedValue.deriveDimension(
                    TypedValue.COMPLEX_UNIT_SP, px.toFloat(), res.displayMetrics
                ).sp
            }
        }
    }
}