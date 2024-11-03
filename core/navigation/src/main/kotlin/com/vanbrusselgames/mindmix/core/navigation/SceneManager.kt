package com.vanbrusselgames.mindmix.core.navigation

import androidx.compose.runtime.mutableStateOf
import androidx.navigation.NavController

class SceneManager {
    enum class Scene {
        GAME2048, MENU, MINESWEEPER, SOLITAIRE, SUDOKU
    }

    companion object {
        val scenes = mapOf(
            0 to Scene.SUDOKU,
            1 to Scene.SOLITAIRE,
            2 to Scene.MINESWEEPER,
            3 to Scene.MENU,
            4 to Scene.GAME2048
        )
        var currentScene = Scene.MENU
        val dialogActiveState = mutableStateOf(false)
    }

    val onDestinationChange =
        NavController.OnDestinationChangedListener { navController, destination, bundle ->
            dialogActiveState.value = destination.navigatorName === "dialog"
        }
}