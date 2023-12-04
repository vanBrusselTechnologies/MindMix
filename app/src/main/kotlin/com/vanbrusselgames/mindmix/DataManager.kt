package com.vanbrusselgames.mindmix

import android.content.Context
import android.util.Log
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

        private enum class DataType { Sudoku, Solitaire, MineSweeper }

        private val safeCodeMap =
            mapOf(0 to DataType.Sudoku, 1 to DataType.Solitaire, 2 to DataType.MineSweeper)

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
                        when (safeCodeMap[safeCode]) {
                            DataType.Sudoku -> {
                                val data = Json.decodeFromString<SudokuData>(json)
                                SudokuManager.loadFromFile(data)
                            }

                            DataType.Solitaire -> {
                                val data = Json.decodeFromString<SolitaireData>(json)
                                SolitaireManager.loadFromFile(data)
                            }

                            DataType.MineSweeper -> {
                                val data = Json.decodeFromString<MinesweeperData>(json)
                                MinesweeperManager.loadFromFile(data)
                            }

                            else -> {}
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
            for (entry in safeCodeMap) {
                val dataString = when (entry.value) {
                    DataType.Sudoku -> SudokuManager.saveToFile()
                    DataType.Solitaire -> SolitaireManager.saveToFile()
                    DataType.MineSweeper -> MinesweeperManager.saveToFile()
                    else -> null
                } ?: continue
                str.append("\n${entry.key}%$dataString")
            }
            file.writeText(str.toString().trim(), fileDecoding)
        }
    }
}