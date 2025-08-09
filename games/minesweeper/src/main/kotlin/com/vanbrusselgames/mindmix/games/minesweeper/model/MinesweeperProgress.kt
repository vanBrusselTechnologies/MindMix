package com.vanbrusselgames.mindmix.games.minesweeper.model

import com.vanbrusselgames.mindmix.core.utils.constants.Difficulty
import kotlinx.serialization.Serializable

@Serializable
data class MinesweeperProgress(
    val difficulty: Difficulty = Difficulty.MEDIUM,
    val input: List<Int> = listOf(),
    val mines: List<Int> = listOf()
)