package com.vanbrusselgames.mindmix

import com.vanbrusselgames.mindmix.core.data.DataManager
import com.vanbrusselgames.mindmix.core.navigation.SceneManager.Scene

val saveSceneData: (dataManager: DataManager) -> Unit = { dataManager ->
    Scene.entries.forEach {
        when (it) {
            Scene.SUDOKU -> dataManager.saveScene(it, sudoku.viewModel.saveToFile())
            Scene.SOLITAIRE -> dataManager.saveScene(it, solitaire.viewModel.saveToFile())
            Scene.MINESWEEPER -> dataManager.saveScene(it, minesweeper.viewModel.saveToFile())
            Scene.MENU -> dataManager.saveScene(it, menu.viewModel.saveToFile())
            Scene.GAME2048 -> dataManager.saveScene(it, game2048.viewModel.saveToFile())
        }
    }
}