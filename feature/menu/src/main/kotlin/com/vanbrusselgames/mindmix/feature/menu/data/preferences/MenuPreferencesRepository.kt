package com.vanbrusselgames.mindmix.feature.menu.data.preferences

import android.content.Context
import com.vanbrusselgames.mindmix.core.data.dataStore
import dagger.hilt.android.qualifiers.ApplicationContext
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.map
import javax.inject.Inject

class MenuPreferencesRepository @Inject constructor(@ApplicationContext ctx: Context) {
    private val dataStore = ctx.dataStore

    fun getPreferences(): Flow<MenuPreferences> {
        return dataStore.data.map { it.toMenuPreferences() }
    }

    suspend fun savePreferences(prefs: MenuPreferences) {
        dataStore.saveMenuPreferences(prefs)
    }
}