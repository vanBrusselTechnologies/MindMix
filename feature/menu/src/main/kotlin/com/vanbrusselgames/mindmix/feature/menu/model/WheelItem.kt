package com.vanbrusselgames.mindmix.feature.menu.model

import androidx.compose.runtime.mutableStateOf
import com.vanbrusselgames.mindmix.core.games.model.GameType
import com.vanbrusselgames.mindmix.core.model.GameScene
import com.vanbrusselgames.mindmix.core.model.SceneRegistry
import com.vanbrusselgames.mindmix.games.game2048.model.Game2048
import com.vanbrusselgames.mindmix.games.minesweeper.model.Minesweeper
import com.vanbrusselgames.mindmix.games.solitaire.model.Solitaire
import com.vanbrusselgames.mindmix.games.sudoku.model.Sudoku

data class WheelItem(val game: GameScene, val radius: Float, val angle: Float) {
    val isSelected = mutableStateOf(false)
    val visible = mutableStateOf(true)

    val title = when (game) {
        SceneRegistry.Game2048 -> Game2048.NAME_RES_ID
        SceneRegistry.Minesweeper -> Minesweeper.NAME_RES_ID
        SceneRegistry.Solitaire -> Solitaire.NAME_RES_ID
        SceneRegistry.Sudoku -> Sudoku.NAME_RES_ID
        else -> -1
    }

    val gameType = when (game) {
        SceneRegistry.Game2048 -> GameType.GAME2048
        SceneRegistry.Minesweeper -> GameType.MINESWEEPER
        SceneRegistry.Solitaire -> GameType.SOLITAIRE
        SceneRegistry.Sudoku -> GameType.SUDOKU
        else -> GameType.GAME2048
    }
}