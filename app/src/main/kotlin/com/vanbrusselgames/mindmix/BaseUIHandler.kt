package com.vanbrusselgames.mindmix

abstract class BaseUIHandler {
    open fun openSettings() {}

    open fun backToMenu(){
        DataManager.save()
        SceneManager.loadScene(SceneManager.Scene.MENU)
    }

    open fun openShop() {}
}