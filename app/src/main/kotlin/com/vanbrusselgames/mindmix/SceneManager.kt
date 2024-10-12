package com.vanbrusselgames.mindmix

import androidx.navigation.NavHostController
import com.vanbrusselgames.mindmix.games.game2048.Game2048
import com.vanbrusselgames.mindmix.games.minesweeper.MinesweeperManager
import com.vanbrusselgames.mindmix.games.solitaire.SolitaireManager
import com.vanbrusselgames.mindmix.games.sudoku.SudokuManager

class SceneManager {
    enum class Scene {
        GAME2048, MENU, MINESWEEPER, SOLITAIRE, SUDOKU
    }

    companion object {
        lateinit var navController: NavHostController
        val scenes = mapOf(
            0 to Scene.SUDOKU,
            1 to Scene.SOLITAIRE,
            2 to Scene.MINESWEEPER,
            3 to Scene.MENU,
            4 to Scene.GAME2048
        )

        var currentScene = Scene.MENU

        fun loadScene(scene: Scene) {
            currentScene = scene
            var route = scene.name.lowercase()
            when (scene) {
                Scene.MINESWEEPER -> MinesweeperManager.loadPuzzle()
                Scene.SOLITAIRE -> SolitaireManager.loadPuzzle()
                Scene.SUDOKU -> SudokuManager.startPuzzle()
                Scene.MENU -> {}
                Scene.GAME2048 -> route = Game2048.NAV_ROUTE
            }
            navController.navigate(route)
        }
    }
}