package com.vanbrusselgames.mindmix.games.game2048

import androidx.compose.runtime.mutableStateOf
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.lerp
import com.google.common.math.IntMath.pow
import kotlin.math.log2

data class GridCell2048(val id: Int, private var _value: Int) {
    private val color0 = Color(255, 255, 222)
    private val color11 = Color.Red
    private val color22 = Color(0, 0, 111)
    private val color33 = Color.Black
    val background = mutableStateOf<Color>(color0)

    var value
        get() = _value
        set(value) {
            this._value = value
            updateBackground()
        }

    private fun updateBackground() {
        if (_value == 0) {
            background.value = Color.Transparent
            return
        }
        if (_value <= pow(2, 11)) {
            background.value = lerp(color0, color11, log2(_value.toFloat()) / 11f)
            return
        }
        if (_value <= pow(2, 22)) {
            background.value = lerp(color11, color22, log2(_value - 11f) / 11f)
            return
        }
        if (_value <= pow(2, 33)) {
            background.value = lerp(color22, color33, log2(_value - 22f) / 11f)
            return
        }
        background.value = Color.Black
    }
}