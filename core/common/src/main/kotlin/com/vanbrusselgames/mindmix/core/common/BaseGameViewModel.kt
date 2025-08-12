package com.vanbrusselgames.mindmix.core.common

abstract class BaseGameViewModel : BaseScreenViewModel(), IBaseGameViewModel {
    override val isMenu = false

    abstract override fun startNewGame()
}