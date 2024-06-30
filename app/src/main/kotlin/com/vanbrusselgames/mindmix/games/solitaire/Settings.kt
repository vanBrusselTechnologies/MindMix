package com.vanbrusselgames.mindmix.games.solitaire

import android.content.res.Configuration
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.IntrinsicSize
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.layout.widthIn
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.DropdownMenuItem
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.ExposedDropdownMenuBox
import androidx.compose.material3.ExposedDropdownMenuDefaults
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
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.vanbrusselgames.mindmix.BaseUIHandler
import com.vanbrusselgames.mindmix.PREF_KEY_SOLITAIRE__CARD_TYPE
import com.vanbrusselgames.mindmix.R
import com.vanbrusselgames.mindmix.Settings
import com.vanbrusselgames.mindmix.games.solitaire.SolitaireManager.Instance.timer
import com.vanbrusselgames.mindmix.savePreferences
import com.vanbrusselgames.mindmix.ui.theme.MindMixTheme

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun SolitaireSettings() {
    Settings.Screen(timer) {
        Column(
            Modifier
                .padding(24.dp)
                .width(IntrinsicSize.Max),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            val ctx = LocalContext.current

            Text(
                text = stringResource(R.string.solitaire_name),
                Modifier.fillMaxWidth(),
                fontSize = 36.sp,
                fontWeight = FontWeight.ExtraBold,
                textAlign = TextAlign.Center
            )
            Spacer(Modifier.height(20.dp))
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
                    stringResource(R.string.card_type),
                    Modifier.padding(24.dp),
                    defaultButtonColors.contentColor
                )
                Spacer(Modifier.weight(1f))
                ExposedDropdownMenuBox(
                    dropdownExpanded,
                    onExpandedChange = { dropdownExpanded = !dropdownExpanded },
                    Modifier.padding(4.dp)
                ) {
                    val textFieldValue = stringResource(
                        when (SolitaireManager.cardVisualType.value) {
                            PlayingCard.CardVisualType.SIMPLE -> R.string.card_type_simple
                            PlayingCard.CardVisualType.DETAILED -> R.string.card_type_detailed
                        }
                    )
                    TextField(value = textFieldValue,
                        {},
                        modifier = Modifier.menuAnchor().widthIn(1.dp, Dp.Infinity),
                        readOnly = true,
                        trailingIcon = {
                            ExposedDropdownMenuDefaults.TrailingIcon(dropdownExpanded)
                        })
                    ExposedDropdownMenu(dropdownExpanded, onDismissRequest = {
                        dropdownExpanded = false
                    }) {
                        DropdownMenuItem(text = { Text(stringResource(R.string.card_type_simple)) },
                            onClick = {
                                SolitaireManager.cardVisualType.value =
                                    PlayingCard.CardVisualType.SIMPLE
                                dropdownExpanded = false
                                savePreferences(
                                    ctx,
                                    PREF_KEY_SOLITAIRE__CARD_TYPE,
                                    PlayingCard.CardVisualType.SIMPLE.ordinal
                                )
                            })
                        DropdownMenuItem(text = { Text(stringResource(R.string.card_type_detailed)) },
                            onClick = {
                                SolitaireManager.cardVisualType.value =
                                    PlayingCard.CardVisualType.DETAILED
                                dropdownExpanded = false
                                savePreferences(
                                    ctx,
                                    PREF_KEY_SOLITAIRE__CARD_TYPE,
                                    PlayingCard.CardVisualType.DETAILED.ordinal
                                )
                            })
                    }
                }
            }
        }
    }
}

@Preview(
    locale = "nl", uiMode = Configuration.UI_MODE_NIGHT_YES or Configuration.UI_MODE_TYPE_NORMAL
)
@Preview
@Composable
fun Prev_Settings_Screen() {
    MindMixTheme {
        BaseUIHandler.openSettings()
        SolitaireSettings()
    }
}