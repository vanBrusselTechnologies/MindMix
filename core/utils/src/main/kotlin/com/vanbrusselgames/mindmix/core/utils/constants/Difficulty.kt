package com.vanbrusselgames.mindmix.core.utils.constants

import com.vanbrusselgames.mindmix.core.utils.R

enum class Difficulty : StringEnum {
    EASY, MEDIUM, HARD, EXPERT, MASTER;

    override fun getStringResource(): Int {
        return when (this) {
            EASY -> R.string.difficulty_easy
            MEDIUM -> R.string.difficulty_medium
            HARD -> R.string.difficulty_hard
            EXPERT -> R.string.difficulty_expert
            MASTER -> R.string.difficulty_master
        }
    }
}