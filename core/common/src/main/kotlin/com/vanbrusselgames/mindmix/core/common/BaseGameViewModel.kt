package com.vanbrusselgames.mindmix.core.common

abstract class BaseGameViewModel : BaseScreenViewModel() {
    abstract val descResId: Int

    override val isMenu = false

    abstract fun startNewGame()
}