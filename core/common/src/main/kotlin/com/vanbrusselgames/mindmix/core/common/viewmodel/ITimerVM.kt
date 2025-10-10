package com.vanbrusselgames.mindmix.core.common.viewmodel

import com.vanbrusselgames.mindmix.core.common.model.GameTimer

interface ITimerVM {
    val timer: GameTimer

    fun onOpenDialog()
}