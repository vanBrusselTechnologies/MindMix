package com.vanbrusselgames.mindmix.feature.menu.viewmodel

import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.common.IBaseScreenViewModel
import com.vanbrusselgames.mindmix.core.model.GameScene
import com.vanbrusselgames.mindmix.feature.menu.model.GameWheel
import kotlinx.coroutines.flow.StateFlow

interface IMenuScreenViewModel : IBaseScreenViewModel {
    val preferencesLoaded: StateFlow<Boolean>
    val games: Map<Int, GameScene>
    var selectedGame: GameScene
    val wheelModel: GameWheel

    fun navigateToSelectedGame(navController: NavController)
}