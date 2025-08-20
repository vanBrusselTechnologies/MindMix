package com.vanbrusselgames.mindmix.feature.settings.viewmodel

import androidx.compose.runtime.MutableState
import com.vanbrusselgames.mindmix.core.designsystem.theme.SelectedTheme
import kotlinx.coroutines.flow.StateFlow

interface ISettingsViewModel {
    val preferencesLoaded: StateFlow<Boolean>

    val theme: MutableState<SelectedTheme>
    val userId: MutableState<String>
    val signedIn: MutableState<Boolean>

    fun setTheme(value: SelectedTheme)
}