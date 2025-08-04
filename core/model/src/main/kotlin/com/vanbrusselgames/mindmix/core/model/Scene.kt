package com.vanbrusselgames.mindmix.core.model

sealed interface Scene {
    val sceneId: Int
    val name: String
}

data class GameScene(
    override val sceneId: Int, val gameId: Int, override val name: String
) : Scene

data class StaticScene(
    override val sceneId: Int, override val name: String
) : Scene