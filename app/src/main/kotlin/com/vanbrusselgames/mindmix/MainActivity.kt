package com.vanbrusselgames.mindmix

import android.os.Build
import android.os.Bundle
import android.view.WindowInsets
import android.view.WindowInsetsController
import android.view.WindowManager
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.result.ActivityResult
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.animation.fadeIn
import androidx.compose.animation.fadeOut
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.SnackbarHostState
import androidx.compose.material3.Surface
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.ui.Modifier
import androidx.core.view.WindowCompat
import androidx.core.view.WindowInsetsCompat
import androidx.core.view.WindowInsetsControllerCompat
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import androidx.navigation.navArgument
import com.google.firebase.Firebase
import com.google.firebase.appcheck.appCheck
import com.google.firebase.appcheck.playintegrity.PlayIntegrityAppCheckProviderFactory
import com.google.firebase.initialize
import com.vanbrusselgames.mindmix.menu.MenuLayout
import com.vanbrusselgames.mindmix.minesweeper.MinesweeperLayout
import com.vanbrusselgames.mindmix.minesweeper.MinesweeperManager
import com.vanbrusselgames.mindmix.solitaire.SolitaireLayout
import com.vanbrusselgames.mindmix.solitaire.SolitaireManager
import com.vanbrusselgames.mindmix.sudoku.SudokuLayout
import com.vanbrusselgames.mindmix.sudoku.SudokuManager
import com.vanbrusselgames.mindmix.ui.theme.MindMixTheme
import kotlinx.coroutines.CoroutineScope

class MainActivity : ComponentActivity() {
    companion object {
        lateinit var networkMonitor: NetworkMonitor
        lateinit var adManager: AdManager
        lateinit var dataManager: DataManager

        lateinit var snackbarHostState: SnackbarHostState
        lateinit var scope: CoroutineScope
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        Logger.start(this)
        Firebase.initialize(this)
        Firebase.appCheck.installAppCheckProviderFactory(
            PlayIntegrityAppCheckProviderFactory.getInstance(),
        )
        AuthManager.start(this)
        ReviewManager.start(this)
        UpdateManager.start(this,
            registerForActivityResult(ActivityResultContracts.StartIntentSenderForResult()) { result: ActivityResult ->
                if (result.resultCode != RESULT_OK) {
                    Logger.w("Update flow failed! Result code: " + result.resultCode)
                }
            })

        networkMonitor = NetworkMonitor(this)
        adManager = AdManager(this)
        dataManager = DataManager(applicationContext)

        PixelHelper.setResources(resources)
        setFullScreen(actionBar, window)

        setContent {
            MindMixTheme {
                scope = rememberCoroutineScope()
                snackbarHostState = remember { SnackbarHostState() }
                // A surface container using the 'background' color from the theme
                Surface(
                    modifier = Modifier.fillMaxSize(),
                    color = MaterialTheme.colorScheme.background,
                    contentColor = MaterialTheme.colorScheme.onBackground
                ) {
                    SceneManager.navController = rememberNavController()
                    NavHost(navController = SceneManager.navController,
                        startDestination = "main",
                        Modifier.fillMaxSize(),
                        enterTransition = { fadeIn() },
                        exitTransition = { fadeOut() }) {
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

    override fun onPause() {
        super.onPause()
        DataManager.save()
    }

    override fun onWindowFocusChanged(hasFocus: Boolean) {
        super.onWindowFocusChanged(hasFocus)
        if (!hasFocus) DataManager.save()
    }

    private fun setFullScreen(actionBar: android.app.ActionBar?, window: android.view.Window) {
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
}