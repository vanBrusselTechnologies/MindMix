package com.vanbrusselgames.mindmix.feature.settings.viewmodel

import androidx.compose.runtime.mutableStateOf
import androidx.lifecycle.ViewModel
import com.vanbrusselgames.mindmix.core.designsystem.theme.SelectedTheme
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow

class MockSettingsViewModel : ViewModel(), ISettingsViewModel {
    override val preferencesLoaded = MutableStateFlow(false).asStateFlow()

    override val theme = mutableStateOf(SelectedTheme.System)

    override val userId = mutableStateOf("")
    override val signedIn = mutableStateOf(false)

    override fun setTheme(value: SelectedTheme) {
        theme.value = value
    }
}