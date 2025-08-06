package com.vanbrusselgames.mindmix.feature.menu

import androidx.compose.runtime.mutableStateOf
import androidx.datastore.preferences.core.Preferences
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.common.BaseScreenViewModel
import com.vanbrusselgames.mindmix.core.common.coins
import com.vanbrusselgames.mindmix.core.designsystem.theme.SelectedTheme
import com.vanbrusselgames.mindmix.core.model.GameScene
import com.vanbrusselgames.mindmix.core.model.Scene
import com.vanbrusselgames.mindmix.core.model.SceneRegistry
import com.vanbrusselgames.mindmix.feature.menu.data.PREF_KEY_THEME
import com.vanbrusselgames.mindmix.games.game2048.navigation.navigateToGame2048
import com.vanbrusselgames.mindmix.games.minesweeper.navigation.navigateToMinesweeper
import com.vanbrusselgames.mindmix.games.solitaire.navigation.navigateToSolitaire
import com.vanbrusselgames.mindmix.games.sudoku.navigation.navigateToSudoku
import kotlinx.serialization.json.Json

//@HiltViewModel -- only when using Hilt @Inject
class MenuScreenViewModel : BaseScreenViewModel() {
    override val nameResId: Int = Menu.NAME_RES_ID

    override val isMenu = true

    companion object Instance {
        const val ADD_DUPLICATES = true
    }

    val theme = mutableStateOf(SelectedTheme.System)
    val games = mapOf(
        SceneRegistry.Sudoku.gameId to SceneRegistry.Sudoku,
        SceneRegistry.Solitaire.gameId to SceneRegistry.Solitaire,
        SceneRegistry.Minesweeper.gameId to SceneRegistry.Minesweeper,
        SceneRegistry.Game2048.gameId to SceneRegistry.Game2048
    )
    var selectedGame: GameScene = SceneRegistry.Minesweeper
    var settingsGame: Scene = SceneRegistry.Menu

    val wheelModel = GameWheel(this, games.size)

    /*private var selectedGameModeIndices = mapOf(
            SceneRegistry.Minesweeper to 0,
            SceneRegistry.Solitaire to 0,
            SceneRegistry.Sudoku to 0,
        )*/

    fun onLoadPreferences(preferences: Preferences) {
        if (preferences[PREF_KEY_THEME] != null) {
            theme.value = SelectedTheme.entries.first { it.ordinal == preferences[PREF_KEY_THEME] }
        }
    }

    fun loadFromFile(data: MenuData) {
        selectedGame =
            SceneRegistry.allScenes.first { it.sceneId == data.selectedGame } as GameScene
        wheelModel.selectedId = games.filter { it.value == selectedGame }.keys.first()
        wheelModel.rotationAngle = wheelModel.selectedId * wheelModel.angleStep
        coins = data.coins
        //selectedGameModeIndices = data.selectedGameModeIndices
    }

    fun saveToFile(): String {
        return Json.encodeToString(MenuData(selectedGame.sceneId, coins/*, selectedGameModeIndices*/))
    }

    //fun getSelectedGameModeIndex(gameIndex: Int = -1): Int {
    //    val selectedGameIndex = if (gameIndex == -1) selectedGameIndex else gameIndex
    //    return selectedGameModeIndices[selectedGameIndex]
    //}

    //fun setSelectedGameModeIndex(gameModeIndex: Int, gameIndex: Int = -1) {
    //    val selectedGameIndex = if (gameIndex == -1) selectedGameIndex else gameIndex
    //    selectedGameModeIndices[selectedGameIndex] = gameModeIndex
    //}

    fun navigateToSelectedGame(navController: NavController) {
        when (selectedGame) {
            SceneRegistry.Game2048 -> navController.navigateToGame2048()
            SceneRegistry.Minesweeper -> navController.navigateToMinesweeper()
            SceneRegistry.Solitaire -> navController.navigateToSolitaire()
            SceneRegistry.Sudoku -> navController.navigateToSudoku()
        }
    }
}