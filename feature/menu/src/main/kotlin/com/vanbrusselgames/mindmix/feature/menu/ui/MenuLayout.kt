package com.vanbrusselgames.mindmix.feature.menu.ui

import androidx.compose.animation.AnimatedContent
import androidx.compose.animation.AnimatedContentScope
import androidx.compose.animation.ExperimentalSharedTransitionApi
import androidx.compose.animation.SharedTransitionLayout
import androidx.compose.animation.SharedTransitionScope
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.offset
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.rounded.PlayArrow
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme.colorScheme
import androidx.compose.material3.Surface
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.blur
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.tooling.preview.PreviewScreenSizes
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import androidx.navigation.NavController
import androidx.navigation.compose.rememberNavController
import com.vanbrusselgames.mindmix.core.common.ui.BaseScene
import com.vanbrusselgames.mindmix.core.designsystem.theme.MindMixTheme
import com.vanbrusselgames.mindmix.core.navigation.SceneManager
import com.vanbrusselgames.mindmix.feature.menu.R
import com.vanbrusselgames.mindmix.feature.menu.viewmodel.IMenuScreenViewModel
import com.vanbrusselgames.mindmix.feature.menu.viewmodel.MockMenuScreenViewModel
import com.vanbrusselgames.mindmix.feature.settings.navigation.navigateToSettings

@OptIn(ExperimentalSharedTransitionApi::class)
@Composable
fun SharedTransitionScope.SceneUI(
    viewModel: IMenuScreenViewModel,
    navController: NavController,
    animatedContentScope: AnimatedContentScope
) {
    BaseScene(viewModel, {}, {}, { navController.navigateToSettings() }) {
        val loadedState = viewModel.preferencesLoaded.collectAsStateWithLifecycle()
        if (loadedState.value) SetLayoutGameWheel(viewModel, navController, animatedContentScope)
    }
}

@OptIn(ExperimentalSharedTransitionApi::class)
@Composable
fun SharedTransitionScope.SetLayoutGameWheel(
    viewModel: IMenuScreenViewModel,
    navController: NavController,
    animatedContentScope: AnimatedContentScope
) {
    Box(
        Modifier
            .fillMaxSize()
            .blur(if (SceneManager.dialogActiveState.value) 6.dp else 0.dp),
        Alignment.BottomCenter
    ) {
        GameWheel(viewModel, navController, animatedContentScope, viewModel.wheelModel)
        PlayButton(viewModel, navController, Modifier.align(Alignment.BottomCenter))
    }
}

@Composable
fun PlayButton(viewModel: IMenuScreenViewModel, navController: NavController, modifier: Modifier) {
    Button(
        { viewModel.navigateToSelectedGame(navController) },
        modifier.offset(0.dp, (-5).dp),
        colors = ButtonDefaults.buttonColors(
            containerColor = colorScheme.primary,
            contentColor = colorScheme.onPrimary,
            disabledContainerColor = colorScheme.primary,
            disabledContentColor = colorScheme.onPrimary,
        )
    ) {
        Row(verticalAlignment = Alignment.CenterVertically) {
            Icon(
                Icons.Rounded.PlayArrow,
                null,
                tint = colorScheme.onPrimary,
            )
            Text(
                stringResource(R.string.play),
                color = colorScheme.onPrimary,
                fontSize = 19.sp,
                textAlign = TextAlign.Center,
            )
        }
    }
}

@OptIn(ExperimentalSharedTransitionApi::class)
@PreviewScreenSizes
@Composable
private fun PrevMenu() {
    SharedTransitionLayout {
        AnimatedContent(null) {
            it
            MindMixTheme {
                Surface {
                    val vm = remember { MockMenuScreenViewModel() }
                    SceneUI(vm, rememberNavController(), this@AnimatedContent)
                }
            }
        }
    }
}