package com.vanbrusselgames.mindmix.feature.gamefinished

import androidx.compose.foundation.Image
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.heightIn
import androidx.compose.foundation.layout.sizeIn
import androidx.compose.foundation.layout.width
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Share
import androidx.compose.material3.Card
import androidx.compose.material3.Icon
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.alpha
import androidx.compose.ui.platform.LocalConfiguration
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.navigation.NavController
import com.google.firebase.analytics.FirebaseAnalytics
import com.vanbrusselgames.mindmix.core.advertisement.AdDoublerButton
import com.vanbrusselgames.mindmix.core.advertisement.AdManager
import com.vanbrusselgames.mindmix.core.logging.Logger
import com.vanbrusselgames.mindmix.core.ui.DialogButton

@Composable
fun GameFinishedDialog(content: @Composable () -> Unit) {
    val screenWidth = LocalConfiguration.current.screenWidthDp.dp
    val screenHeight = LocalConfiguration.current.screenHeightDp.dp
    Card(
        Modifier
            .sizeIn(maxWidth = screenWidth * 0.95f, maxHeight = screenHeight * 0.95f)
            .alpha(0.9f)
    ) {
        content()
    }
}

@Composable
fun GameFinishedRewardRow(reward: Int, adManager: () -> AdManager) {
    Row(
        Modifier.heightIn(max = 48.dp),
        horizontalArrangement = Arrangement.SpaceEvenly,
        verticalAlignment = Alignment.CenterVertically
    ) {
        var adShown by remember { mutableStateOf(false) }
        var bonus by remember { mutableIntStateOf(0) }
        Image(painterResource(R.drawable.coin), "Coin")
        Spacer(Modifier.width(4.dp))
        Text(text = (reward * (1 + bonus)).toString(), fontSize = 28.sp)
        if (!adShown) {
            Spacer(Modifier.width(16.dp))
            AdDoublerButton(adManager = adManager()) { adReward ->
                adShown = true
                bonus = adReward
                //todo: MainActivity.menu.viewModel.coins += reward * bonus
                // DataManager.save()
                logEarnedCurrencyReward(reward * bonus)
            }
        }
    }
}

@Composable
fun Buttons(
    navController: NavController,
    modifier: Modifier = Modifier,
    onClickShare: (() -> Unit)? = null,
    onClickPlayAgain: (() -> Unit)? = null,
    backToMenu: () -> Unit
) {
    if (onClickShare != null) {
        DialogButton(onClickShare, modifier) {
            Row(verticalAlignment = Alignment.CenterVertically) {
                Icon(Icons.Default.Share, "Share")
                Text(" ${stringResource(R.string.share)}")
            }
        }
    }
    if (onClickPlayAgain != null) {
        DialogButton({
            onClickPlayAgain()
            navController.popBackStack()
        }, modifier) { Text(stringResource(R.string.play_again)) }
    }
    DialogButton(backToMenu, modifier) {
        Text(
            stringResource(R.string.back_to_menu), textAlign = TextAlign.Center
        )
    }
}

private fun logEarnedCurrencyReward(reward: Int) {
    Logger.logEvent(FirebaseAnalytics.Event.EARN_VIRTUAL_CURRENCY) {
        param(FirebaseAnalytics.Param.VIRTUAL_CURRENCY_NAME, "Coin")
        param(FirebaseAnalytics.Param.VALUE, reward.toDouble())
        param(FirebaseAnalytics.Param.CURRENCY, "EUR")
    }
}