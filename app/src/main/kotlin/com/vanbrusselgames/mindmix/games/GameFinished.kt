package com.vanbrusselgames.mindmix.games

import androidx.compose.foundation.Image
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.heightIn
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.width
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Share
import androidx.compose.material3.Button
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
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
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.google.firebase.analytics.FirebaseAnalytics
import com.vanbrusselgames.mindmix.BaseLayout
import com.vanbrusselgames.mindmix.BaseUIHandler
import com.vanbrusselgames.mindmix.DataManager
import com.vanbrusselgames.mindmix.Logger
import com.vanbrusselgames.mindmix.MainActivity
import com.vanbrusselgames.mindmix.R
import com.vanbrusselgames.mindmix.menu.MenuManager

class GameFinished {
    companion object {
        val visible = mutableStateOf(false)

        private var titleId = 0
        private var descriptionId = 0
        private var reward = 0

        fun onGameFinished(titleId: Int, descriptionId: Int, reward: Int) {
            this.titleId = titleId
            this.descriptionId = descriptionId
            this.descriptionId = descriptionId
            this.reward = reward
            if (reward != 0) {
                MenuManager.coins += reward
                logEarnedCurrencyReward(reward)
            }
            DataManager.save()
            BaseLayout.activeOverlapUI.value = true
            visible.value = true
        }

        @Composable
        fun Screen(
            onClickShare: (() -> Unit)? = null,
            onClickPlayAgain: (() -> Unit)? = null,
            sceneSpecific: (@Composable () -> Unit)? = null
        ) {
            if (!visible.value) return
            val adManager = remember { MainActivity.adManager }
            Box(Modifier.fillMaxSize(), Alignment.Center) {
                Box(
                    Modifier.fillMaxSize(0.95f), Alignment.Center
                ) {
                    Card(
                        Modifier.alpha(0.9f), elevation = CardDefaults.cardElevation(20.dp)
                    ) {
                        Column(
                            Modifier.padding(16.dp),
                            horizontalAlignment = Alignment.CenterHorizontally
                        ) {
                            Text(
                                stringResource(titleId),
                                fontSize = 25.sp,
                                fontWeight = FontWeight.ExtraBold
                            )
                            Spacer(Modifier.height(2.dp))
                            Text(stringResource(descriptionId), textAlign = TextAlign.Center)
                            if (sceneSpecific != null) {
                                Spacer(Modifier.height(2.dp))
                                sceneSpecific()
                            }
                            if (reward != 0) {
                                Spacer(Modifier.height(8.dp))
                                Row(
                                    Modifier.heightIn(max = 48.dp),
                                    horizontalArrangement = Arrangement.SpaceEvenly,
                                    verticalAlignment = Alignment.CenterVertically
                                ) {
                                    var adShown by remember { mutableStateOf(false) }
                                    var bonus by remember { mutableIntStateOf(0) }
                                    val adLoaded = remember { mutableStateOf(false) }
                                    adManager.checkAdLoaded(adLoaded)
                                    Image(painterResource(R.drawable.coin), "Coin")
                                    Spacer(Modifier.width(4.dp))
                                    Text(text = (reward * (1 + bonus)).toString(), fontSize = 28.sp)
                                    if (!adShown) {
                                        Spacer(Modifier.width(16.dp))
                                        Button(onClick = {
                                            adManager.showAd(adLoaded) { adReward ->
                                                adShown = true
                                                bonus = adReward
                                                MenuManager.coins += reward * bonus
                                                DataManager.save()
                                                logEarnedCurrencyReward(reward * bonus)
                                            }
                                        }, enabled = adLoaded.value) {
                                            Icon(
                                                painterResource(R.drawable.outline_smart_display_24),
                                                "Advertisement"
                                            )
                                            Spacer(Modifier.width(4.dp))
                                            if (!adLoaded.value) {
                                                Text(stringResource(R.string.loading))
                                            } else Text("+200%")
                                        }
                                    }
                                }
                            }
                            Spacer(Modifier.height(8.dp))
                            Row(
                                horizontalArrangement = Arrangement.spacedBy(
                                    space = 8.dp, alignment = Alignment.CenterHorizontally
                                ), verticalAlignment = Alignment.CenterVertically
                            ) {
                                if (onClickShare != null) {
                                    Button(onClickShare) {
                                        Row(verticalAlignment = Alignment.CenterVertically) {
                                            Icon(Icons.Default.Share, "Share")
                                            Text(" ${stringResource(R.string.share)}")
                                        }
                                    }
                                }
                                if (onClickPlayAgain != null) {
                                    Button({
                                        onClickPlayAgain()
                                        BaseLayout.activeOverlapUI.value = false
                                        visible.value = false
                                    }) {
                                        Text(stringResource(R.string.play_again))
                                    }
                                }
                                Button({
                                    BaseUIHandler.backToMenu()
                                    BaseLayout.activeOverlapUI.value = false
                                    visible.value = false
                                }) {
                                    Text(
                                        stringResource(R.string.back_to_menu),
                                        textAlign = TextAlign.Center
                                    )
                                }
                            }
                        }
                    }
                }
            }
        }

        private fun logEarnedCurrencyReward(reward: Int) {
            Logger.logEvent(FirebaseAnalytics.Event.EARN_VIRTUAL_CURRENCY) {
                param(FirebaseAnalytics.Param.VIRTUAL_CURRENCY_NAME, "Coin")
                param(FirebaseAnalytics.Param.VALUE, reward.toDouble())
                param(FirebaseAnalytics.Param.CURRENCY, "EUR")
            }
        }
    }
}