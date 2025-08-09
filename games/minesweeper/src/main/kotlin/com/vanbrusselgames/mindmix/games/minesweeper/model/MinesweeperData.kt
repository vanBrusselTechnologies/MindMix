package com.vanbrusselgames.mindmix.games.minesweeper.model

import kotlinx.serialization.Serializable

@Serializable
data class MinesweeperData(val progress: List<MinesweeperProgress> = listOf())