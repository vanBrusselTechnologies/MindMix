package com.vanbrusselgames.mindmix.core.common

interface IBaseGameViewModel : IBaseScreenViewModel {
    abstract val descResId: Int

    abstract fun startNewGame()
}