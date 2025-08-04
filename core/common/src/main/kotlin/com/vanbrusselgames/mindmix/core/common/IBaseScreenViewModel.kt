package com.vanbrusselgames.mindmix.core.common

import androidx.compose.ui.unit.Dp
import androidx.datastore.preferences.core.Preferences

interface IBaseScreenViewModel {
    val isMenu: Boolean
    val nameResId: Int

    val blurStrength: Dp
    val topRowButtonSize: Dp
    val padding: Dp
    val topRowHeight: Dp

    fun onOpenDialog()
}