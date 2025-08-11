package com.vanbrusselgames.mindmix.games.solitaire.model

import com.vanbrusselgames.mindmix.core.utils.constants.StringEnum
import com.vanbrusselgames.mindmix.games.solitaire.R

enum class CardVisualType : StringEnum {
    DETAILED, SIMPLE;

    override fun getStringResource(): Int {
        return when (this) {
            DETAILED -> R.string.card_type_detailed
            SIMPLE -> R.string.card_type_simple
        }
    }
}