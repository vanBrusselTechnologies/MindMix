package com.vanbrusselgames.mindmix.preferences

import android.content.Context
import com.vanbrusselgames.mindmix.core.data.dataStore
import dagger.hilt.android.qualifiers.ApplicationContext
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.map
import javax.inject.Inject

class UserPreferencesRepository @Inject constructor(@ApplicationContext ctx: Context) {
    private val dataStore = ctx.dataStore

    fun getPreferences(): Flow<UserPreferences> {
        return dataStore.data.map { it.toUserPreferences() }
    }

    suspend fun savePreferences(prefs: UserPreferences) {
        dataStore.saveUserPreferences(prefs)
    }
}