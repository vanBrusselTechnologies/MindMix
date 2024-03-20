package com.vanbrusselgames.mindmix.games

import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.rememberCoroutineScope
import com.vanbrusselgames.mindmix.BaseLayout
import com.vanbrusselgames.mindmix.SceneManager
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch
import kotlin.math.floor

class GameTimer {
    companion object {
        val instances: MutableList<GameTimer> = mutableListOf()

        fun pauseAll() {
            for (timer in instances) {
                timer.pause()
            }
        }
    }

    init {
        instances.add(this)
    }

    private var running = false
    private var currentSeconds = mutableIntStateOf(0)
    var currentMillis = 0L
        private set
    private var currentSecs = 0
    private var startMillis = 0L
    private var pausedAt = 0L

    fun start() {
        running = true
        startMillis = System.currentTimeMillis()
        currentMillis = 0L
        currentSecs = 0
        currentSeconds.intValue = 0
    }

    fun start(millis: Long) {
        running = true
        startMillis = System.currentTimeMillis() - millis
        currentMillis = millis
        currentSeconds.intValue = (currentMillis / 1000).toInt()
    }

    fun pause() {
        pausedAt = System.currentTimeMillis()
        running = false
    }

    fun resume() {
        if (pausedAt != 0L) {
            startMillis += System.currentTimeMillis() - pausedAt
            pausedAt = 0L
        }
        running = true
    }

    fun stop() {
        pause()
        currentMillis = pausedAt - startMillis
    }

    fun addMillis(millis: Long){
        startMillis -= millis
        currentMillis += millis
    }

    @Composable
    fun Timer() {
        if (!BaseLayout.disableTopRowButtons.value && SceneManager.currentScene != SceneManager.Scene.MENU) running = true
        val scope = rememberCoroutineScope()
        LaunchedEffect(true) {
            scope.launch(coroutineContext) {
                while (true) {
                    if (running) {
                        currentMillis = System.currentTimeMillis() - startMillis
                        val sec = (currentMillis / 1000).toInt()
                        if (currentSecs != sec) {
                            currentSecs = sec
                            currentSeconds.intValue = sec
                        }
                    }
                    delay(250)
                }
            }
        }
        val seconds = currentSeconds.intValue
        Text(
            text = String.format(
                "%02d:%02d:%02d",
                floor(seconds / 360f).toInt(),
                (floor(seconds / 60f) % 60).toInt(),
                seconds % 60
            )
        )
    }

    fun formatTime(includeMillis:Boolean): String{
        if(includeMillis){
            return String.format(
                "%01d:%02d:%02d.%03d",
                floor(currentMillis / 360000f).toInt(),
                (floor(currentMillis / 60000f) % 60).toInt(),
                (floor(currentMillis / 1000f) % 60).toInt(),
                currentMillis % 1000
            )
        }
        return String.format(
            "%01d:%02d:%02d",
            floor(currentMillis / 360000f).toInt(),
            (floor(currentMillis / 60000f) % 60).toInt(),
            (floor(currentMillis / 1000f) % 60).toInt()
        )
    }
}