package com.vanbrusselgames.mindmix.feature.settings.viewmodel

import androidx.compose.runtime.mutableStateOf
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.vanbrusselgames.mindmix.core.authentication.AuthManager
import com.vanbrusselgames.mindmix.core.data.preferences.UserPreferences
import com.vanbrusselgames.mindmix.core.data.preferences.UserPreferencesRepository
import com.vanbrusselgames.mindmix.core.designsystem.theme.SelectedTheme
import com.vanbrusselgames.mindmix.core.logging.Logger
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.flow.onStart
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class SettingsViewModel @Inject constructor(
    authManager: AuthManager, private val prefsRepository: UserPreferencesRepository
) : ViewModel(), ISettingsViewModel {
    private val _preferencesLoaded = MutableStateFlow(false)
    override val preferencesLoaded = _preferencesLoaded.onStart { loadPreferences() }
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000L), false)

    override val theme = mutableStateOf(SelectedTheme.System)

    override val userId = authManager.userId
    override val signedIn = authManager.signedIn

    private suspend fun loadPreferences() {
        applyPreferences(prefsRepository.getPreferences().first())
    }

    private suspend fun applyPreferences(preferences: UserPreferences) {
        Logger.d("[settings] applyPreferences")
        theme.value = SelectedTheme.entries.first { it.ordinal == preferences.theme }
        _preferencesLoaded.emit(true)
    }

    override fun setTheme(value: SelectedTheme) {
        theme.value = value

        viewModelScope.launch {
            prefsRepository.savePreferences(UserPreferences(value.ordinal))
        }
    }
}