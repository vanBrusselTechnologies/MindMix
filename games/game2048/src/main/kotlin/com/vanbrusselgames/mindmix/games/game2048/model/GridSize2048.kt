package com.vanbrusselgames.mindmix.games.game2048.model

import com.vanbrusselgames.mindmix.core.utils.constants.StringEnum
import com.vanbrusselgames.mindmix.games.game2048.R

enum class GridSize2048 : StringEnum {
    THREE, FOUR, FIVE, SIX, SEVEN;

    override fun getStringResource(): Int {
        return when (this) {
            THREE -> R.string.game_2048_size_3x3
            FOUR -> R.string.game_2048_size_4x4
            FIVE -> R.string.game_2048_size_5x5
            SIX -> R.string.game_2048_size_6x6
            SEVEN -> R.string.game_2048_size_7x7
        }
    }

    fun getSize(): Int {
        return when (this) {
            THREE -> 3
            FOUR -> 4
            FIVE -> 5
            SIX -> 6
            SEVEN -> 7
        }
    }

    fun getMaxCellCount(): Int {
        return this.getSize() * this.getSize()
    }
}