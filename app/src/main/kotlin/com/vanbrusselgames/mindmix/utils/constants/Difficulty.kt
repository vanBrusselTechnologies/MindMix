package com.vanbrusselgames.mindmix.utils.constants

import com.vanbrusselgames.mindmix.R

enum class Difficulty {
    EASY, MEDIUM, HARD, EXPERT, MASTER;

    fun getStringResource(): Int {
        return when (this) {
            EASY -> R.string.difficulty_easy
            MEDIUM -> R.string.difficulty_medium
            HARD -> R.string.difficulty_hard
            EXPERT -> R.string.difficulty_expert
            MASTER -> R.string.difficulty_master
        }
    }
}