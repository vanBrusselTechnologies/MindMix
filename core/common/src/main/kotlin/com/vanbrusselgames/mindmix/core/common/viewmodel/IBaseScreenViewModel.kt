package com.vanbrusselgames.mindmix.core.common.viewmodel

import androidx.compose.ui.unit.Dp

interface IBaseScreenViewModel {
    val isMenu: Boolean

    val blurStrength: Dp

    fun onOpenDialog()
}