package com.vanbrusselgames.mindmix.menu

import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.IntrinsicSize
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.heightIn
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme.colorScheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalUriHandler
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.vanbrusselgames.mindmix.AuthManager
import com.vanbrusselgames.mindmix.R
import com.vanbrusselgames.mindmix.Settings

private const val DATA_DELETION_URL =
    "https://docs.google.com/forms/d/e/1FAIpQLSf-_Yo6db7aYUt3qcEBq-rxCgjVCN1uVcWeeZ_oXQyZH8DIyQ/viewform?usp=pp_url&entry.2052602114="

@Composable
fun MenuSettings() {
    Settings.Screen {
        Column(
            Modifier
                .padding(24.dp)
                .width(IntrinsicSize.Max),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            Text(
                text = stringResource(R.string.settings),
                Modifier.fillMaxWidth(),
                fontSize = 36.sp,
                fontWeight = FontWeight.ExtraBold,
                textAlign = TextAlign.Center
            )
            Spacer(Modifier.height(20.dp))

            val signedIn =
                remember { mutableStateOf(AuthManager.isAuthenticated && AuthManager.currentUser != null) }
            Button(
                { AuthManager.signIn(signedIn) },
                Modifier.fillMaxWidth(),
                enabled = !signedIn.value,
                RoundedCornerShape(6.dp)
            ) {
                Row(
                    Modifier.heightIn(max = 36.dp), verticalAlignment = Alignment.CenterVertically
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
                    uriHandler.openUri(DATA_DELETION_URL + userId)
                }, Modifier.fillMaxWidth(), colors = ButtonDefaults.buttonColors(
                    containerColor = colorScheme.error, contentColor = colorScheme.onError
                ), shape = RoundedCornerShape(6.dp)
            ) {
                Text(stringResource(R.string.request_data_deletion))
            }
            Spacer(Modifier.height(8.dp))
            Text(
                "User ID: ${AuthManager.currentUser?.uid ?: ""}",
                Modifier.fillMaxWidth(),
                fontSize = 10.sp,
                textAlign = TextAlign.Center
            )
        }
    }
}

@Preview(locale = "nl")
@Preview
@Composable
fun Prev_Settings_Screen() {
    Settings.visible.value = true
    MenuSettings()
}