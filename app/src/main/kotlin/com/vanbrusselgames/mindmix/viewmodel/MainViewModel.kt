package com.vanbrusselgames.mindmix.viewmodel

import androidx.compose.runtime.mutableStateOf
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.vanbrusselgames.mindmix.core.data.UserRepository
import com.vanbrusselgames.mindmix.core.data.preferences.UserPreferences
import com.vanbrusselgames.mindmix.core.data.preferences.UserPreferencesRepository
import com.vanbrusselgames.mindmix.core.designsystem.theme.SelectedTheme
import com.vanbrusselgames.mindmix.core.logging.Logger
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.launch
import javax.inject.Inject

@HiltViewModel
class MainViewModel @Inject constructor(
    private val prefsRepository: UserPreferencesRepository
) : ViewModel() {
    private val _preferencesLoaded = MutableStateFlow(false)
    val preferencesLoaded =
        _preferencesLoaded.stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000L), false)

    val theme = mutableStateOf(SelectedTheme.System)

    init {
        viewModelScope.launch { loadPreferences() }
    }

    private suspend fun loadPreferences() {
        // Collect is required, preferences can be updated in settings
        prefsRepository.getPreferences().collect { applyPreferences(it) }
    }

    private suspend fun applyPreferences(preferences: UserPreferences) {
        Logger.d("[main] applyPreferences")
        theme.value = SelectedTheme.entries.first { it.ordinal == preferences.theme }
        _preferencesLoaded.emit(true)
    }
}