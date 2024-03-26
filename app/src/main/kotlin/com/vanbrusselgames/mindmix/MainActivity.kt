package com.vanbrusselgames.mindmix

import android.content.Context
import android.os.Build
import android.os.Bundle
import android.view.WindowInsets
import android.view.WindowInsetsController
import android.view.WindowManager
import androidx.activity.ComponentActivity
import androidx.activity.compose.BackHandler
import androidx.activity.result.ActivityResult
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.animation.fadeIn
import androidx.compose.animation.fadeOut
import androidx.compose.foundation.isSystemInDarkTheme
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.SnackbarHostState
import androidx.compose.material3.Surface
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.ComposeView
import androidx.core.view.WindowCompat
import androidx.core.view.WindowInsetsCompat
import androidx.core.view.WindowInsetsControllerCompat
import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.preferencesDataStore
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import androidx.navigation.navArgument
import com.google.firebase.Firebase
import com.google.firebase.appcheck.appCheck
import com.google.firebase.appcheck.playintegrity.PlayIntegrityAppCheckProviderFactory
import com.google.firebase.initialize
import com.vanbrusselgames.mindmix.games.GameHelp
import com.vanbrusselgames.mindmix.games.GameLoader
import com.vanbrusselgames.mindmix.games.GameMenu
import com.vanbrusselgames.mindmix.games.GameTimer
import com.vanbrusselgames.mindmix.games.minesweeper.MinesweeperLayout
import com.vanbrusselgames.mindmix.games.solitaire.SolitaireLayout
import com.vanbrusselgames.mindmix.games.sudoku.SudokuLayout
import com.vanbrusselgames.mindmix.menu.MenuLayout
import com.vanbrusselgames.mindmix.menu.MenuManager
import com.vanbrusselgames.mindmix.ui.theme.MindMixTheme
import com.vanbrusselgames.mindmix.ui.theme.SelectedTheme
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch

val Context.dataStore: DataStore<Preferences> by preferencesDataStore(name = "settings")

class MainActivity : ComponentActivity() {
    companion object {
        lateinit var networkMonitor: NetworkMonitor
        lateinit var adManager: AdManager

        lateinit var snackbarHostState: SnackbarHostState
        lateinit var scope: CoroutineScope
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        Logger(this)
        networkMonitor = NetworkMonitor(this@MainActivity)

        PixelHelper.setResources(resources)
        setFullScreen(actionBar, window)
        CoroutineScope(Dispatchers.IO).launch {
            Firebase.initialize(this@MainActivity)
            Firebase.appCheck.installAppCheckProviderFactory(
                PlayIntegrityAppCheckProviderFactory.getInstance()
            )
            AuthManager.start(this@MainActivity)

            GameLoader.init()
        }

        loadPreferences(applicationContext.dataStore)
        DataManager(this)

        setContentView(ComposeView(this).apply {
            setContent {
                val darkTheme = when (MenuManager.theme.value) {
                    SelectedTheme.System -> isSystemInDarkTheme()
                    SelectedTheme.Dark -> true
                    SelectedTheme.Light -> false
                }
                MindMixTheme(darkTheme) {
                    scope = rememberCoroutineScope()
                    snackbarHostState = remember { SnackbarHostState() }
                    // A surface container using the 'background' color from the theme
                    Surface(
                        modifier = Modifier.fillMaxSize(),
                        color = MaterialTheme.colorScheme.background,
                        contentColor = MaterialTheme.colorScheme.onBackground
                    ) {
                        SceneManager.navController = rememberNavController()
                        SceneManager.navController.enableOnBackPressed(false)
                        NavHost(navController = SceneManager.navController,
                            startDestination = "main",
                            Modifier.fillMaxSize(),
                            enterTransition = { fadeIn() },
                            exitTransition = { fadeOut() }) {
                            composable("main") {
                                MenuLayout().Scene()
                            }
                            composable("menu") {
                                MenuLayout().Scene()
                            }
                            composable(
                                "solitaire?mode={mode}",
                                arguments = listOf(navArgument("mode") { defaultValue = "0" })
                            ) {
                                val mode = it.arguments?.getString("mode")
                                SolitaireLayout().Scene()
                            }
                            composable(
                                "sudoku?mode={mode}",
                                arguments = listOf(navArgument("mode") { defaultValue = "0" })
                            ) {
                                val mode = it.arguments?.getString("mode")
                                SudokuLayout().Scene()
                            }
                            composable(
                                "minesweeper?mode={mode}",
                                arguments = listOf(navArgument("mode") { defaultValue = "0" })
                            ) {
                                val mode = it.arguments?.getString("mode")
                                MinesweeperLayout().Scene()
                            }
                        }
                    }
                    BackHandler {
                        if (SceneManager.currentScene != SceneManager.Scene.MENU) {
                            if (BaseLayout.disableTopRowButtons.value) {
                                disableUI()
                            } else {
                                onLoseFocus()
                            }
                        }
                    }
                }
            }
        })

        UpdateManager.start(this@MainActivity,
            registerForActivityResult(ActivityResultContracts.StartIntentSenderForResult()) { result: ActivityResult ->
                if (result.resultCode != RESULT_OK) {
                    Logger.w("Update flow failed! Result code: " + result.resultCode)
                }
            })

        CoroutineScope(Dispatchers.Main).launch {
            adManager = AdManager(this@MainActivity)
            ReviewManager.start(this@MainActivity)
        }
    }

    override fun onPause() {
        super.onPause()
        onLoseFocus()
    }

    override fun onWindowFocusChanged(hasFocus: Boolean) {
        super.onWindowFocusChanged(hasFocus)
        if (!hasFocus) {
            onLoseFocus()
        }
    }

    private fun onLoseFocus() {
        GameTimer.pauseAll()
        DataManager.save()
        if (!BaseLayout.disableTopRowButtons.value && SceneManager.currentScene != SceneManager.Scene.MENU) {
            GameMenu.visible.value = true
            BaseLayout.disableTopRowButtons.value = true
        }
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

    private fun disableUI() {
        if (Settings.visible.value || GameMenu.visible.value || GameHelp.visible.value) {
            BaseLayout.disableTopRowButtons.value = false
        } // else: GameFinished visible, so don't enable top row buttons
        // todo: Rewrite to if(GameFinished.visible.value){}
        Settings.visible.value = false
        GameMenu.visible.value = false
        GameHelp.visible.value = false
    }
}