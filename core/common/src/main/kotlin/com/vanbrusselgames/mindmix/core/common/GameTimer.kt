package com.vanbrusselgames.mindmix.core.common

import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.mutableLongStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import kotlinx.coroutines.delay
import kotlin.math.floor
import kotlin.math.max

class GameTimer {
    private val running = mutableStateOf(false)
    var currentMillis = 0L
        private set
    private var startMillis = 0L
    private var pausedAt = 0L
    private var currentTime = mutableLongStateOf(0)
    var penaltyMillis = 0L

    fun start() {
        startMillis = System.currentTimeMillis()
        currentMillis = 0L
        running.value = true
    }

    fun set(millis: Long) {
        startMillis = System.currentTimeMillis() - millis
        currentMillis = millis
        pause()
    }

    fun pause() {
        if (pausedAt != 0L) return
        pausedAt = System.currentTimeMillis()
        running.value = false
    }

    fun resume() {
        if (pausedAt != 0L) {
            startMillis += System.currentTimeMillis() - pausedAt
            pausedAt = 0L
            currentMillis = System.currentTimeMillis() - startMillis
            currentTime.longValue = currentMillis
        }
        running.value = true
    }

    fun stop() {
        pause()
        currentMillis = pausedAt - startMillis
        currentTime.longValue = currentMillis
    }

    fun addMillis(millis: Long) {
        // todo: keep penalty seconds separated
        //  penaltyMillis += millis

        startMillis -= millis
        currentMillis += millis
        currentTime.longValue = currentMillis
    }

    @Composable
    fun Timer(viewModel: BaseGameViewModel) {
        if (!SceneManager.dialogActiveState.value && SceneManager.currentScene != SceneManager.Scene.MENU) {
            resume()
        }

        currentTime = remember { mutableLongStateOf(currentMillis) }

        val seconds = max(currentTime.longValue / 1000f, 0f)
        Text(
            text = String.format(
                "%02d:%02d:%02d",
                floor(seconds / 3600f).toInt(),
                (floor(seconds / 60f) % 60).toInt(),
                (seconds % 60).toInt()
            )
        )
        if (running.value) {
            LaunchedEffect(currentTime.longValue) {
                val diff = ((startMillis - System.currentTimeMillis()) % 1000 - 1000) % 1000 + 1000
                delay(diff)
                if (!running.value) return@LaunchedEffect
                currentMillis = System.currentTimeMillis() - startMillis
                currentTime.longValue = currentMillis
            }
        }
    }

    fun formatTime(includeMillis: Boolean): String {
        if (includeMillis) {
            return String.format(
                "%01d:%02d:%02d.%03d",
                floor(currentMillis / 3600000f).toInt(),
                (floor(currentMillis / 60000f) % 60).toInt(),
                (floor(currentMillis / 1000f) % 60).toInt(),
                currentMillis % 1000
            )
        }
        return String.format(
            "%01d:%02d:%02d",
            floor(currentMillis / 3600000f).toInt(),
            (floor(currentMillis / 60000f) % 60).toInt(),
            (floor(currentMillis / 1000f) % 60).toInt()
        )
    }
}