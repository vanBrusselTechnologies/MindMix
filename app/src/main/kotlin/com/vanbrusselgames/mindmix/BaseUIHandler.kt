package com.vanbrusselgames.mindmix

abstract class BaseUIHandler {
    open fun openSettings() {}

    open fun backToMenu() = SceneManager.setCurrentScene(SceneManager.Scene.MENU)

    open fun openShop() {}
}