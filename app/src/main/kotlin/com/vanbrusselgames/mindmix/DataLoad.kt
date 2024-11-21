package com.vanbrusselgames.mindmix

import androidx.datastore.preferences.core.Preferences
import com.vanbrusselgames.mindmix.core.navigation.SceneManager.Scene
import com.vanbrusselgames.mindmix.feature.menu.Menu
import com.vanbrusselgames.mindmix.feature.menu.MenuData
import com.vanbrusselgames.mindmix.games.game2048.Game2048
import com.vanbrusselgames.mindmix.games.game2048.Game2048Data
import com.vanbrusselgames.mindmix.games.minesweeper.Minesweeper
import com.vanbrusselgames.mindmix.games.minesweeper.MinesweeperData
import com.vanbrusselgames.mindmix.games.solitaire.Solitaire
import com.vanbrusselgames.mindmix.games.solitaire.SolitaireData
import com.vanbrusselgames.mindmix.games.sudoku.PuzzleType
import com.vanbrusselgames.mindmix.games.sudoku.Sudoku
import com.vanbrusselgames.mindmix.games.sudoku.SudokuData
import com.vanbrusselgames.mindmix.games.sudoku.SudokuLoader
import kotlinx.serialization.json.Json

val menu = Menu()
val game2048 = Game2048()
val minesweeper = Minesweeper()
val solitaire = Solitaire()
val sudoku = Sudoku()

val loadDataForScene: (jsonParser: Json, Scene, String) -> Unit = { jsonParser, saveScene, json ->
    when (saveScene) {
        Scene.SUDOKU -> {
            val data = jsonParser.decodeFromString<SudokuData>(json)
            sudoku.viewModel.loadFromFile(data)
        }

        Scene.SOLITAIRE -> {
            val data = jsonParser.decodeFromString<SolitaireData>(json)
            solitaire.viewModel.loadFromFile(data)
        }

        Scene.MINESWEEPER -> {
            val data = jsonParser.decodeFromString<MinesweeperData>(json)
            minesweeper.viewModel.loadFromFile(data)
        }

        Scene.MENU -> {
            val data = jsonParser.decodeFromString<MenuData>(json)
            menu.viewModel.loadFromFile(data)
        }

        Scene.GAME2048 -> {
            val data = jsonParser.decodeFromString<Game2048Data>(json)
            game2048.viewModel.loadFromFile(data)
        }
    }
}

val onLoadPreferences: (preferences: Preferences) -> Unit = { preferences ->
    sudoku.viewModel.onLoadPreferences(preferences)
    solitaire.viewModel.onLoadPreferences(preferences)
    minesweeper.viewModel.onLoadPreferences(preferences)
    menu.viewModel.onLoadPreferences(preferences)
    game2048.viewModel.onLoadPreferences(preferences)
}

val gameLoad: suspend (loadFromFile: Boolean) -> Unit = { loadFromFile ->
    try {
        SudokuLoader.requestPuzzles(
            sudoku.viewModel, PuzzleType.Classic, loadFromFile
        )
    } catch (_: Exception) {
    }
}