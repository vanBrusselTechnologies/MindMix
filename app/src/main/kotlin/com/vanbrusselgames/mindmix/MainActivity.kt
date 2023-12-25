package com.vanbrusselgames.mindmix

import android.os.Build
import android.os.Bundle
import android.view.WindowInsets
import android.view.WindowInsetsController
import android.view.WindowManager
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Surface
import androidx.compose.ui.Modifier
import androidx.core.view.WindowCompat
import androidx.core.view.WindowInsetsCompat
import androidx.core.view.WindowInsetsControllerCompat
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import androidx.navigation.navArgument
import com.vanbrusselgames.mindmix.menu.MenuLayout
import com.vanbrusselgames.mindmix.minesweeper.MinesweeperLayout
import com.vanbrusselgames.mindmix.minesweeper.MinesweeperManager
import com.vanbrusselgames.mindmix.solitaire.SolitaireLayout
import com.vanbrusselgames.mindmix.solitaire.SolitaireManager
import com.vanbrusselgames.mindmix.sudoku.SudokuLayout
import com.vanbrusselgames.mindmix.sudoku.SudokuManager
import com.vanbrusselgames.mindmix.ui.theme.MindMixTheme


class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        AuthManager.start(this)

        PixelHelper.setResources(resources)
        setFullScreen(actionBar, window)
        DataManager.init(applicationContext)
        DataManager.load()

        setContent {
            MindMixTheme {
                // A surface container using the 'background' color from the theme
                Surface(
                    modifier = Modifier.fillMaxSize(),
                    color = MaterialTheme.colorScheme.background,
                    contentColor = MaterialTheme.colorScheme.onBackground
                ) {
                    SceneManager.navController = rememberNavController()
                    NavHost(navController = SceneManager.navController, startDestination = "main", Modifier.fillMaxSize()) {
                        composable("main") { MenuLayout().BaseScene() }
                        composable("menu") { MenuLayout().BaseScene() }
                        composable(
                            "solitaire?mode={mode}",
                            arguments = listOf(navArgument("mode") { defaultValue = "0" })
                        ) { backStackEntry ->
                            val mode = backStackEntry.arguments?.getString("mode")
                            SolitaireManager.loadPuzzle()
                            SolitaireLayout().BaseScene()
                        }
                        composable(
                            "sudoku?mode={mode}",
                            arguments = listOf(navArgument("mode") { defaultValue = "0" })
                        ) { backStackEntry ->
                            val mode = backStackEntry.arguments?.getString("mode")
                            SudokuManager.loadPuzzle()
                            SudokuLayout().BaseScene()
                        }
                        composable(
                            "minesweeper?mode={mode}",
                            arguments = listOf(navArgument("mode") { defaultValue = "0" })
                        ) { backStackEntry ->
                            val mode = backStackEntry.arguments?.getString("mode")
                            MinesweeperManager.loadPuzzle()
                            MinesweeperLayout().BaseScene()
                        }
                    }
                }
            }
        }
    }
}

fun setFullScreen(actionBar: android.app.ActionBar?, window: android.view.Window) {
    actionBar?.hide()
    WindowCompat.setDecorFitsSystemWindows(window, false)
    if (Build.VERSION.SDK_INT < Build.VERSION_CODES.R) {
        window.addFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN)
    } else {
        window.insetsController?.apply {
            hide(WindowInsets.Type.statusBars())
            systemBarsBehavior = WindowInsetsController.BEHAVIOR_SHOW_TRANSIENT_BARS_BY_SWIPE
        }
    }
    WindowInsetsControllerCompat(
        window, window.decorView
    ).let { controller ->
        controller.hide(WindowInsetsCompat.Type.systemBars())
        controller.systemBarsBehavior =
            WindowInsetsControllerCompat.BEHAVIOR_SHOW_TRANSIENT_BARS_BY_SWIPE
    }
}