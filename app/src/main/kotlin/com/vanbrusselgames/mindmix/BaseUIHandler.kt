package com.vanbrusselgames.mindmix

import com.vanbrusselgames.mindmix.games.GameHelp
import com.vanbrusselgames.mindmix.games.GameMenu
import com.vanbrusselgames.mindmix.games.GameTimer

abstract class BaseUIHandler {
    companion object {
        fun openSettings() {
            Settings.visible.value = true
            BaseLayout.activeOverlapUI.value = true
        }

        fun backToMenu() {
            DataManager.save()
            SceneManager.loadScene(SceneManager.Scene.MENU)
        }

        fun openGameMenu(){
            GameTimer.pauseAll()
            GameMenu.visible.value = true
            BaseLayout.activeOverlapUI.value = true
        }

        fun openHelp() {
            GameTimer.pauseAll()
            GameHelp.visible.value = true
            BaseLayout.activeOverlapUI.value = true
        }
    }
}