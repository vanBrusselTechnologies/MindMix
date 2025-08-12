package com.vanbrusselgames.mindmix.games.solitaire.ui.dialogs

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
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.Surface
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import androidx.navigation.NavController
import androidx.navigation.compose.rememberNavController
import com.vanbrusselgames.mindmix.core.designsystem.theme.MindMixTheme
import com.vanbrusselgames.mindmix.core.ui.EnumDropdown
import com.vanbrusselgames.mindmix.feature.settings.SettingsDialog
import com.vanbrusselgames.mindmix.games.solitaire.R
import com.vanbrusselgames.mindmix.games.solitaire.model.CardVisualType
import com.vanbrusselgames.mindmix.games.solitaire.model.Solitaire
import com.vanbrusselgames.mindmix.games.solitaire.viewmodel.ISolitaireViewModel
import com.vanbrusselgames.mindmix.games.solitaire.viewmodel.MockSolitaireViewModel

@Composable
fun SolitaireSettingsDialog(viewModel: ISolitaireViewModel, navController: NavController) {
    SettingsDialog(navController) {
        Column(
            Modifier
                .padding(24.dp)
                .width(IntrinsicSize.Max),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            Text(
                text = stringResource(R.string.solitaire_name),
                Modifier.fillMaxWidth(),
                fontSize = 36.sp,
                fontWeight = FontWeight.ExtraBold,
                textAlign = TextAlign.Center
            )
            val loadedState = viewModel.preferencesLoaded.collectAsStateWithLifecycle()
            if (!loadedState.value) return@SettingsDialog
            Spacer(Modifier.height(20.dp))
            CardTypeDropdownRow(viewModel)
        }
    }
}

@Composable
fun CardTypeDropdownRow(viewModel: ISolitaireViewModel) {
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
            Modifier.padding(20.dp),
            defaultButtonColors.contentColor
        )
        Spacer(Modifier.weight(1f))
        val modifier = Modifier
            .padding(4.dp)
            .width(IntrinsicSize.Min)
        val callback = { type: CardVisualType -> viewModel.setCardVisualType(type) }
        EnumDropdown(modifier, viewModel.cardVisualType, callback, CardVisualType.entries.toList())
    }
}

@Preview(
    locale = "nl", uiMode = Configuration.UI_MODE_NIGHT_YES or Configuration.UI_MODE_TYPE_NORMAL
)
@Preview
@Composable
fun Prev_Settings() {
    MindMixTheme {
        Surface {
            val vm = remember { MockSolitaireViewModel() }
            SolitaireSettingsDialog(vm, rememberNavController())
        }
    }
}