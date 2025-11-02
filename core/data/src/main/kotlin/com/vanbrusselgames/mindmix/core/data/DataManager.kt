package com.vanbrusselgames.mindmix.core.data

import android.content.Context
import android.os.Handler
import android.os.Looper
import com.google.firebase.Firebase
import com.google.firebase.auth.auth
import com.google.firebase.storage.StorageReference
import com.google.firebase.storage.storage
import com.vanbrusselgames.mindmix.core.logging.Logger
import com.vanbrusselgames.mindmix.core.model.Scene
import com.vanbrusselgames.mindmix.core.model.SceneRegistry
import dagger.hilt.android.qualifiers.ApplicationContext
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import java.io.File
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class DataManager @Inject constructor(@ApplicationContext ctx: Context) {
    private val fileName = "save.vbg"
    private val fileDecoding = Charsets.UTF_8
    private val file: File = File(ctx.filesDir, fileName)
    private var loaded = false
    private var isAutoSaving = false
    private var hasUpdate = false

    private val dataMap =
        SceneRegistry.allScenes.map { it.sceneId }.associateWith { "" }.toMutableMap()

    private val storagePath: StorageReference?
        get() = Firebase.auth.currentUser?.uid.let {
            return when (it) {
                null -> null
                "" -> null
                else -> Firebase.storage.reference.child("Users/${it}/$fileName")
            }
        }

    init {
        file.createNewFile()
    }

    fun initialLoad() {
        if (!loaded) load()
        autoSave()
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

    private fun load() {
        try {
            // TODO: Download online file from firebase
            val content = file.readLines(fileDecoding)
            for (line in content) {
                try {
                    if (line.isEmpty()) continue
                    val i = line.indexOf('%')
                    if (i == -1) continue
                    val safeCode = line.take(i).toInt()
                    val json = line.substring(i + 1)
                    if (dataMap.containsKey(safeCode) && dataMap[safeCode] != "") continue
                    dataMap[safeCode] = json
                } catch (e: Exception) {
                    Logger.e("Error reading line of save file", e)
                }
            }
        } catch (e: Exception) {
            Logger.e("Error loading save file", e)
        }
        loaded = true
    }

    fun saveScene(game: Scene, data: String) {
        dataMap[game.sceneId] = data
        hasUpdate = true
    }

    fun save(withPublish: Boolean = true) {
        if (!loaded || !hasUpdate) return
        CoroutineScope(Dispatchers.IO).launch {
            val sb = StringBuilder()
            dataMap.forEach {
                if (it.value.trim().isNotEmpty()) sb.append("\n${it.key}%${it.value}")
            }
            if (sb.isEmpty()) return@launch
            file.writeText(sb.toString().trim(), fileDecoding)
            sb.clear()
            hasUpdate = false
            if (withPublish) {
                storagePath?.putStream(file.inputStream())
                file.inputStream().close()
            }
        }
    }

    fun getSavedDataForScene(scene: Scene): String {
        return dataMap[scene.sceneId]!!
    }
}