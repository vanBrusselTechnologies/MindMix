package com.vanbrusselgames.mindmix.menu

class MenuManager {
    companion object Instance {
        const val gameCount = 5
        const val withDuplicates = true

        fun getSelectedGameIndex(): Int {
            return menuData.selectedGameIndex
        }

        fun setSelectedGameIndex(gameIndex: Int) {
            menuData.selectedGameIndex = (gameIndex + gameCount * 10) % gameCount
        }

        fun getSelectedGameModeIndex(gameIndex: Int = -1): Int {
            val selectedGameIndex = if(gameIndex == -1) menuData.selectedGameIndex else gameIndex
            return menuData.selectedGameModeIndexes[selectedGameIndex]
        }

        fun setSelectedGameModeIndex(gameModeIndex: Int, gameIndex: Int = -1) {
            val selectedGameIndex = if(gameIndex == -1) menuData.selectedGameIndex else gameIndex
            menuData.selectedGameModeIndexes[selectedGameIndex] = gameModeIndex
        }

        private val menuData = MenuData
    }
}