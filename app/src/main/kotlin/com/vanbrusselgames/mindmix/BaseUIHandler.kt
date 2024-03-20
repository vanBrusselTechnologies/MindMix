package com.vanbrusselgames.mindmix

import com.vanbrusselgames.mindmix.games.GameHelp
import com.vanbrusselgames.mindmix.games.GameMenu
import com.vanbrusselgames.mindmix.games.GameTimer

abstract class BaseUIHandler {
    open fun openSettings() {}

    companion object {
        fun backToMenu() {
            DataManager.save()
            SceneManager.loadScene(SceneManager.Scene.MENU)
        }

        fun openGameMenu(){
            GameTimer.pauseAll()
            GameMenu.visible.value = true
            BaseLayout.disableTopRowButtons.value = true
        }

        fun openHelp() {
            GameTimer.pauseAll()
            GameHelp.visible.value = true
            BaseLayout.disableTopRowButtons.value = true
        }
    }
}