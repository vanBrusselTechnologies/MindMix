package com.vanbrusselgames.mindmix.core.data

import android.content.Context
import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.preferencesDataStore
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.launch
import kotlinx.coroutines.runBlocking

val Context.dataStore: DataStore<Preferences> by preferencesDataStore("settings")

fun loadPreferences(dataStore: DataStore<Preferences>, onLoadPreferences: (Preferences) -> Unit) {
    CoroutineScope(Dispatchers.IO).launch {
        dataStore.data.first { pref ->
            onLoadPreferences(pref)
            true
        }
    }
}

fun <T> savePreferences(ctx: Context, key: Preferences.Key<T>, value: T) {
    runBlocking {
        ctx.dataStore.edit {
            it[key] = value
        }
    }
}