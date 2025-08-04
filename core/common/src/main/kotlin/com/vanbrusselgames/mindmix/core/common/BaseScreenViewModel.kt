package com.vanbrusselgames.mindmix.core.common

import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.times
import androidx.lifecycle.ViewModel

abstract class BaseScreenViewModel : ViewModel(), IBaseScreenViewModel {
    abstract override val isMenu: Boolean
    abstract override val nameResId: Int

    override val blurStrength = 10.dp

    override val topRowButtonSize = 48.dp
    override val padding = topRowButtonSize / 100f
    override val topRowHeight = (topRowButtonSize + 2f * padding)

    override fun onOpenDialog() {}
}