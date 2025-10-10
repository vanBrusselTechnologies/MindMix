package com.vanbrusselgames.mindmix.core.common.viewmodel

abstract class BaseGameViewModel : BaseScreenViewModel(), IBaseGameViewModel {
    override val isMenu = false

    abstract override fun startNewGame()
}