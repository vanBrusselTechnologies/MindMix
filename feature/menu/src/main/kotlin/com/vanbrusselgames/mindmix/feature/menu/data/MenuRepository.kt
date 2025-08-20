package com.vanbrusselgames.mindmix.feature.menu.data

import com.vanbrusselgames.mindmix.core.data.DataManager
import com.vanbrusselgames.mindmix.core.model.SceneRegistry
//import com.vanbrusselgames.mindmix.feature.menu.model.MenuData
import kotlinx.serialization.json.Json
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class MenuRepository @Inject constructor(private val dataManager: DataManager) {
    private val jsonParser = Json { ignoreUnknownKeys = true }

    private fun saveProgress() {
        val data = ""//Json.encodeToString(MenuData(0))
        dataManager.saveScene(SceneRegistry.Menu, data)
    }

    fun forceSave() {
        dataManager.save(true)
    }
}
