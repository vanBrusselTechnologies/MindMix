package com.vanbrusselgames.mindmix

import androidx.navigation.NavHostController
import com.vanbrusselgames.mindmix.games.minesweeper.MinesweeperManager
import com.vanbrusselgames.mindmix.games.solitaire.SolitaireManager
import com.vanbrusselgames.mindmix.games.sudoku.SudokuManager

class SceneManager {
    enum class Scene {
        MENU, MINESWEEPER, SOLITAIRE, SUDOKU
    }

    companion object {
        lateinit var navController: NavHostController
        val scenes =
            mapOf(0 to Scene.SUDOKU, 1 to Scene.SOLITAIRE, 2 to Scene.MINESWEEPER, 3 to Scene.MENU)

        var currentScene = Scene.MENU

        fun loadScene(scene: Scene) {
            when (scene) {
                Scene.MINESWEEPER -> MinesweeperManager.loadPuzzle()
                Scene.SOLITAIRE -> SolitaireManager.loadPuzzle()
                Scene.SUDOKU -> SudokuManager.startPuzzle()
                Scene.MENU -> {}
            }
            openScene(scene)
        }

        private fun openScene(scene: Scene) {
            currentScene = scene
            navController.navigate(scene.name.lowercase())
        }
    }
}