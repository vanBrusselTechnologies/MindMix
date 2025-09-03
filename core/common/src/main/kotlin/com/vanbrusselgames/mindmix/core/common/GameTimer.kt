package com.vanbrusselgames.mindmix.core.common

import androidx.compose.runtime.LongState
import androidx.compose.runtime.State
import androidx.compose.runtime.mutableLongStateOf
import androidx.compose.runtime.mutableStateOf
import kotlin.time.Duration.Companion.milliseconds

class GameTimer {
    private val _running = mutableStateOf(false)
    val running: State<Boolean> = _running
    private val _currentTime = mutableLongStateOf(0)
    val currentTime: LongState = _currentTime

    var currentMillis = 0L
        private set
    private var startMillis = 0L
    private var pausedAt = 0L
    var addedMillis = 0L
        private set

    fun reset() {
        addedMillis = 0
        pausedAt = 0
        _currentTime.longValue = 0
        _running.value = false
    }

    fun start() {
        reset()
        startMillis = System.currentTimeMillis()
        currentMillis = 0L
        _running.value = true
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
        _currentTime.longValue = currentMillis + addedMillis
        _running.value = false
    }

    fun resume() {
        if (pausedAt != 0L) {
            startMillis += System.currentTimeMillis() - pausedAt
            pausedAt = 0L
            currentMillis = System.currentTimeMillis() - startMillis
            _currentTime.longValue = currentMillis + addedMillis
        }
        if (startMillis == 0L) start()
        _running.value = true
    }

    fun update() {
        currentMillis = System.currentTimeMillis() - startMillis
        _currentTime.longValue = currentMillis + addedMillis
    }

    fun stop() {
        pause()
    }

    fun addMillis(millis: Long) {
        addedMillis += millis
        _currentTime.longValue = currentMillis + addedMillis
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