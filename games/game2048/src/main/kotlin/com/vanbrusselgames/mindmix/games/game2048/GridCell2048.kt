package com.vanbrusselgames.mindmix.games.game2048

import androidx.compose.runtime.mutableStateOf
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.lerp
import androidx.compose.ui.util.fastRoundToInt
import kotlin.math.log2

data class GridCell2048(val id: Int, private var _value: Long) {
    private val color0 = Color(255, 255, 222)
    private val color11 = Color.Red
    private val color22 = Color.Blue
    private val color33 = Color.Black
    val background = mutableStateOf<Color>(Color.Transparent)

    override fun equals(other: Any?): Boolean {
        if (this === other) return true
        if (other !is GridCell2048) return false

        if (id == other.id) return true
        return false
    }

    override fun hashCode(): Int {
        return id
    }

    init {
        updateBackground()
    }

    var value
        get() = _value
        set(value) {
            if (_value == value) return
            _value = value
            updateBackground()
        }

    private fun updateBackground() {
        if (_value == 0L) {
            background.value = Color.Transparent
            return
        }
        val exponent = log2(_value.toDouble()).fastRoundToInt()
        background.value = when {
            (exponent <= 11) -> lerp(color0, color11, exponent / 11f)
            (exponent <= 22) -> lerp(color11, color22, (exponent - 11f) / 11f)
            (exponent <= 33) -> lerp(color22, color33, (exponent - 22f) / 11f)
            else -> Color.Black
        }
    }
}