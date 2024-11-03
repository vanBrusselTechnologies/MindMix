package com.vanbrusselgames.mindmix.core.common

import android.content.Context
import android.net.ConnectivityManager
import android.net.Network
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.coroutineScope
import kotlinx.coroutines.launch
import java.io.File

class GameLoader {
    companion object {
        private var initialized = false

        private lateinit var ctx: Context

        suspend fun init(ctx: Context, networkMonitor: NetworkMonitor) {
            this.ctx = ctx
            if (!initialized) {
                initialized = true
                startLoading(true)
            }
            networkMonitor.registerNetworkCallback(object : ConnectivityManager.NetworkCallback() {
                override fun onAvailable(network: Network) {
                    CoroutineScope(Dispatchers.IO).launch {
                        startLoading(false)
                    }
                }
            })
        }

        private suspend fun startLoading(loadFromFile: Boolean) {
            coroutineScope {
                launch(coroutineContext) {
                    /*todo:
                        try {
                            SudokuLoader.requestPuzzles(
                                MainActivity.sudoku.viewModel,
                                PuzzleType.Classic,
                                loadFromFile
                            )
                        } catch (_: Exception) {
                        }
                     */
                }
            }
        }

        fun getFileName(gameId: Int, gameModeId: Int, difficulty: String): String {
            return "loadedPuzzles/${gameId}/${gameModeId}/${difficulty}.save.vbg"
        }

        private fun getFileName(filePath: String): String {
            val dirsAndFile = filePath.split("/").toMutableList()
            return dirsAndFile.removeAt(dirsAndFile.lastIndex)
        }

        private fun getDir(filePath: String, fileName: String): File {
            val dirPath = filePath.substring(0, filePath.length - fileName.length)
            val dir = File(ctx.filesDir, dirPath)
            if (!dir.exists()) dir.mkdirs()
            return dir
        }

        private fun getFile(filePath: String): File {
            val fileName = getFileName(filePath)
            val file = File(getDir(filePath, fileName), fileName)
            file.createNewFile()
            return file
        }

        fun readFile(fileName: String): List<String> {
            return getFile(fileName).readLines()
        }

        fun appendToFile(fileName: String, puzzles: List<String>) {
            val file = getFile(fileName)
            val content = file.readLines()
            val newPuzzles = mutableListOf<String>()
            for (puzzle in puzzles.distinct()) {
                if (!content.contains(puzzle)) newPuzzles.add(puzzle)
            }
            val prefix = if (content.isEmpty()) "" else "\n"
            val addedContent = newPuzzles.joinToString("\n")
            file.appendText(prefix + addedContent)
        }

        fun removeFromFile(fileName: String, puzzle: String) {
            val file = getFile(fileName)
            val content = file.readLines().distinct().toMutableList()
            content.remove(puzzle)
            file.writeText(content.joinToString("\n"))
        }
    }
}