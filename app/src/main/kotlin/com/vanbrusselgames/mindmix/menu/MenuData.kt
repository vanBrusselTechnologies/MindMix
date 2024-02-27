package com.vanbrusselgames.mindmix.menu

import com.vanbrusselgames.mindmix.SceneManager
import kotlinx.serialization.Serializable

@Serializable
data class MenuData(val selectedGame: SceneManager.Scene, val coins: Int/*, val selectedGameModeIndices: List<Int>*/)