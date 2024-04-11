package com.vanbrusselgames.mindmix.games

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Close
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.mutableStateOf
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.alpha
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.vanbrusselgames.mindmix.BaseLayout

class GameHelp {
    companion object {
        val visible = mutableStateOf(false)

        @Composable
        fun Screen(titleId: Int, descriptionId: Int, timer: GameTimer? = null) {
            if (!visible.value) return
            val title = stringResource(titleId)
            val description = stringResource(descriptionId)

            Box(Modifier.fillMaxSize(), Alignment.Center) {
                Box(
                    Modifier.fillMaxSize(0.95f), Alignment.Center
                ) {
                    Card(
                        Modifier.alpha(0.9f), elevation = CardDefaults.cardElevation(20.dp)
                    ) {
                        Box(Modifier.align(Alignment.CenterHorizontally)) {
                            IconButton(
                                onClick = {
                                    timer?.resume()
                                    visible.value = false
                                    BaseLayout.activeOverlapUI.value = false
                                }, Modifier.align(Alignment.TopEnd)
                            ) {
                                Icon(Icons.Default.Close, contentDescription = "Close")
                            }
                            Column(
                                Modifier.padding(16.dp),
                                horizontalAlignment = Alignment.CenterHorizontally
                            ) {
                                Text(
                                    text = title,
                                    fontSize = 32.sp,
                                    fontWeight = FontWeight.ExtraBold
                                )
                                Spacer(Modifier.height(16.dp))
                                Text(text = description, textAlign = TextAlign.Center)
                            }
                        }
                    }
                }
            }
        }
    }
}