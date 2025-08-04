package com.vanbrusselgames.mindmix.core.navigation

import androidx.compose.runtime.mutableStateOf
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.logging.Logger
import com.vanbrusselgames.mindmix.core.model.Scene
import com.vanbrusselgames.mindmix.core.model.SceneRegistry

class SceneManager {
    companion object {
        var currentScene: Scene = SceneRegistry.Menu
        val dialogActiveState = mutableStateOf(false)
    }

    val onDestinationChange =
        NavController.OnDestinationChangedListener { navController, destination, bundle ->
            dialogActiveState.value = destination.navigatorName === "dialog"
        }
}

fun NavController.navigateToMenu() {
    Logger.d("Navigate to: Menu")
    SceneManager.currentScene = SceneRegistry.Menu
    navigate(AppRoutes.Menu) {
        popUpTo(AppRoutes.Menu) { inclusive = true }
    }
}