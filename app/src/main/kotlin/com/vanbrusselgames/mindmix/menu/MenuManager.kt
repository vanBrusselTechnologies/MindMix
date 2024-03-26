package com.vanbrusselgames.mindmix.menu

import androidx.compose.runtime.mutableStateOf
import com.vanbrusselgames.mindmix.SceneManager
import com.vanbrusselgames.mindmix.ui.theme.SelectedTheme
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json

class MenuManager {
    companion object Instance {
        val theme = mutableStateOf(SelectedTheme.System)
        val games = SceneManager.scenes.filter { e -> e.value != SceneManager.Scene.MENU }
        const val GAME_COUNT = 3
        const val ADD_DUPLICATES = true
        var selectedGame = SceneManager.Scene.MINESWEEPER
        var coins = 0
        //private var selectedGameModeIndices = intArrayOf(0, 0, 0)

        fun loadFromFile(data: MenuData) {
            selectedGame = data.selectedGame
            coins = data.coins
            //selectedGameModeIndices = data.selectedGameModeIndices.toIntArray()
        }

        fun saveToFile(): String {
            return Json.encodeToString(
                MenuData(selectedGame, coins/*, selectedGameModeIndices.toList()*/)
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