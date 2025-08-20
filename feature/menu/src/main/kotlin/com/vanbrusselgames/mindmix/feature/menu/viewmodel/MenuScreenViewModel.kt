package com.vanbrusselgames.mindmix.feature.menu.viewmodel

import androidx.lifecycle.viewModelScope
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.common.BaseScreenViewModel
import com.vanbrusselgames.mindmix.core.logging.Logger
import com.vanbrusselgames.mindmix.core.model.GameScene
import com.vanbrusselgames.mindmix.core.model.SceneRegistry
import com.vanbrusselgames.mindmix.feature.menu.data.MenuRepository
import com.vanbrusselgames.mindmix.feature.menu.data.preferences.MenuPreferences
import com.vanbrusselgames.mindmix.feature.menu.data.preferences.MenuPreferencesRepository
import com.vanbrusselgames.mindmix.feature.menu.model.GameWheel
import com.vanbrusselgames.mindmix.games.game2048.navigation.navigateToGame2048
import com.vanbrusselgames.mindmix.games.minesweeper.navigation.navigateToMinesweeper
import com.vanbrusselgames.mindmix.games.solitaire.navigation.navigateToSolitaire
import com.vanbrusselgames.mindmix.games.sudoku.navigation.navigateToSudoku
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.flow.onStart
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class MenuScreenViewModel @Inject constructor(
    private val menuRepository: MenuRepository,
    private val prefsRepository: MenuPreferencesRepository
) : BaseScreenViewModel(), IMenuScreenViewModel {
    override val isMenu = true

    private val _preferencesLoaded = MutableStateFlow(false)
    override val preferencesLoaded = _preferencesLoaded.onStart { loadPreferences() }
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000L), false)

    override val games = mapOf(
        SceneRegistry.Sudoku.gameId to SceneRegistry.Sudoku,
        SceneRegistry.Solitaire.gameId to SceneRegistry.Solitaire,
        SceneRegistry.Minesweeper.gameId to SceneRegistry.Minesweeper,
        SceneRegistry.Game2048.gameId to SceneRegistry.Game2048
    )

    override var selectedGame: GameScene = SceneRegistry.Game2048

    override val wheelModel = GameWheel(this, games.size)

    private suspend fun loadPreferences() {
        applyPreferences(prefsRepository.getPreferences().first())
    }

    private suspend fun applyPreferences(preferences: MenuPreferences) {
        Logger.d("[menu] applyPreferences")
        selectedGame =
            SceneRegistry.allScenes.first { it.sceneId == preferences.selectedGame } as GameScene
        //selectedGameModeIndices = data.selectedGameModeIndices

        wheelModel.selectedId = games.filter { it.value == selectedGame }.keys.first()
        wheelModel.rotationAngle = wheelModel.selectedId * wheelModel.angleStep

        _preferencesLoaded.emit(true)
    }

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
        viewModelScope.launch {
            prefsRepository.savePreferences(MenuPreferences(selectedGame.sceneId))
        }
        when (selectedGame) {
            SceneRegistry.Game2048 -> navController.navigateToGame2048()
            SceneRegistry.Minesweeper -> navController.navigateToMinesweeper()
            SceneRegistry.Solitaire -> navController.navigateToSolitaire()
            SceneRegistry.Sudoku -> navController.navigateToSudoku()
        }
    }
}