package com.vanbrusselgames.mindmix.games.solitaire.viewmodel

import android.app.Activity
import androidx.compose.runtime.MutableState
import androidx.compose.runtime.State
import androidx.compose.ui.geometry.Offset
import androidx.compose.ui.geometry.Rect
import androidx.compose.ui.geometry.Size
import androidx.compose.ui.unit.Density
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.DpSize
import androidx.compose.ui.unit.IntOffset
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.common.viewmodel.IBaseGameViewModel
import com.vanbrusselgames.mindmix.core.common.viewmodel.ITimerVM
import com.vanbrusselgames.mindmix.games.solitaire.model.CardVisualType
import com.vanbrusselgames.mindmix.games.solitaire.model.FinishedGame
import com.vanbrusselgames.mindmix.games.solitaire.model.PlayingCard
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.flow.StateFlow

interface ISolitaireViewModel : IBaseGameViewModel, ITimerVM {
    val finishedGame: State<FinishedGame>
    val cardVisualType: State<CardVisualType>
    val couldGetFinished: State<Boolean>
    val restStackEnabled: State<Boolean>

    val puzzleLoaded: StateFlow<Boolean>
    val preferencesLoaded: StateFlow<Boolean>

    val cards: Array<PlayingCard>
    val cardSize: State<Size>
    val cardSizeDp: State<DpSize>
    var distanceBetweenCards: Float
    var finished: Boolean

    fun onTap(offset: Offset, onReleaseMovingCards: () -> Unit)
    fun onDragStart(offset: Offset)
    fun moveCards(intOffset: IntOffset, dragBounds: Rect, coroutineScope: CoroutineScope)
    fun onReleaseMovingCards(navController: NavController)
    fun resetRestStack()
    fun turnFromRestStack()
    fun onUpdateTableSize(width: Dp, height: Dp, density: Density): DpSize
    fun onClickFinishGame(navController: NavController)

    fun forceSave()
    fun checkAdLoaded(activity: Activity, adLoaded: MutableState<Boolean>)
    fun showAd(activity: Activity, adLoaded: MutableState<Boolean>, onAdWatched: (Int) -> Unit)

    fun setCardVisualType(value: CardVisualType)
}