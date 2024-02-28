package com.vanbrusselgames.mindmix

abstract class BaseUIHandler {
    open fun openSettings() {}

    open fun backToMenu(){
        DataManager.save()
        SceneManager.loadScene(SceneManager.Scene.MENU)
    }

    fun openHelp(){
        BaseLayout.helpOpened.value = true
        BaseLayout.disableTopRowButtons.value = true
    }
}