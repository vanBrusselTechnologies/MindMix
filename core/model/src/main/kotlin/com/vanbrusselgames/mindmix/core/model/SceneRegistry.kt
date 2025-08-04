package com.vanbrusselgames.mindmix.core.model

object SceneRegistry {
    val Sudoku = GameScene(sceneId = 0, gameId = 0, name = "sudoku")
    val Solitaire = GameScene(sceneId = 1, gameId = 1, name = "solitaire")
    val Minesweeper = GameScene(sceneId = 2, gameId = 2, name = "minesweeper")
    val Game2048 = GameScene(sceneId = 4, gameId = 3, name = "2048")

    val Menu = StaticScene(sceneId = 3, name = "menu")

    val allScenes: List<Scene> = listOf(
        Sudoku,
        Solitaire,
        Minesweeper,
        Game2048,
        Menu,
    )
}