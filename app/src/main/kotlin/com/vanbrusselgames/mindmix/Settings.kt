package com.vanbrusselgames.mindmix

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Close
import androidx.compose.material3.Card
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.runtime.Composable
import androidx.compose.runtime.mutableStateOf
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.alpha
import com.vanbrusselgames.mindmix.games.GameTimer

class Settings {
    companion object {
        val visible = mutableStateOf(false)

        @Composable
        fun Screen(timer: GameTimer? = null, content: @Composable () -> Unit) {
            if (!visible.value) return
            Box(Modifier.fillMaxSize(), Alignment.Center) {
                Box(Modifier.fillMaxSize(0.95f), Alignment.Center) {
                    Card(Modifier.alpha(0.9f)) {
                        Box(
                            Modifier.align(Alignment.CenterHorizontally)
                        ) {
                            IconButton(
                                onClick = {
                                    timer?.resume()
                                    visible.value = false
                                    BaseLayout.activeOverlapUI.value = false
                                }, Modifier.align(Alignment.TopEnd)
                            ) {
                                Icon(Icons.Default.Close, contentDescription = "Close")
                            }
                            content()
                        }
                    }
                }
            }
        }
    }
}