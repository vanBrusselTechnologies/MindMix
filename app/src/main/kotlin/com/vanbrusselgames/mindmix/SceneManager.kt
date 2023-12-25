package com.vanbrusselgames.mindmix

import android.util.Log
import androidx.navigation.NavHostController

class SceneManager {
    enum class Scene {
        MAIN, MENU, MINESWEEPER, SOLITAIRE, SUDOKU
    }

    companion object {
        lateinit var navController: NavHostController

        fun loadScene(sceneIndex: Int){
            if (sceneIndex < 0) return
            val index = sceneIndex % Scene.entries.size
            val scene = enumValues<Scene>().find { it.ordinal == index } ?: return
            loadScene(scene)
        }

        fun loadScene(scene: Scene) {
            Log.d("MindMix", "scene:  ${scene.name.lowercase()}")
            openScene(scene)
        }

        private fun openScene(scene: Scene) {
            navController.navigate(scene.name.lowercase())
        }
    }
}