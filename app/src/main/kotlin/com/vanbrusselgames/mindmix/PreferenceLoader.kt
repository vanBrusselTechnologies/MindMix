package com.vanbrusselgames.mindmix

import android.content.Context
import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.core.booleanPreferencesKey
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.core.emptyPreferences
import androidx.datastore.preferences.core.intPreferencesKey
import com.vanbrusselgames.mindmix.games.minesweeper.MinesweeperManager
import com.vanbrusselgames.mindmix.games.solitaire.PlayingCard
import com.vanbrusselgames.mindmix.games.solitaire.SolitaireManager
import com.vanbrusselgames.mindmix.games.sudoku.SudokuManager
import com.vanbrusselgames.mindmix.menu.MenuManager
import com.vanbrusselgames.mindmix.ui.theme.SelectedTheme
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.flow.map
import kotlinx.coroutines.runBlocking

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
            if (pref[PREF_KEY_SUDOKU__CHECK_CONFLICTING_CELLS] != null) {
                SudokuManager.checkConflictingCells.value =
                    pref[PREF_KEY_SUDOKU__CHECK_CONFLICTING_CELLS]!!
            }
            if (pref[PREF_KEY_SUDOKU__AUTO_EDIT_NOTES] != null) {
                SudokuManager.autoEditNotes.value = pref[PREF_KEY_SUDOKU__AUTO_EDIT_NOTES]!!
            }
            if (pref[PREF_KEY_SOLITAIRE__CARD_TYPE] != null) {
                SolitaireManager.cardVisualType.value = PlayingCard.CardVisualType.entries.first {
                    it.ordinal == pref[PREF_KEY_SOLITAIRE__CARD_TYPE]
                }
            }
            if (pref[PREF_KEY_MENU__THEME] != null) {
                MenuManager.theme.value = SelectedTheme.entries.first {
                    it.ordinal == pref[PREF_KEY_MENU__THEME]
                }
            }
            if (pref[PREF_KEY_MINESWEEPER__AUTO_FLAG] != null) {
                MinesweeperManager.autoFlag = pref[PREF_KEY_MINESWEEPER__AUTO_FLAG]!!
            }
            if (pref[PREF_KEY_MINESWEEPER__SAFE_START] != null) {
                MinesweeperManager.safeStart = pref[PREF_KEY_MINESWEEPER__SAFE_START]!!
            }
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