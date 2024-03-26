package com.vanbrusselgames.mindmix.games

import com.vanbrusselgames.mindmix.games.sudoku.SudokuLoader
import kotlinx.coroutines.coroutineScope
import kotlinx.coroutines.launch

class GameLoader {
    companion object {
        private var initialized = false

        suspend fun init() {
            if (!initialized) {
                initialized = true
                startLoading()
            }
        }

        private suspend fun startLoading() {
            coroutineScope {
                launch(coroutineContext) { SudokuLoader.startLoading() }
            }
        }
    }
}