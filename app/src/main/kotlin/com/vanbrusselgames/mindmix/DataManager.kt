package com.vanbrusselgames.mindmix

import android.content.Context
import android.util.Log
import com.vanbrusselgames.mindmix.menu.MenuData
import com.vanbrusselgames.mindmix.menu.MenuManager
import com.vanbrusselgames.mindmix.minesweeper.MinesweeperData
import com.vanbrusselgames.mindmix.minesweeper.MinesweeperManager
import com.vanbrusselgames.mindmix.solitaire.SolitaireData
import com.vanbrusselgames.mindmix.solitaire.SolitaireManager
import com.vanbrusselgames.mindmix.sudoku.SudokuData
import com.vanbrusselgames.mindmix.sudoku.SudokuManager
import kotlinx.serialization.json.Json
import java.io.File

class DataManager {
    companion object {
        private const val filename = "save.vbg"
        private val fileDecoding = Charsets.UTF_8
        private lateinit var file: File

        fun init(ctx: Context) {
            file = File(ctx.filesDir, filename)
            file.createNewFile()
        }

        fun load() {
            try {
                val content = file.readLines(fileDecoding)
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
                        Log.e("MindMix", "Error reading line of safe file: ${e.message}")
                        Log.e("MindMix", e.toString())
                    }
                }
            } catch (e: Exception) {
                Log.e("MindMix", "Error loading file: ${e.message}")
                Log.e("MindMix", e.toString())
            }
        }

        fun save() {
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
        }
    }
}