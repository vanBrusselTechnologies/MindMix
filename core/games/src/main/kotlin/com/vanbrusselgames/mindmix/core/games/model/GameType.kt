package com.vanbrusselgames.mindmix.core.games.model

import androidx.annotation.DrawableRes
import com.vanbrusselgames.mindmix.core.games.R

enum class GameType(@DrawableRes val iconRes: Int) {
    GAME2048(R.drawable.game_icon_2048_4x4),
    MINESWEEPER(R.drawable.game_icon_minesweeper),
    SUDOKU(R.drawable.game_icon_sudoku),
    SOLITAIRE(R.drawable.game_icon_solitaire),
}