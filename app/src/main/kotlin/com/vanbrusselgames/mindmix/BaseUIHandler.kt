package com.vanbrusselgames.mindmix

abstract class BaseUIHandler {
    open fun openSettings() {}

    companion object {
        fun backToMenu() {
            DataManager.save()
            SceneManager.loadScene(SceneManager.Scene.MENU)
        }

        fun openGameMenu(){
            GameMenu.visible.value = true
            BaseLayout.disableTopRowButtons.value = true
        }

        fun openHelp() {
            BaseLayout.helpOpened.value = true
            BaseLayout.disableTopRowButtons.value = true
        }
    }
}