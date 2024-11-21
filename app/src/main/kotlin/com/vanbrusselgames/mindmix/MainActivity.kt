package com.vanbrusselgames.mindmix

import android.os.Bundle
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
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.ComposeView
import androidx.navigation.NavHostController
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import com.google.firebase.Firebase
import com.google.firebase.appcheck.appCheck
import com.google.firebase.appcheck.playintegrity.PlayIntegrityAppCheckProviderFactory
import com.google.firebase.initialize
import com.vanbrusselgames.mindmix.core.advertisement.AdManager
import com.vanbrusselgames.mindmix.core.authentication.AuthManager
import com.vanbrusselgames.mindmix.core.common.BaseGameViewModel
import com.vanbrusselgames.mindmix.core.common.BaseScreenViewModel
import com.vanbrusselgames.mindmix.core.common.GameLoader
import com.vanbrusselgames.mindmix.core.common.NetworkMonitor
import com.vanbrusselgames.mindmix.core.data.DataManager
import com.vanbrusselgames.mindmix.core.data.dataStore
import com.vanbrusselgames.mindmix.core.data.loadPreferences
import com.vanbrusselgames.mindmix.core.designsystem.theme.MindMixTheme
import com.vanbrusselgames.mindmix.core.designsystem.theme.SelectedTheme
import com.vanbrusselgames.mindmix.core.logging.Logger
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.core.navigation.SceneManager.Scene
import com.vanbrusselgames.mindmix.feature.gamefinished.navigation.gameFinishedDialog
import com.vanbrusselgames.mindmix.feature.gamehelp.navigation.gameHelpDialog
import com.vanbrusselgames.mindmix.feature.gamemenu.navigation.gameMenuDialog
import com.vanbrusselgames.mindmix.feature.gamemenu.navigation.navigateToGameMenu
import com.vanbrusselgames.mindmix.feature.menu.MenuSettings
import com.vanbrusselgames.mindmix.feature.menu.navigation.MenuRoute
import com.vanbrusselgames.mindmix.feature.menu.navigation.menu
import com.vanbrusselgames.mindmix.feature.menu.navigation.navigateToMenu
import com.vanbrusselgames.mindmix.feature.settings.navigation.navigateToSettings
import com.vanbrusselgames.mindmix.feature.settings.navigation.settingsDialog
import com.vanbrusselgames.mindmix.games.game2048.Game2048GameFinishedDialog
import com.vanbrusselgames.mindmix.games.game2048.Game2048Settings
import com.vanbrusselgames.mindmix.games.game2048.navigation.game2048
import com.vanbrusselgames.mindmix.games.minesweeper.MinesweeperGameFinishedDialog
import com.vanbrusselgames.mindmix.games.minesweeper.MinesweeperSettings
import com.vanbrusselgames.mindmix.games.minesweeper.navigation.minesweeper
import com.vanbrusselgames.mindmix.games.solitaire.SolitaireGameFinishedDialog
import com.vanbrusselgames.mindmix.games.solitaire.SolitaireSettings
import com.vanbrusselgames.mindmix.games.solitaire.navigation.solitaire
import com.vanbrusselgames.mindmix.games.sudoku.SudokuGameFinishedDialog
import com.vanbrusselgames.mindmix.games.sudoku.SudokuSettings
import com.vanbrusselgames.mindmix.games.sudoku.navigation.sudoku
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch

class MainActivity : ComponentActivity() {
    lateinit var dataManager: DataManager

    fun getViewModelByScene(scene: Scene): BaseScreenViewModel {
        return when (scene) {
            Scene.SUDOKU -> sudoku.viewModel
            Scene.GAME2048 -> game2048.viewModel
            Scene.MENU -> menu.viewModel
            Scene.MINESWEEPER -> minesweeper.viewModel
            Scene.SOLITAIRE -> solitaire.viewModel
        }
    }

    val snackbarHostState = SnackbarHostState()

    lateinit var navController: NavHostController

