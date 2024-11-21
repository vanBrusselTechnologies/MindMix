package com.vanbrusselgames.mindmix.core.data

import android.content.Context
import android.os.Handler
import android.os.Looper
import com.google.firebase.Firebase
import com.google.firebase.storage.StorageReference
import com.google.firebase.storage.storage
import com.vanbrusselgames.mindmix.core.logging.Logger
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.core.navigation.SceneManager.Scene
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.runBlocking
import kotlinx.serialization.json.Json
import java.io.File

class DataManager(
    ctx: Context,
    val userId: () -> String,
    loadDataForScene: (jsonParser: Json, Scene, String) -> Unit,
    val saveSceneData: (dataManager: DataManager) -> Unit
) {
    companion object {
        private const val FILE_NAME = "save.vbg"
        private val fileDecoding = Charsets.UTF_8
        private lateinit var file: File
        private var loaded = false
        private val storage = Firebase.storage
        private var isAutoSaving = false
        private val jsonParser = Json { ignoreUnknownKeys = true }

        private val dataMap = mutableMapOf(
            Scene.SUDOKU to "",
            Scene.SOLITAIRE to "",
            Scene.MINESWEEPER to "",
            Scene.MENU to "",
            Scene.GAME2048 to ""
        )
    }

    private val storagePath: StorageReference?
        get() = userId().let {
            return when (it) {
                "" -> null
                else -> storage.reference.child("Users/${it}/$FILE_NAME")
            }
        }


    private fun load(loadDataForScene: (jsonParser: Json, Scene, String) -> Unit) {
        try {
            //TODO: Download online file from firebase
            val content = file.readLines(fileDecoding)
            for (line in content) {
                try {
                    if (line.isEmpty()) continue
                    val i = line.indexOf('%')
                    if (i == -1) continue
                    val safeCode = line.substring(0, i).toInt()
                    val json = line.substring(i + 1)
                    val saveScene = SceneManager.scenes[safeCode]
                    if (saveScene == null) continue
                    dataMap[saveScene] = json
                    loadDataForScene(jsonParser, saveScene, json)
                } catch (e: Exception) {
                    Logger.e("Error reading line of save file", e)
                }
            }
        } catch (e: Exception) {
            Logger.e("Error loading save file", e)
        }
        loaded = true
    }

    fun saveScene(scene: Scene, data: String) {
        dataMap[scene] = data
    }

    fun save(withPublish: Boolean = true) {
        if (!loaded) return
        CoroutineScope(Dispatchers.IO).launch {
            val str: StringBuilder = StringBuilder()
            saveSceneData(this@DataManager)
            for (entry in SceneManager.scenes) str.append("\n${entry.key}%${dataMap[entry.value]}")
            file.writeText(str.toString().trim(), fileDecoding)
            if (withPublish) {
                storagePath?.putStream(file.inputStream())
                file.inputStream().close()
            }
        }
    }

    init {
        file = File(ctx.filesDir, FILE_NAME)
        file.createNewFile()
        runBlocking(Dispatchers.IO) {
            if (!loaded) load(loadDataForScene)
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
                mainHandler.postDelayed(this, 5000)
                save(autoSaveCount++ % 12 == 0)
            }
        })
    }
}