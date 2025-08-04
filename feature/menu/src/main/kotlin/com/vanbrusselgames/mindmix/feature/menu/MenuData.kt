package com.vanbrusselgames.mindmix.feature.menu

import kotlinx.serialization.Serializable

@Serializable
data class MenuData(
    val selectedGame: Int,
    val coins: Int/*, val selectedGameModeIndices: Map<GameScene, Int>*/
)