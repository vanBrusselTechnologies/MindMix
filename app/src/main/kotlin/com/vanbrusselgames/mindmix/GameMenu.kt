package com.vanbrusselgames.mindmix

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.IntrinsicSize
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Home
import androidx.compose.material.icons.filled.PlayArrow
import androidx.compose.material3.Button
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.Icon
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.mutableStateOf
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.alpha
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp

class GameMenu {
    companion object {
        val visible = mutableStateOf(false)

        @Composable
        fun Screen(gameNameId: Int, startNewGame: () -> Unit) {
            if (!visible.value) return
            Box(Modifier.fillMaxSize(), Alignment.Center) {
                Box(Modifier.fillMaxSize(0.95f), Alignment.Center) {
                    Card(
                        Modifier.alpha(0.9f),
                        elevation = CardDefaults.cardElevation(20.dp),
                    ) {
                        Column(
                            Modifier
                                .padding(24.dp)
                                .align(Alignment.CenterHorizontally)
                                .width(intrinsicSize = IntrinsicSize.Max),
                            horizontalAlignment = Alignment.CenterHorizontally
                        ) {
                            Text(
                                text = stringResource(gameNameId),
                                fontSize = 36.sp,
                                fontWeight = FontWeight.ExtraBold
                            )
                            Spacer(Modifier.height(15.dp))
                            Button(
                                {
                                    visible.value = false
                                    BaseLayout.disableTopRowButtons.value = false
                                }, Modifier.fillMaxWidth(), shape = RoundedCornerShape(6.dp)
                            ) {
                                Icon(Icons.Default.PlayArrow, contentDescription = "Continue")
                                Spacer(Modifier.width(8.dp))
                                Text(stringResource(R.string.continue_game))
                            }
                            Spacer(Modifier.height(2.dp))
                            Button({
                                startNewGame()
                                visible.value = false
                                BaseLayout.disableTopRowButtons.value = false
                            }, Modifier.fillMaxWidth(), shape = RoundedCornerShape(6.dp)) {
                                Icon(
                                    painter = painterResource(R.drawable.outline_autorenew_24),
                                    contentDescription = "Start new game"
                                )
                                Spacer(Modifier.width(8.dp))
                                Text(stringResource(R.string.play_again))
                            }
                            Spacer(Modifier.height(2.dp))
                            Button({
                                BaseUIHandler.backToMenu()
                                visible.value = false
                                BaseLayout.disableTopRowButtons.value = false
                            }, Modifier.fillMaxWidth(), shape = RoundedCornerShape(6.dp)) {
                                Icon(Icons.Default.Home, contentDescription = "Back to menu")
                                Spacer(Modifier.width(8.dp))
                                Text(stringResource(R.string.back_to_menu))
                            }
                        }
                    }
                }
            }
        }
    }
}

@Preview(locale = "nl")
@Preview
@Composable
fun Prev_Screen() {
    GameMenu.visible.value = true
    GameMenu.Screen(R.string.sudoku_name){}
}