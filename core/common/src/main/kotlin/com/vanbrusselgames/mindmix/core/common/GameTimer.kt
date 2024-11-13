package com.vanbrusselgames.mindmix.core.common

import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.mutableLongStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import kotlinx.coroutines.delay
import kotlin.time.Duration.Companion.milliseconds
import kotlin.time.Duration.Companion.seconds

class GameTimer {
    private val running = mutableStateOf(false)
    var currentMillis = 0L
        private set
    private var startMillis = 0L
    private var pausedAt = 0L
    var addedMillis = 0L
        private set
    private var currentTime = mutableLongStateOf(0)

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
        currentMillis = pausedAt - startMillis
        currentTime.longValue = currentMillis + addedMillis
        running.value = false
    }

    fun resume() {
        if (pausedAt != 0L) {
            startMillis += System.currentTimeMillis() - pausedAt
            pausedAt = 0L
            currentMillis = System.currentTimeMillis() - startMillis
            currentTime.longValue = currentMillis + addedMillis
        }
        running.value = true
    }

    fun stop() {
        pause()
    }

    fun addMillis(millis: Long) {
        addedMillis += millis
        currentTime.longValue = currentMillis + addedMillis
    }

    @Composable
    fun Timer() {
        if (!SceneManager.dialogActiveState.value && SceneManager.currentScene != SceneManager.Scene.MENU) {
            resume()
        }

        currentTime = remember { mutableLongStateOf(currentMillis + addedMillis) }

        Text(formatDuration(currentTime.longValue, false))

        LaunchedEffect(running.value) {
            while (running.value) {
                delay(1.seconds)
                currentMillis = System.currentTimeMillis() - startMillis
                currentTime.longValue = currentMillis + addedMillis
            }
        }
    }

    fun formatDuration(millis: Long, includeMillis: Boolean = false): String {
        return millis.milliseconds.toComponents { m, s, n ->
            val minutes = if (m < 10) "0$m" else m.toString()
            val seconds = if (s < 10) "0$s" else s.toString()
            if (includeMillis) {
                val mil = n / 1000000
                val millis = if (mil < 10) "00$mil" else if (mil < 100) "0$mil" else mil.toString()
                "$minutes:$seconds.${millis}"
            } else {
                "$minutes:$seconds"
            }
        }
    }
}