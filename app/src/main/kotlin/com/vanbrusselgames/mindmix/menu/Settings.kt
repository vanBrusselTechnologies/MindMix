package com.vanbrusselgames.mindmix.menu

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.heightIn
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Close
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.alpha
import androidx.compose.ui.platform.LocalUriHandler
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.vanbrusselgames.mindmix.AuthManager
import com.vanbrusselgames.mindmix.BaseLayout
import com.vanbrusselgames.mindmix.Logger
import com.vanbrusselgames.mindmix.R


class Settings {
    companion object {
        private const val baseDataDeletionUrl =
            "https://docs.google.com/forms/d/e/1FAIpQLSf-_Yo6db7aYUt3qcEBq-rxCgjVCN1uVcWeeZ_oXQyZH8DIyQ/viewform?usp=pp_url&entry.2052602114="
        val visible = mutableStateOf(false)

        @Composable
        fun Screen() {
            if (!visible.value) return
            val colorScheme = MaterialTheme.colorScheme
            Box(Modifier.fillMaxSize(), Alignment.Center) {
                Box(Modifier.fillMaxSize(0.95f), Alignment.Center) {
                    Card(
                        Modifier.alpha(0.9f),
                        elevation = CardDefaults.cardElevation(20.dp),
                    ) {
                        Box(
                            Modifier.align(Alignment.CenterHorizontally)
                        ) {
                            IconButton(
                                onClick = {
                                    visible.value = false
                                    BaseLayout.disableTopRowButtons.value = false
                                }, Modifier.align(Alignment.TopEnd)
                            ) {
                                Icon(Icons.Default.Close, contentDescription = "Close")
                            }
                            Column(
                                Modifier
                                    .padding(16.dp)
                                    .fillMaxWidth(0.9f),
                                horizontalAlignment = Alignment.CenterHorizontally
                            ) {
                                Text(
                                    text = stringResource(R.string.settings),
                                    fontSize = 36.sp,
                                    fontWeight = FontWeight.ExtraBold
                                )
                                Spacer(Modifier.height(20.dp))
                                Row(
                                    horizontalArrangement = Arrangement.spacedBy(
                                        space = 8.dp, alignment = Alignment.CenterHorizontally
                                    ), verticalAlignment = Alignment.CenterVertically
                                ) {

                                }

                                val signedIn =
                                    remember { mutableStateOf(AuthManager.isAuthenticated && AuthManager.currentUser != null) }
                                Button(
                                    { AuthManager.signIn(signedIn) },
                                    shape = RoundedCornerShape(6.dp),
                                    enabled = !signedIn.value
                                ) {
                                    Row(
                                        Modifier.heightIn(max = 36.dp),
                                        verticalAlignment = Alignment.CenterVertically
                                    ) {
                                        Icon(
                                            painter = painterResource(R.drawable.games_controller),
                                            contentDescription = "Play Games Sign-In"
                                        )
                                        Spacer(Modifier.width(8.dp))
                                        Text(stringResource(if (signedIn.value) R.string.connected else R.string.signin))
                                    }
                                }
                                Spacer(Modifier.height(2.dp))

                                val uriHandler = LocalUriHandler.current
                                Button(
                                    {
                                        val userId = AuthManager.currentUser?.uid ?: ""
                                        Logger.d(baseDataDeletionUrl + userId)
                                        uriHandler.openUri(baseDataDeletionUrl + userId)
                                    }, colors = ButtonDefaults.buttonColors(
                                        containerColor = colorScheme.error,
                                        contentColor = colorScheme.onError
                                    ), shape = RoundedCornerShape(6.dp)
                                ) {
                                    Text(stringResource(R.string.request_data_deletion))
                                }
                                Spacer(Modifier.height(8.dp))
                                Text(
                                    "User ID: ${AuthManager.currentUser?.uid ?: ""}",
                                    fontSize = 10.sp
                                )
                            }
                        }
                    }
                }
            }
        }
    }
}