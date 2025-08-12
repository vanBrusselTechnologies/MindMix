package com.vanbrusselgames.mindmix.core.common

import androidx.compose.ui.unit.Dp

interface IBaseScreenViewModel {
    val isMenu: Boolean

    val blurStrength: Dp
    val topRowButtonSize: Dp
    val padding: Dp
    val topRowHeight: Dp

    fun onOpenDialog()
}