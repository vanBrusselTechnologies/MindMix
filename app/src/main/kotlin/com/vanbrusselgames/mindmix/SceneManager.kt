package com.vanbrusselgames.mindmix

import androidx.navigation.NavHostController

class SceneManager {
    enum class Scene {
        MENU, MINESWEEPER, SOLITAIRE, SUDOKU
    }

    companion object {
        lateinit var navController: NavHostController
        val scenes =
            mapOf(0 to Scene.SUDOKU, 1 to Scene.SOLITAIRE, 2 to Scene.MINESWEEPER, 3 to Scene.MENU)

        fun loadScene(scene: Scene) {
            openScene(scene)
        }

        private fun openScene(scene: Scene) {
            navController.navigate(scene.name.lowercase())
        }
    }
}