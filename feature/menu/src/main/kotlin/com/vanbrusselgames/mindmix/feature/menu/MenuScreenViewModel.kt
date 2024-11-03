package com.vanbrusselgames.mindmix.feature.menu

import androidx.compose.runtime.mutableStateOf
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.common.BaseScreenViewModel
import com.vanbrusselgames.mindmix.core.designsystem.theme.SelectedTheme
import com.vanbrusselgames.mindmix.core.navigation.SceneManager.Scene
import com.vanbrusselgames.mindmix.games.game2048.Game2048
import com.vanbrusselgames.mindmix.games.game2048.navigation.navigateTo2048
import com.vanbrusselgames.mindmix.games.minesweeper.Minesweeper
import com.vanbrusselgames.mindmix.games.minesweeper.navigation.navigateToMinesweeper
import com.vanbrusselgames.mindmix.games.solitaire.Solitaire
import com.vanbrusselgames.mindmix.games.solitaire.navigation.navigateToSolitaire
import com.vanbrusselgames.mindmix.games.sudoku.Sudoku
import com.vanbrusselgames.mindmix.games.sudoku.navigation.navigateToSudoku
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json

class MenuScreenViewModel : BaseScreenViewModel() {
    override val nameResId: Int = Menu.NAME_RES_ID

    override val isMenu = true

    companion object Instance {
        const val ADD_DUPLICATES = true
    }

    val theme = mutableStateOf(SelectedTheme.System)
    val games = mapOf(
        Sudoku.GAME_ID to Scene.SUDOKU,
        Solitaire.GAME_ID to Scene.SOLITAIRE,
        Minesweeper.GAME_ID to Scene.MINESWEEPER,
        Game2048.GAME_ID to Scene.GAME2048
    )
    var selectedGame = Scene.MINESWEEPER
    var settingsGame = Scene.MENU
    var coins = 0

    val wheelModel = GameWheel(this, games.size)

    /*private var selectedGameModeIndices = mapOf(
            Scene.MINESWEEPER to 0,
            Scene.SOLITAIRE to 0,
            Scene.SUDOKU to 0,
        )*/

    fun loadFromFile(data: MenuData) {
        selectedGame = data.selectedGame
        wheelModel.selectedId = games.filter { g -> g.value == selectedGame }.keys.first()
        wheelModel.rotationAngle = wheelModel.selectedId * wheelModel.angleStep
        coins = data.coins
        //selectedGameModeIndices = data.selectedGameModeIndices
    }

    fun saveToFile(): String {
        return Json.encodeToString(
            MenuData(selectedGame, coins/*, selectedGameModeIndices*/)
        )
    }

    //fun getSelectedGameModeIndex(gameIndex: Int = -1): Int {
    //    val selectedGameIndex = if (gameIndex == -1) selectedGameIndex else gameIndex
    //    return selectedGameModeIndices[selectedGameIndex]
    //}

    //fun setSelectedGameModeIndex(gameModeIndex: Int, gameIndex: Int = -1) {
    //    val selectedGameIndex = if (gameIndex == -1) selectedGameIndex else gameIndex
    //    selectedGameModeIndices[selectedGameIndex] = gameModeIndex
    //}

    fun navigateToSelectedGame(navController: NavController){
        when(selectedGame){
            Scene.GAME2048 -> navController.navigateTo2048()
            Scene.MENU -> {}
            Scene.MINESWEEPER -> navController.navigateToMinesweeper()
            Scene.SOLITAIRE -> navController.navigateToSolitaire()
            Scene.SUDOKU -> navController.navigateToSudoku()
        }
    }
}