    fun currentViewModel(): BaseScreenViewModel {
        return getViewModelByScene(SceneManager.currentScene)
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        Firebase.initialize(this)
        Firebase.appCheck.installAppCheckProviderFactory(PlayIntegrityAppCheckProviderFactory.getInstance())

        val networkMonitor = NetworkMonitor(this)
        val authManager = AuthManager(this)
        val adManager = AdManager(this, networkMonitor)
        dataManager =
            DataManager(this, { authManager.userId.value }, loadDataForScene, saveSceneData)
        loadPreferences(applicationContext.dataStore, onLoadPreferences)
        CoroutineScope(Dispatchers.IO).launch {
            GameLoader.init(this@MainActivity, networkMonitor, gameLoad)
        }
        val updateManager = UpdateManager(this, snackbarHostState, dataManager)
        setContentView(ComposeView(this).apply {
            setContent {
                val darkTheme = when (menu.viewModel.theme.value) {
                    SelectedTheme.System -> isSystemInDarkTheme()
                    SelectedTheme.Dark -> true
                    SelectedTheme.Light -> false
                }
                navController = rememberNavController()
                navController.enableOnBackPressed(false)
                navController.addOnDestinationChangedListener(SceneManager().onDestinationChange)
                MindMixTheme(darkTheme) {
                    Surface(
                        modifier = Modifier.fillMaxSize(),
                        color = MaterialTheme.colorScheme.background,
                        contentColor = MaterialTheme.colorScheme.onBackground
                    ) {
                        NavHost(navController = navController,
                            startDestination = MenuRoute,
                            Modifier.fillMaxSize(),
                            enterTransition = { fadeIn() },
                            exitTransition = { fadeOut() }) {
                            menu(navController, menu.viewModel, snackbarHostState)
                            solitaire(navController, solitaire.viewModel, snackbarHostState)
                            sudoku(navController, sudoku.viewModel, snackbarHostState)
                            minesweeper(navController, minesweeper.viewModel, snackbarHostState)
                            game2048(navController, game2048.viewModel, snackbarHostState)
                            gameMenuDialog(navController,
                                { currentViewModel().nameResId },
                                { (currentViewModel() as BaseGameViewModel).startNewGame() },
                                { navController.navigateToSettings() }) { navController.navigateToMenu() }
                            gameHelpDialog(
                                navController,
                                { currentViewModel().nameResId }) { (currentViewModel() as BaseGameViewModel).descResId }
                            gameFinishedDialog {
                                when (SceneManager.currentScene) {
                                    Scene.GAME2048 -> Game2048GameFinishedDialog(navController,
                                        game2048.viewModel,
                                        { adManager },
                                        { navController.navigateToMenu() }) { dataManager.save() }

                                    Scene.MINESWEEPER -> MinesweeperGameFinishedDialog(navController,
                                        minesweeper.viewModel,
                                        { adManager },
                                        { navController.navigateToMenu() }) { dataManager.save() }

                                    Scene.SOLITAIRE -> SolitaireGameFinishedDialog(navController,
                                        solitaire.viewModel,
                                        { adManager },
                                        { navController.navigateToMenu() }) { dataManager.save() }

                                    Scene.SUDOKU -> SudokuGameFinishedDialog(navController,
                                        sudoku.viewModel,
                                        { adManager },
                                        { navController.navigateToMenu() }) { dataManager.save() }

                                    Scene.MENU -> {}
                                }
                            }
                            settingsDialog(navController) {
                                val settingsScene =
                                    if (SceneManager.currentScene == Scene.MENU) menu.viewModel.settingsGame else SceneManager.currentScene
                                menu.viewModel.settingsGame = Scene.MENU
                                when (settingsScene) {
                                    Scene.GAME2048 -> Game2048Settings(game2048.viewModel)

                                    Scene.MENU -> MenuSettings(context, menu.viewModel, authManager)

                                    Scene.MINESWEEPER -> MinesweeperSettings(
                                        context, minesweeper.viewModel
                                    )

                                    Scene.SOLITAIRE -> SolitaireSettings(
                                        context, solitaire.viewModel
                                    )

                                    Scene.SUDOKU -> SudokuSettings(context, sudoku.viewModel)
                                }
                            }
                        }
                    }
                    BackHandler { openGameMenu() }
                }
            }
        })

        updateManager.checkForUpdates(registerForActivityResult(ActivityResultContracts.StartIntentSenderForResult()) { result: ActivityResult ->
            if (result.resultCode != RESULT_OK) {
                Logger.w("Update flow failed! Result code: " + result.resultCode)
            }
        })

        CoroutineScope(Dispatchers.Main).launch {
            ReviewManager.start(this@MainActivity)
        }
    }

    override fun onPause() {
        super.onPause()
        onLoseFocus()
    }

    override fun onWindowFocusChanged(hasFocus: Boolean) {
        super.onWindowFocusChanged(hasFocus)
        if (!hasFocus) onLoseFocus()
    }

    private fun onLoseFocus() {
        dataManager.save()
        openGameMenu()
    }

    private fun openGameMenu() {
        if (!SceneManager.dialogActiveState.value && SceneManager.currentScene != Scene.MENU) {
            navController.navigateToGameMenu()
            currentViewModel().onOpenDialog()
        }
    }
}