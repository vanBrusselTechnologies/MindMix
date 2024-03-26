package com.vanbrusselgames.mindmix.menu

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
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
import androidx.compose.material3.DropdownMenuItem
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.ExposedDropdownMenuBox
import androidx.compose.material3.ExposedDropdownMenuDefaults
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme.colorScheme
import androidx.compose.material3.Text
import androidx.compose.material3.TextField
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.platform.LocalUriHandler
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.vanbrusselgames.mindmix.AuthManager
import com.vanbrusselgames.mindmix.PREF_KEY_MENU__THEME
import com.vanbrusselgames.mindmix.R
import com.vanbrusselgames.mindmix.Settings
import com.vanbrusselgames.mindmix.savePreferences
import com.vanbrusselgames.mindmix.ui.theme.SelectedTheme

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

            ThemeDropdown()

            Spacer(Modifier.height(2.dp))

            val signedIn = remember {
                mutableStateOf(AuthManager.isAuthenticated && AuthManager.currentUser != null)
            }
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

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun ThemeDropdown() {
    val ctx = LocalContext.current
    var dropdownExpanded by remember { mutableStateOf(false) }
    val defaultButtonColors = ButtonDefaults.buttonColors()
    Row(
        Modifier
            .fillMaxWidth()
            .background(defaultButtonColors.containerColor, RoundedCornerShape(6.dp)),
        Arrangement.Center,
        Alignment.CenterVertically
    ) {
        Text(
            stringResource(R.string.theme),
            Modifier.padding(16.dp),
            defaultButtonColors.contentColor
        )
        Spacer(Modifier.width(12.dp))
        Spacer(Modifier.weight(1f))
        ExposedDropdownMenuBox(
            dropdownExpanded,
            onExpandedChange = { dropdownExpanded = !dropdownExpanded },
            Modifier
                .padding(4.dp)
                .width(IntrinsicSize.Min)
        ) {
            val textFieldValue = stringResource(
                when (MenuManager.theme.value) {
                    SelectedTheme.System -> R.string.system
                    SelectedTheme.Dark -> R.string.dark_theme
                    SelectedTheme.Light -> R.string.light_theme
                }
            )
            TextField(value = textFieldValue,
                {},
                modifier = Modifier.menuAnchor(),
                readOnly = true,
                trailingIcon = {
                    ExposedDropdownMenuDefaults.TrailingIcon(dropdownExpanded)
                })
            ExposedDropdownMenu(expanded = dropdownExpanded, onDismissRequest = {
                dropdownExpanded = false
            }, Modifier.width(IntrinsicSize.Min)) {
                DropdownMenuItem(text = { Text(stringResource(R.string.system)) }, onClick = {
                    MenuManager.theme.value = SelectedTheme.System
                    dropdownExpanded = false
                    savePreferences(
                        ctx, PREF_KEY_MENU__THEME, SelectedTheme.System.ordinal
                    )
                })
                DropdownMenuItem(text = { Text(stringResource(R.string.dark_theme)) }, onClick = {
                    MenuManager.theme.value = SelectedTheme.Dark
                    dropdownExpanded = false
                    savePreferences(
                        ctx, PREF_KEY_MENU__THEME, SelectedTheme.Dark.ordinal
                    )
                })
                DropdownMenuItem(text = { Text(stringResource(R.string.light_theme)) }, onClick = {
                    MenuManager.theme.value = SelectedTheme.Light
                    dropdownExpanded = false
                    savePreferences(
                        ctx, PREF_KEY_MENU__THEME, SelectedTheme.Light.ordinal
                    )
                })
            }
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