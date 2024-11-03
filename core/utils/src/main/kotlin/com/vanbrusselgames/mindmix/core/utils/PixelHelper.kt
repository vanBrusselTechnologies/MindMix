package com.vanbrusselgames.mindmix.core.utils

import android.content.res.Resources
import android.os.Build
import android.util.TypedValue
import androidx.compose.ui.unit.TextUnit
import androidx.compose.ui.unit.sp

class PixelHelper {
    companion object {
        fun pxToSp(res: Resources, px: Int): TextUnit {
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