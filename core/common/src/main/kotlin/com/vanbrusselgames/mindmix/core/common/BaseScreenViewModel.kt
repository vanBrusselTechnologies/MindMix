package com.vanbrusselgames.mindmix.core.common

import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.times
import androidx.datastore.preferences.core.Preferences
import androidx.lifecycle.ViewModel

abstract class BaseScreenViewModel : ViewModel() {
    abstract val isMenu: Boolean
    abstract val nameResId: Int

    val blurStrength = 10.dp

    val topRowButtonSize = 48.dp
    val padding = topRowButtonSize / 100f
    val topRowHeight = (topRowButtonSize + 2f * padding)

    open fun onOpenDialog() {}

    abstract fun onLoadPreferences(preferences: Preferences)
    //abstract fun onLoadData()
}