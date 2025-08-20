package com.vanbrusselgames.mindmix.core.common

import androidx.compose.ui.unit.dp
import androidx.lifecycle.ViewModel

abstract class BaseScreenViewModel : ViewModel(), IBaseScreenViewModel {
    abstract override val isMenu: Boolean

    override val blurStrength = 10.dp

    override fun onOpenDialog() {}
}