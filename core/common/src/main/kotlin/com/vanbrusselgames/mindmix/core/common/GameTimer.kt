package com.vanbrusselgames.mindmix.core.common

import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

class GameTimer(private val timerScope: CoroutineScope = CoroutineScope(Dispatchers.IO)) {
    private val _currentTimeMillis = MutableStateFlow(0L)
    val currentMillis = _currentTimeMillis.asStateFlow()

    private var timerJob: Job? = null

    private var isRunning = false
    private var startSystemMillis = 0L
    private var timeWhenPausedMillis = 0L
    var addedMillis = 0L
        private set

    private fun startUpdateLoop() {
        timerJob?.cancel()
        timerJob = timerScope.launch {
            while (isRunning) {
                val elapsed = System.currentTimeMillis() - startSystemMillis
                _currentTimeMillis.value = elapsed + timeWhenPausedMillis + addedMillis
                delay(100)
            }
        }
    }

    private fun stopUpdateLoop() {
        timerJob?.cancel()
        timerJob = null
    }

    fun reset() {
        stopUpdateLoop()
        addedMillis = 0L
        timeWhenPausedMillis = 0L
        startSystemMillis = 0L
        _currentTimeMillis.value = 0L
        isRunning = false
    }

    fun start() {
        reset()
        startSystemMillis = System.currentTimeMillis()
        isRunning = true
        startUpdateLoop()
    }

    fun set(millis: Long, addedMillis: Long = 0L) {
        stopUpdateLoop()
        timeWhenPausedMillis = millis
        this.addedMillis = addedMillis
        startSystemMillis = System.currentTimeMillis()
        _currentTimeMillis.value = timeWhenPausedMillis + this.addedMillis
        isRunning = false
    }

    fun pause() {
        if (!isRunning) return
        stopUpdateLoop()
        timeWhenPausedMillis = _currentTimeMillis.value - addedMillis
        isRunning = false
    }

    fun resume() {
        if (isRunning) return
        if (startSystemMillis == 0L) { // Never started
            start()
            return
        }

        startSystemMillis = System.currentTimeMillis()
        isRunning = true
        startUpdateLoop()
    }

    fun addMillis(millis: Long) {
        addedMillis += millis
        _currentTimeMillis.value += millis
    }
}