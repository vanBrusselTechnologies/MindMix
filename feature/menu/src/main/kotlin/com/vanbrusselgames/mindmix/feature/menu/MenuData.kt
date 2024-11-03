package com.vanbrusselgames.mindmix.feature.menu

import com.vanbrusselgames.mindmix.core.navigation.SceneManager.Scene
import kotlinx.serialization.Serializable

@Serializable
data class MenuData(
    val selectedGame: Scene,
    val coins: Int/*, val selectedGameModeIndices: Map<SceneManager.Scene, Int>*/
)