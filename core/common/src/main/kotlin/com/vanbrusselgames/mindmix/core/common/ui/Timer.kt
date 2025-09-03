package com.vanbrusselgames.mindmix.core.common.ui

import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.remember
import androidx.compose.ui.Modifier
import com.vanbrusselgames.mindmix.core.common.GameTimer
import com.vanbrusselgames.mindmix.core.model.SceneRegistry
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import kotlinx.coroutines.delay
import kotlin.time.Duration.Companion.seconds

@Composable
fun Timer(timer: GameTimer, modifier: Modifier = Modifier) {
    LaunchedEffect(SceneManager.dialogActiveState.value, SceneManager.currentScene) {
        if (!SceneManager.dialogActiveState.value && SceneManager.currentScene != SceneRegistry.Menu) timer.resume()
    }

    val formattedTime = remember(timer.currentTime.longValue) {
        timer.formatDuration(timer.currentTime.longValue, false)
    }

    Text(formattedTime, modifier)

    LaunchedEffect(timer.running.value) {
        while (timer.running.value) {
            delay(1.seconds)
            timer.update()
        }
    }
}