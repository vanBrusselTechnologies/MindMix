package com.vanbrusselgames.mindmix.core.data

import android.content.Context
import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.core.booleanPreferencesKey
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.core.emptyPreferences
import androidx.datastore.preferences.core.intPreferencesKey
import androidx.datastore.preferences.preferencesDataStore
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.flow.map
import kotlinx.coroutines.runBlocking

val Context.dataStore: DataStore<Preferences> by preferencesDataStore(name = "settings")

//region Sudoku
val PREF_KEY_SUDOKU__CHECK_CONFLICTING_CELLS = booleanPreferencesKey("sudoku_checkConflictingCells")
val PREF_KEY_SUDOKU__AUTO_EDIT_NOTES = booleanPreferencesKey("sudoku_autoEditNotes")
//endregion

//region Solitaire
val PREF_KEY_SOLITAIRE__CARD_TYPE = intPreferencesKey("solitaire_cardType")
//endregion

//region Minesweeper
val PREF_KEY_MINESWEEPER__AUTO_FLAG = booleanPreferencesKey("minesweeper_autoFlag")
val PREF_KEY_MINESWEEPER__SAFE_START = booleanPreferencesKey("minesweeper_safeStart")
//endregion

//region Menu
val PREF_KEY_MENU__THEME = intPreferencesKey("menu_theme")
//endregion

fun loadPreferences(dataStore: DataStore<Preferences>) {
    runBlocking {
        dataStore.data.map { pref ->
            /*todo:
                if (pref[PREF_KEY_SUDOKU__CHECK_CONFLICTING_CELLS] != null) {
                    MainActivity.sudoku.viewModel.checkConflictingCells.value =
                        pref[PREF_KEY_SUDOKU__CHECK_CONFLICTING_CELLS]!!
                }
                if (pref[PREF_KEY_SUDOKU__AUTO_EDIT_NOTES] != null) {
                    MainActivity.sudoku.viewModel.autoEditNotes.value =
                        pref[PREF_KEY_SUDOKU__AUTO_EDIT_NOTES]!!
                }
                if (pref[PREF_KEY_SOLITAIRE__CARD_TYPE] != null) {
                    MainActivity.solitaire.viewModel.cardVisualType.value =
                        PlayingCard.CardVisualType.entries.first {
                            it.ordinal == pref[PREF_KEY_SOLITAIRE__CARD_TYPE]
                        }
                }
                if (pref[PREF_KEY_MENU__THEME] != null) {
                    MainActivity.menu.viewModel.theme.value =
                        SelectedTheme.entries.first {
                            it.ordinal == pref[PREF_KEY_MENU__THEME]
                        }
                }
                if (pref[PREF_KEY_MINESWEEPER__AUTO_FLAG] != null) {
                    MainActivity.minesweeper.viewModel.autoFlag =
                        pref[PREF_KEY_MINESWEEPER__AUTO_FLAG]!!
                }
                if (pref[PREF_KEY_MINESWEEPER__SAFE_START] != null) {
                    MainActivity.minesweeper.viewModel.safeStart =
                        pref[PREF_KEY_MINESWEEPER__SAFE_START]!!
                }
             */
            emptyPreferences()
        }.first()
    }
}

fun <T> savePreferences(ctx: Context, key: Preferences.Key<T>, value: T) {
    runBlocking {
        ctx.dataStore.edit {
            it[key] = value
        }
    }
}