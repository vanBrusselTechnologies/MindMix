package com.vanbrusselgames.mindmix.core.data

import android.content.Context
import android.os.Handler
import android.os.Looper
import com.google.firebase.Firebase
import com.google.firebase.storage.StorageReference
import com.google.firebase.storage.storage
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.runBlocking
import kotlinx.serialization.json.Json
import java.io.File

class DataManager(ctx: Context) {
    companion object {
        private const val FILE_NAME = "save.vbg"
        private val fileDecoding = Charsets.UTF_8
        private lateinit var file: File
        private var loaded = false
        private val storage = Firebase.storage
        private var isAutoSaving = false

        private val storagePath: StorageReference?
            get() = AuthManager.userId.value.let {
                return when (it) {
                    "" -> null
                    else -> storage.reference.child("Users/${it}/$FILE_NAME")
                }
            }

        private val jsonParser = Json { ignoreUnknownKeys = true }

        private fun load() {/*todo: try
               {
                    //Download online file from firebase
                    val content = file.readLines(fileDecoding)
                    for (line in content) {
                        try {
                            if (line.isEmpty()) continue
                            val i = line.indexOf('%')
                            if (i == -1) continue
                            val safeCode = line.substring(0, i).toInt()
                            val json = line.substring(i + 1)
                            when (com.vanbrusselgames.mindmix.core.navigation.SceneManager.scenes[safeCode]) {
                                com.vanbrusselgames.mindmix.core.navigation.SceneManager.Scene.SUDOKU -> {
                                    val data = jsonParser.decodeFromString<com.vanbrusselgames.mindmix.games.sudoku.SudokuData>(json)
                                    com.vanbrusselgames.mindmix.games.sudoku.SudokuLoader.loadFromFile(
                                        com.vanbrusselgames.mindmix.games.sudoku.Sudoku.viewModel,
                                        data
                                    )
                                }
                                com.vanbrusselgames.mindmix.core.navigation.SceneManager.Scene.SOLITAIRE -> {
                                    val data = jsonParser.decodeFromString<com.vanbrusselgames.mindmix.games.solitaire.SolitaireData>(json)
                                    com.vanbrusselgames.mindmix.games.solitaire.GameViewModel.loadFromFile(
                                        data
                                    )
                                }
                                com.vanbrusselgames.mindmix.core.navigation.SceneManager.Scene.MINESWEEPER -> {
                                    val data = jsonParser.decodeFromString<com.vanbrusselgames.mindmix.games.minesweeper.MinesweeperData>(json)
                                    com.vanbrusselgames.mindmix.games.minesweeper.GameViewModel.loadFromFile(
                                        data
                                    )
                                }
                                com.vanbrusselgames.mindmix.core.navigation.SceneManager.Scene.MENU -> {
                                    val data = jsonParser.decodeFromString<com.vanbrusselgames.mindmix.feature.menu.MenuData>(json)
                                    com.vanbrusselgames.mindmix.feature.menu.MenuScreenViewModel.loadFromFile(
                                        data
                                    )
                                }
                                com.vanbrusselgames.mindmix.core.navigation.SceneManager.Scene.GAME2048 -> {
                                }
                                null -> {}
                            }
                        } catch (e: Exception) {
                            Logger.e("Error reading line of save file", e)
                        }
                    }
                } catch (e: Exception) {
                    Logger.e("Error loading save file", e)
                }
                loaded = true
            */
        }

        fun save(withPublish: Boolean = true) {/*todo:
                if (!loaded) return
                CoroutineScope(Dispatchers.IO).launch {
                    val str: StringBuilder = StringBuilder()
                    for (entry in com.vanbrusselgames.mindmix.core.navigation.SceneManager.scenes) {
                        val dataString = when (entry.value) {
                            com.vanbrusselgames.mindmix.core.navigation.SceneManager.Scene.SUDOKU -> com.vanbrusselgames.mindmix.games.sudoku.GameViewModel.saveToFile()
                            com.vanbrusselgames.mindmix.core.navigation.SceneManager.Scene.SOLITAIRE -> com.vanbrusselgames.mindmix.games.solitaire.GameViewModel.saveToFile()
                            com.vanbrusselgames.mindmix.core.navigation.SceneManager.Scene.MINESWEEPER -> com.vanbrusselgames.mindmix.games.minesweeper.GameViewModel.saveToFile()
                            com.vanbrusselgames.mindmix.core.navigation.SceneManager.Scene.MENU -> com.vanbrusselgames.mindmix.feature.menu.MenuScreenViewModel.saveToFile()
                            com.vanbrusselgames.mindmix.core.navigation.SceneManager.Scene.GAME2048 -> null
                        }
                        str.append("\n${entry.key}%$dataString")
                    }
                    file.writeText(str.toString().trim(), fileDecoding)
                    if (withPublish) {
                        storagePath?.putStream(file.inputStream())
                        file.inputStream().close()
                    }
                }
            */
        }
    }

    init {
        file = File(ctx.filesDir, FILE_NAME)
        file.createNewFile()
        runBlocking(Dispatchers.IO) {
            if (!loaded) load()
            autoSave()
            true
        }
    }

    var autoSaveCount = 0
    private fun autoSave() {
        if (isAutoSaving) return
        isAutoSaving = true
        val mainHandler = Handler(Looper.getMainLooper())
        mainHandler.post(object : Runnable {
            override fun run() {
                save(autoSaveCount++ % 12 == 0)
                mainHandler.postDelayed(this, 5000)
            }
        })
    }
}