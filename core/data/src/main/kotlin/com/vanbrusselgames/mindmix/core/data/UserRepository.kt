package com.vanbrusselgames.mindmix.core.data

import com.vanbrusselgames.mindmix.core.logging.Logger
import com.vanbrusselgames.mindmix.core.model.SceneRegistry
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.serialization.json.Json
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class UserRepository @Inject constructor(private val dataManager: DataManager) {
    private val jsonParser = Json { ignoreUnknownKeys = true }

    private val _coins = MutableStateFlow(-1)
    val coins: StateFlow<Int> = _coins.asStateFlow()

    fun initUserData() {
        if (coins.value != -1) return
        val json = dataManager.getSavedDataForScene(SceneRegistry.Menu)
        if (json.trim() == "") {
            Logger.d("New user, set user data base values")
            _coins.value = 0
            saveProgress()
            return
        }
        val data = jsonParser.decodeFromString<UserData>(json)
        _coins.value = data.coins
    }

    fun addCoins(amount: Int) {
        _coins.value += amount
        saveProgress()
    }

    private fun saveProgress() {
        val data = Json.encodeToString(UserData(coins.value))
        dataManager.saveScene(SceneRegistry.Menu, data)
    }
}
