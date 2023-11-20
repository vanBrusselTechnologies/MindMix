package com.vanbrusselgames.mindmix

import android.util.Log
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import com.vanbrusselgames.mindmix.menu.MenuLayout
import com.vanbrusselgames.mindmix.minesweeper.MinesweeperLayout
import com.vanbrusselgames.mindmix.minesweeper.MinesweeperManager
import com.vanbrusselgames.mindmix.solitaire.SolitaireLayout
import com.vanbrusselgames.mindmix.solitaire.SolitaireManager
import com.vanbrusselgames.mindmix.sudoku.SudokuLayout
import com.vanbrusselgames.mindmix.sudoku.SudokuManager

class SceneManager {
    enum class Scene {
        MAIN, MENU, MINESWEEPER, SOLITAIRE, SUDOKU
    }

    companion object {
        private var _currentScene: Scene by mutableStateOf(Scene.MAIN)

        @Composable
        private fun LoadScene(scene: Scene) {
            Log.d("MindMix", "scene:  $scene")
            when (scene) {
                Scene.MAIN -> MenuLayout().BaseScene()
                Scene.MENU -> MenuLayout().BaseScene()

                Scene.SOLITAIRE -> {
                    SolitaireManager.loadPuzzle()
                    SolitaireLayout().BaseScene()
                }

                Scene.SUDOKU -> {
                    SudokuManager.loadPuzzle()
                    SudokuLayout().BaseScene()
                }

                Scene.MINESWEEPER -> {
                    MinesweeperManager.loadPuzzle()
                    MinesweeperLayout().BaseScene()
                }
            }
        }


        @Composable
        fun OpenScene() {
            LoadScene(_currentScene)
            //If needed: LoadingScreen
            //ShowScene
        }

        fun setCurrentScene(scene: Scene) {
            if (_currentScene != scene) _currentScene = scene
        }

        fun setCurrentScene(sceneIndex: Int) {
            if (sceneIndex < 0) return
            val index = sceneIndex % Scene.values().size
            _currentScene = enumValues<Scene>().find { it.ordinal == index } ?: return
        }
    }
}