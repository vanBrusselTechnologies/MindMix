package com.vanbrusselgames.mindmix.feature.menu.viewmodel

import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.common.BaseScreenViewModel
import com.vanbrusselgames.mindmix.core.model.GameScene
import com.vanbrusselgames.mindmix.core.model.SceneRegistry
import com.vanbrusselgames.mindmix.feature.menu.model.GameWheel
import com.vanbrusselgames.mindmix.games.game2048.navigation.navigateToGame2048
import com.vanbrusselgames.mindmix.games.minesweeper.navigation.navigateToMinesweeper
import com.vanbrusselgames.mindmix.games.solitaire.navigation.navigateToSolitaire
import com.vanbrusselgames.mindmix.games.sudoku.navigation.navigateToSudoku
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow

class MockMenuScreenViewModel : BaseScreenViewModel(), IMenuScreenViewModel {
    override val isMenu = true

    override val preferencesLoaded = MutableStateFlow(false).asStateFlow()

    override val games = mapOf(
        SceneRegistry.Sudoku.gameId to SceneRegistry.Sudoku,
        SceneRegistry.Solitaire.gameId to SceneRegistry.Solitaire,
        SceneRegistry.Minesweeper.gameId to SceneRegistry.Minesweeper,
        SceneRegistry.Game2048.gameId to SceneRegistry.Game2048
    )

    override var selectedGame: GameScene = SceneRegistry.Game2048

    override val wheelModel = GameWheel(this, games.size)

    /*private var selectedGameModeIndices = mapOf(
            SceneRegistry.Minesweeper to 0,
            SceneRegistry.Solitaire to 0,
            SceneRegistry.Sudoku to 0,
        )*/

    //fun getSelectedGameModeIndex(gameIndex: Int = -1): Int {
    //    val selectedGameIndex = if (gameIndex == -1) selectedGameIndex else gameIndex
    //    return selectedGameModeIndices[selectedGameIndex]
    //}

    //fun setSelectedGameModeIndex(gameModeIndex: Int, gameIndex: Int = -1) {
    //    val selectedGameIndex = if (gameIndex == -1) selectedGameIndex else gameIndex
    //    selectedGameModeIndices[selectedGameIndex] = gameModeIndex
    //}

    override fun navigateToSelectedGame(navController: NavController) {
        when (selectedGame) {
            SceneRegistry.Game2048 -> navController.navigateToGame2048()
            SceneRegistry.Minesweeper -> navController.navigateToMinesweeper()
            SceneRegistry.Solitaire -> navController.navigateToSolitaire()
            SceneRegistry.Sudoku -> navController.navigateToSudoku()
        }
    }
}