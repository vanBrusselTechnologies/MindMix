package com.vanbrusselgames.mindmix.core.common.ui

import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.ui.Modifier
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import com.vanbrusselgames.mindmix.core.common.GameTimer
import com.vanbrusselgames.mindmix.core.model.SceneRegistry
import com.vanbrusselgames.mindmix.core.navigation.SceneManager

@Composable
fun Timer(timer: GameTimer, modifier: Modifier = Modifier) {
    LaunchedEffect(SceneManager.dialogActiveState.value, SceneManager.currentScene) {
        if (!SceneManager.dialogActiveState.value && SceneManager.currentScene != SceneRegistry.Menu) timer.resume()
    }
    val currentTime by timer.currentMillis.collectAsStateWithLifecycle()

    Text(formatDuration(currentTime, false), modifier)
}

fun formatDuration(millis: Long, includeMillis: Boolean): String {
    if (millis < 0) return "00:00" // Or handle error

    val totalSeconds = millis / 1000
    val minutes = totalSeconds / 60
    val seconds = totalSeconds % 60

    val mStr = if (minutes < 10) "0$minutes" else minutes
    val sStr = if (seconds < 10) "0$seconds" else seconds

    if (includeMillis) {
        val millisPart = millis % 1000
        val milStr = when {
            millisPart < 10 -> "00$millisPart"
            millisPart < 100 -> "0$millisPart"
            else -> millisPart
        }
        return "$mStr:$sStr.$milStr"
    } else {
        return "$mStr:$sStr"
    }
}
