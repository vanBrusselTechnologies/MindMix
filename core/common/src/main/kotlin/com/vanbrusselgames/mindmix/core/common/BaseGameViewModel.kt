package com.vanbrusselgames.mindmix.core.common

abstract class BaseGameViewModel : BaseScreenViewModel(), IBaseGameViewModel {
    abstract override val descResId: Int

    override val isMenu = false

    abstract override fun startNewGame()
}