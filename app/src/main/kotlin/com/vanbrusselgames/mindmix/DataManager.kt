package com.vanbrusselgames.mindmix

import android.content.Context
import com.google.firebase.Firebase
import com.google.firebase.storage.StorageReference
import com.google.firebase.storage.storage
import com.vanbrusselgames.mindmix.menu.MenuData
import com.vanbrusselgames.mindmix.menu.MenuManager
import com.vanbrusselgames.mindmix.minesweeper.MinesweeperData
import com.vanbrusselgames.mindmix.minesweeper.MinesweeperManager
import com.vanbrusselgames.mindmix.solitaire.SolitaireData
import com.vanbrusselgames.mindmix.solitaire.SolitaireManager
import com.vanbrusselgames.mindmix.sudoku.SudokuData
import com.vanbrusselgames.mindmix.sudoku.SudokuManager
import kotlinx.coroutines.launch
import kotlinx.coroutines.withTimeout
import kotlinx.serialization.json.Json
import java.io.File

class DataManager(ctx: Context) {
    companion object {
        private const val filename = "save.vbg"
        private val fileDecoding = Charsets.UTF_8
        private lateinit var file: File
        private var loaded = false
        private val storage = Firebase.storage

        private val storagePath: StorageReference?
            get() {
                val cUser = AuthManager.currentUser ?: return null
                //todo: Remove _ in filename below
                return storage.reference.child("Users/${cUser.uid}/_$filename")
            }

        private fun load() {
            try {
                //todo: Try download online file

                val content = file.readLines(fileDecoding)
                if (content.isNotEmpty() && content[0].startsWith("@@@")) {
                    val totalContent = content.joinToString("\n")
                    val coinsString = Regex("int///munten:::[0-9]+,,,").find(totalContent)?.value
                    val coins = coinsString?.split(",,,")?.get(0)?.substring(15)?.toInt()
                    if (coins != null) {
                        MenuManager.coins = coins
                        save()
                    }
                    return
                }
                for (line in content) {
                    try {
                        if (line.isEmpty()) continue
                        val i = line.indexOf('%')
                        if (i == -1) continue
                        val safeCode = line.substring(0, i).toInt()
                        val json = line.substring(i + 1)
                        when (SceneManager.scenes[safeCode]) {
                            SceneManager.Scene.SUDOKU -> {
                                val data = Json.decodeFromString<SudokuData>(json)
                                SudokuManager.loadFromFile(data)
                            }

                            SceneManager.Scene.SOLITAIRE -> {
                                val data = Json.decodeFromString<SolitaireData>(json)
                                SolitaireManager.loadFromFile(data)
                            }

                            SceneManager.Scene.MINESWEEPER -> {
                                val data = Json.decodeFromString<MinesweeperData>(json)
                                MinesweeperManager.loadFromFile(data)
                            }

                            SceneManager.Scene.MENU -> {
                                val data = Json.decodeFromString<MenuData>(json)
                                MenuManager.loadFromFile(data)
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
        }

        fun save(withPublish: Boolean = true) {
            val str: StringBuilder = StringBuilder()
            for (entry in SceneManager.scenes) {
                val dataString = when (entry.value) {
                    SceneManager.Scene.SUDOKU -> SudokuManager.saveToFile()
                    SceneManager.Scene.SOLITAIRE -> SolitaireManager.saveToFile()
                    SceneManager.Scene.MINESWEEPER -> MinesweeperManager.saveToFile()
                    SceneManager.Scene.MENU -> MenuManager.saveToFile()
                }
                str.append("\n${entry.key}%$dataString")
            }
            file.writeText(str.toString().trim(), fileDecoding)
            if (withPublish) storagePath?.putStream(file.inputStream())
        }
    }

    init {
        file = File(ctx.filesDir, filename)
        file.createNewFile()
        if (!loaded) load()
        MainActivity.scope.launch {
            autoSave(0)
        }
    }

    private suspend fun autoSave(i: Int) {
        withTimeout(60000) {
            Logger.d("Save ${if (i == 5) "with publish" else ""}")
            save(i == 5)
            autoSave(i + 1)
        }
    }
}