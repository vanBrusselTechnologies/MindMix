package com.vanbrusselgames.mindmix.games

import com.vanbrusselgames.mindmix.games.sudoku.SudokuLoader
import kotlinx.coroutines.coroutineScope
import kotlinx.coroutines.launch

class GameLoader {
    companion object {
        private var initialized = false
        private lateinit var sudokuLoader: SudokuLoader

        suspend fun init() {
            if (!initialized) {
                initialized = true
                sudokuLoader = SudokuLoader()

                startLoading()
            }
        }

        private suspend fun startLoading() {
            coroutineScope {
                launch(coroutineContext) { sudokuLoader.startLoading() }
            }
        }
    }
}