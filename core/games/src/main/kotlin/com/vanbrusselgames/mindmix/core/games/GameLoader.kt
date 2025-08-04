package com.vanbrusselgames.mindmix.core.games

import android.content.Context
import android.net.ConnectivityManager
import android.net.Network
import com.vanbrusselgames.mindmix.core.common.NetworkMonitor
import dagger.hilt.android.qualifiers.ApplicationContext
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.coroutineScope
import kotlinx.coroutines.launch
import java.io.File
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class GameLoader @Inject constructor(
    @ApplicationContext private val ctx: Context, private val networkMonitor: NetworkMonitor
) {
    suspend fun registerLoader(loader: suspend (loadFromFile: Boolean) -> Unit) {
        startLoading(true, loader)
        networkMonitor.registerNetworkCallback(object : ConnectivityManager.NetworkCallback() {
            override fun onAvailable(network: Network) {
                CoroutineScope(Dispatchers.IO).launch {
                    startLoading(false, loader)
                }
            }
        })
    }

    private suspend fun startLoading(
        loadFromFile: Boolean, loader: suspend (loadFromFile: Boolean) -> Unit
    ) {
        coroutineScope {
            launch(coroutineContext) {
                loader(loadFromFile)
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