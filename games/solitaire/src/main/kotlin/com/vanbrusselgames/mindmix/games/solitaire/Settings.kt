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
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.Surface
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.vanbrusselgames.mindmix.core.designsystem.theme.MindMixTheme
import com.vanbrusselgames.mindmix.core.ui.EnumDropdown
import com.vanbrusselgames.mindmix.games.solitaire.PlayingCard.CardVisualType

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun SolitaireSettings(viewModel: GameViewModel) {
    Column(
        Modifier
            .padding(24.dp)
            .width(IntrinsicSize.Max),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        LocalContext.current
        Text(
            text = stringResource(Solitaire.NAME_RES_ID),
            Modifier.fillMaxWidth(),
            fontSize = 36.sp,
            fontWeight = FontWeight.ExtraBold,
            textAlign = TextAlign.Center
        )
        Spacer(Modifier.height(20.dp))
        CardTypeDropdownRow(viewModel)
    }
}

@Composable
fun CardTypeDropdownRow(viewModel: GameViewModel) {
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
        val callback = { cardType: CardVisualType -> viewModel.cardVisualType.value = cardType }
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
            SolitaireSettings(GameViewModel())
        }
    }
}