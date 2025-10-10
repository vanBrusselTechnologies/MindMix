package com.vanbrusselgames.mindmix.games.game2048.viewmodel

import android.app.Activity
import androidx.compose.runtime.LongState
import androidx.compose.runtime.MutableState
import androidx.compose.runtime.State
import androidx.compose.runtime.snapshots.SnapshotStateList
import androidx.compose.ui.geometry.Offset
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.common.viewmodel.IBaseGameViewModel
import com.vanbrusselgames.mindmix.games.game2048.model.FinishedGame
import com.vanbrusselgames.mindmix.games.game2048.model.GridCell2048
import com.vanbrusselgames.mindmix.games.game2048.model.GridSize2048
import kotlinx.coroutines.flow.StateFlow

interface IGame2048ViewModel : IBaseGameViewModel {
    val finishedGame: State<FinishedGame>
    val gridSize: State<GridSize2048>
    val cellList: SnapshotStateList<GridCell2048>
    val score: LongState

    val puzzleLoaded: StateFlow<Boolean>
    val preferencesLoaded: StateFlow<Boolean>

    var finished: Boolean

    fun handleDragGestures(navController: NavController, totalDragOffset: Offset)

    fun forceSave()
    fun checkAdLoaded(activity: Activity, adLoaded: MutableState<Boolean>)
    fun showAd(activity: Activity, adLoaded: MutableState<Boolean>, onAdWatched: (Int) -> Unit)

    fun setSize(value: GridSize2048)
}