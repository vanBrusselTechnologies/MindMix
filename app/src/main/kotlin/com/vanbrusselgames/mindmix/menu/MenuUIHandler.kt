package com.vanbrusselgames.mindmix.menu

import com.vanbrusselgames.mindmix.BaseUIHandler
import com.vanbrusselgames.mindmix.SceneManager

class MenuUIHandler : BaseUIHandler() {
    companion object {
        fun startGame(game: SceneManager.Scene) {
            SceneManager.loadScene(game)
        }

        fun openShop() {}
    }
}