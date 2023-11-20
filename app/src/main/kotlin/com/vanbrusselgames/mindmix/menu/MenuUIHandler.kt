package com.vanbrusselgames.mindmix.menu

import android.util.Log
import com.vanbrusselgames.mindmix.BaseUIHandler
import com.vanbrusselgames.mindmix.SceneManager

class MenuUIHandler : BaseUIHandler() {

    override fun openSettings() {
        super.openSettings()
        Log.d("MindMix", "Show Settings")
    }

    override fun backToMenu() {
        Log.d("MindMix", "Back to menu")
    }

    fun startGameByIndex(index: Int) {
        Log.d("MindMix", "Selected gameIndex: $index")
        SceneManager.setCurrentScene(index)
    }
}