package com.vanbrusselgames.mindmix.menu

import androidx.compose.runtime.mutableStateOf
import com.vanbrusselgames.mindmix.Logger
import com.vanbrusselgames.mindmix.SceneManager
import com.vanbrusselgames.mindmix.ui.theme.SelectedTheme
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json

class MenuManager {
    companion object Instance {
        val theme = mutableStateOf(SelectedTheme.System)
        val games = SceneManager.scenes.filter { e -> e.value != SceneManager.Scene.MENU }
        const val ADD_DUPLICATES = true
        var selectedGame = SceneManager.Scene.MINESWEEPER
        var settingsGame = mutableStateOf(SceneManager.Scene.MENU)
        var coins = 0

        val wheelModel = GameWheel(games.size)
        /*private var selectedGameModeIndices = mapOf(
            SceneManager.Scene.MINESWEEPER to 0,
            SceneManager.Scene.SOLITAIRE to 0,
            SceneManager.Scene.SUDOKU to 0,
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
    }
}