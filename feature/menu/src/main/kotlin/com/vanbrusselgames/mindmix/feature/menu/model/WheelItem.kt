package com.vanbrusselgames.mindmix.feature.menu.model

import androidx.compose.runtime.mutableStateOf
import com.vanbrusselgames.mindmix.core.model.GameScene
import com.vanbrusselgames.mindmix.core.model.SceneRegistry
import com.vanbrusselgames.mindmix.games.game2048.model.Game2048
import com.vanbrusselgames.mindmix.games.minesweeper.model.Minesweeper
import com.vanbrusselgames.mindmix.games.solitaire.model.Solitaire
import com.vanbrusselgames.mindmix.games.sudoku.model.Sudoku

data class WheelItem(val game: GameScene) {
    var isSelected = mutableStateOf(false)
    var growthFactor = 0f
    var offsetY = 0f
    var angle = 0f
    var radius = 0f

    val title = when (game) {
        SceneRegistry.Minesweeper -> Minesweeper.NAME_RES_ID
        SceneRegistry.Solitaire -> Solitaire.NAME_RES_ID
        SceneRegistry.Sudoku -> Sudoku.NAME_RES_ID
        SceneRegistry.Game2048 -> Game2048.NAME_RES_ID
        else -> -1
    }

    val image = when (game) {
        SceneRegistry.Minesweeper -> Minesweeper.IMAGE_RES_ID
        SceneRegistry.Solitaire -> Solitaire.IMAGE_RES_ID
        SceneRegistry.Sudoku -> Sudoku.IMAGE_RES_ID
        SceneRegistry.Game2048 -> Game2048.IMAGE_RES_ID
        else -> -1
    }
}