package com.vanbrusselgames.mindmix.feature.menu.model

import com.vanbrusselgames.mindmix.feature.menu.viewmodel.IMenuScreenViewModel

data class GameWheel(val viewModel: IMenuScreenViewModel, val gameCount: Int) {
    private val withDuplicates = true
    val wheelItemCount = gameCount * if (withDuplicates) 2 else 1
    val angleStep = 360f / wheelItemCount

    var selectedId = viewModel.games.filter { it.value == viewModel.selectedGame }.keys.first()
    var rotationAngle = selectedId * angleStep
}