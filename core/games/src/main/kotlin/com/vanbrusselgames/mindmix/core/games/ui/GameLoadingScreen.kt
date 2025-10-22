package com.vanbrusselgames.mindmix.core.games.ui

import androidx.compose.animation.AnimatedContentScope
import androidx.compose.animation.ExperimentalSharedTransitionApi
import androidx.compose.animation.SharedTransitionScope
import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.size
import androidx.compose.material3.LinearProgressIndicator
import androidx.compose.material3.MaterialTheme
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.unit.dp
import com.vanbrusselgames.mindmix.core.games.model.GameType
import kotlin.time.Duration.Companion.seconds

val minimumDurationLoadingScreen = 0.5.seconds

@OptIn(ExperimentalSharedTransitionApi::class)
@Composable
fun SharedTransitionScope.GameLoadingScreen(
    animatedContentScope: AnimatedContentScope, gameType: GameType
) {
    Box(
        Modifier
            .fillMaxSize()
            .background(MaterialTheme.colorScheme.background), Alignment.Center
    ) {
        Column(Modifier, Arrangement.Center, Alignment.CenterHorizontally) {
            Image(
                painter = painterResource(id = gameType.iconRes),
                contentDescription = gameType.name,
                modifier = Modifier
                    .size(250.dp)
                    .sharedElement(
                        rememberSharedContentState(key = "image-${gameType.name}"),
                        animatedContentScope
                    )
            )
            Spacer(Modifier.height(10.dp))
            //Text(text = "LOADING...")
            LinearProgressIndicator()
        }
    }
}