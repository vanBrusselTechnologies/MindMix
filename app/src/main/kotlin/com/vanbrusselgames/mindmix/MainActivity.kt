package com.vanbrusselgames.mindmix

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.BackHandler
import androidx.activity.result.ActivityResult
import androidx.activity.result.contract.ActivityResultContracts
import androidx.activity.viewModels
import androidx.compose.animation.fadeIn
import androidx.compose.animation.fadeOut
import androidx.compose.foundation.isSystemInDarkTheme
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.safeDrawingPadding
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Scaffold
import androidx.compose.material3.SnackbarHost
import androidx.compose.material3.SnackbarHostState
import androidx.compose.material3.Surface
import androidx.compose.runtime.State
import androidx.compose.runtime.mutableStateOf
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.ComposeView
import androidx.core.splashscreen.SplashScreen.Companion.installSplashScreen
import androidx.lifecycle.lifecycleScope
import androidx.navigation.NavHostController
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.rememberNavController
import com.google.firebase.Firebase
import com.google.firebase.appcheck.appCheck
import com.google.firebase.appcheck.playintegrity.PlayIntegrityAppCheckProviderFactory
import com.google.firebase.initialize
import com.vanbrusselgames.mindmix.core.advertisement.AdManager
import com.vanbrusselgames.mindmix.core.authentication.AuthManager
import com.vanbrusselgames.mindmix.core.common.BaseScreenViewModel
import com.vanbrusselgames.mindmix.core.data.DataManager
import com.vanbrusselgames.mindmix.core.designsystem.theme.MindMixTheme
import com.vanbrusselgames.mindmix.core.designsystem.theme.SelectedTheme
import com.vanbrusselgames.mindmix.core.logging.Logger
import com.vanbrusselgames.mindmix.core.model.SceneRegistry
import com.vanbrusselgames.mindmix.core.navigation.AppRoutes
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.feature.menu.MenuScreenViewModel
import com.vanbrusselgames.mindmix.feature.menu.MenuSettings
import com.vanbrusselgames.mindmix.feature.menu.navigation.menu
import com.vanbrusselgames.mindmix.feature.settings.navigation.settingsDialog
import com.vanbrusselgames.mindmix.games.game2048.navigation.game2048
import com.vanbrusselgames.mindmix.games.game2048.navigation.navigateToGame2048GameMenu
import com.vanbrusselgames.mindmix.games.minesweeper.navigation.minesweeper
import com.vanbrusselgames.mindmix.games.minesweeper.navigation.navigateToMinesweeperGameMenu
import com.vanbrusselgames.mindmix.games.solitaire.navigation.navigateToSolitaireGameMenu
import com.vanbrusselgames.mindmix.games.solitaire.navigation.solitaire
import com.vanbrusselgames.mindmix.games.sudoku.navigation.navigateToSudokuGameMenu
import com.vanbrusselgames.mindmix.games.sudoku.navigation.sudoku
import dagger.hilt.android.AndroidEntryPoint
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import javax.inject.Inject

@AndroidEntryPoint
class MainActivity : ComponentActivity() {
    @Inject
    lateinit var adManager: AdManager

    @Inject
    lateinit var authManager: AuthManager

    @Inject
    lateinit var dataManager: DataManager

    @Inject
    lateinit var reviewManager: ReviewManager

    @Inject
    lateinit var updateManager: UpdateManager

    private val menuScreenViewModel by viewModels<MenuScreenViewModel>()

    private val _currentViewModel = mutableStateOf<BaseScreenViewModel?>(null)
    val currentViewModel: State<BaseScreenViewModel?> = _currentViewModel

    fun setCurrentViewModel(viewModel: BaseScreenViewModel?) {
        _currentViewModel.value = viewModel
    }

    private val snackbarHostState = SnackbarHostState()

    lateinit var navController: NavHostController

    override fun onCreate(savedInstanceState: Bundle?) {
        val splashscreen = installSplashScreen()
        var keepSplashScreen = true
        super.onCreate(savedInstanceState)

        Firebase.initialize(this)
        Firebase.appCheck.installAppCheckProviderFactory(PlayIntegrityAppCheckProviderFactory.getInstance())

        authManager.initialAuthCheck(this)
        adManager.initialize(this)

        updateManager.checkForUpdates(registerForActivityResult(ActivityResultContracts.StartIntentSenderForResult()) { result: ActivityResult ->
            if (result.resultCode != RESULT_OK) Logger.w("Update flow failed! Result code: " + result.resultCode)
        }, snackbarHostState) { dataManager.save(false) }

        lifecycleScope.launch(Dispatchers.IO) {
            dataManager.initialLoad()

            withContext(Dispatchers.Main) {
                Logger.d("Finished initial load!")
                keepSplashScreen = false
            }
        }
        splashscreen.setKeepOnScreenCondition { keepSplashScreen }
        setContentView(ComposeView(this).apply {
            setContent {
                val darkTheme = when (menuScreenViewModel.theme.value) {
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
                        Scaffold(Modifier.safeDrawingPadding(), snackbarHost = {
                            SnackbarHost(hostState = snackbarHostState)
                        }) {
                            NavHost(
                                navController,
                                startDestination = AppRoutes.Menu,
                                Modifier
                                    .fillMaxSize()
                                    .padding(it),
                                enterTransition = { fadeIn() },
                                exitTransition = { fadeOut() }) {
                                solitaire(navController, ::setCurrentViewModel)
                                sudoku(navController, ::setCurrentViewModel)
                                minesweeper(navController, ::setCurrentViewModel)
                                game2048(navController, ::setCurrentViewModel)

                                menu(navController, menuScreenViewModel, ::setCurrentViewModel)
                                settingsDialog(navController) {
                                    MenuSettings(
                                        context, menuScreenViewModel, authManager
                                    ) { authManager.signIn(this@MainActivity) }
                                }
                            }
                        }
                    }
                    BackHandler { }
                }
            }
        })
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
        dataManager.save(true)
        openGameMenu()
    }

    private fun openGameMenu() {
        if (!SceneManager.dialogActiveState.value) {
            when (SceneManager.currentScene) {
                SceneRegistry.Menu -> return
                SceneRegistry.Game2048 -> navController.navigateToGame2048GameMenu()
                SceneRegistry.Minesweeper -> navController.navigateToMinesweeperGameMenu()
                SceneRegistry.Solitaire -> navController.navigateToSolitaireGameMenu()
                SceneRegistry.Sudoku -> navController.navigateToSudokuGameMenu()
                else -> throw NotImplementedError()
            }
            currentViewModel.value!!.onOpenDialog()
        }
    }
}