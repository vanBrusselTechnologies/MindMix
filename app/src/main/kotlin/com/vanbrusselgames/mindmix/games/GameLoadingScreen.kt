package com.vanbrusselgames.mindmix.games

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.material3.LinearProgressIndicator
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier

@Composable
fun GameLoadingScreen() {
    Box(Modifier.fillMaxSize(), Alignment.Center) {
        Column {
            Text(text = "LOADING...")
            LinearProgressIndicator()
        }
    }
}