package com.vanbrusselgames.mindmix.menu

import com.vanbrusselgames.mindmix.BaseLayout
import com.vanbrusselgames.mindmix.BaseUIHandler
import com.vanbrusselgames.mindmix.SceneManager
import com.vanbrusselgames.mindmix.Settings

class MenuUIHandler : BaseUIHandler() {

    override fun openSettings() {
        Settings.visible.value = true
        BaseLayout.disableTopRowButtons.value = true
    }

    companion object {
        fun startGame(game: SceneManager.Scene) {
            SceneManager.loadScene(game)
        }

        fun openShop() {}
    }
}