package com.vanbrusselgames.mindmix

import androidx.compose.foundation.Image
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
import androidx.compose.foundation.layout.safeDrawingPadding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.layout.width
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material.icons.filled.Close
import androidx.compose.material.icons.filled.Settings
import androidx.compose.material.icons.filled.Share
import androidx.compose.material3.Button
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.Scaffold
import androidx.compose.material3.SnackbarHost
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
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.compose.ui.unit.times
import com.google.firebase.analytics.FirebaseAnalytics
import com.vanbrusselgames.mindmix.menu.MenuManager

abstract class BaseLayout {
    companion object {
        var helpOpened = mutableStateOf(false)
        var disableTopRowButtons = mutableStateOf(false)
    }

    private val topRowButtonSize = 48.dp
    private val padding = topRowButtonSize / 100f
    private val topRowHeight = (topRowButtonSize + 2f * padding)
    protected var screenHeight = 0.dp
    protected var screenWidth = 0.dp

    abstract var uiHandler: BaseUIHandler

    @Composable
    open fun BaseScene(sceneSpecific: @Composable () -> Unit?, isMenu: Boolean = false) {
        val localCurrentConfig = LocalConfiguration.current
        screenHeight = localCurrentConfig.screenHeightDp.dp
        screenWidth = localCurrentConfig.screenWidthDp.dp
        Scaffold(topBar = { TopBar(isMenu) }, snackbarHost = {
            SnackbarHost(hostState = MainActivity.snackbarHostState)
        }, modifier = Modifier.safeDrawingPadding()) {
            Box(
                Modifier
                    .fillMaxSize()
                    .padding(it)
            ) {
                sceneSpecific()
            }
        }
    }

    @Composable
    private fun TopBar(isMenu: Boolean) {
        Box(
            Modifier
                .height(topRowHeight)
                .fillMaxWidth()
        ) {
            if (!disableTopRowButtons.value) {
                if (!isMenu) {

                    IconButton(
                        onClick = { uiHandler.backToMenu() },
                        modifier = Modifier
                            .size(topRowButtonSize)
                            .padding(padding)
                            .align(Alignment.TopStart)
                    ) {
                        Icon(
                            imageVector = Icons.AutoMirrored.Default.ArrowBack,
                            contentDescription = "Back",
                            Modifier.fillMaxSize(0.9f)
                        )
                    }
                    Row(
                        Modifier.align(Alignment.TopEnd)
                    ) {
                        IconButton(
                            onClick = { uiHandler.openHelp() },
                            modifier = Modifier
                                .size(topRowButtonSize)
                                .padding(padding)
                        ) {
                            Icon(
                                painterResource(R.drawable.outline_help_24),
                                "Settings",
                                Modifier.fillMaxSize(0.9f),
                            )
                        }
                    }
                }
                //todo Add App settings
                else Row(
                    Modifier.align(Alignment.TopEnd)
                ) {
                    /*if (isMenu) {
                    IconButton(
                        onClick = { MenuUIHandler.openShop() },
                        modifier = Modifier
                            .size(topRowButtonSize)
                            .padding(padding)
                    ) {
                        Icon(
                            Icons.Filled.ShoppingCart, "Shop", Modifier.fillMaxSize(0.8f)
                        )
                    }
                }*/
                    IconButton(
                        onClick = { uiHandler.openSettings() },
                        modifier = Modifier
                            .size(topRowButtonSize)
                            .padding(padding)
                    ) {
                        Icon(
                            Icons.Filled.Settings, "Settings", Modifier.fillMaxSize(0.9f),
                        )
                    }
                }
            }
        }
    }

    @Composable
    fun BaseGameFinishedPopUp(
        title: String,
        description: String,
        reward: Int = 0,
        onClickShare: (() -> Unit)? = null,
        onClickPlayAgain: (() -> Unit)? = null,
        onClickReturnToMenu: (() -> Unit)? = null,
        sceneSpecific: @Composable () -> Unit = {}
    ) {
        val adManager = remember { MainActivity.adManager }
        val rewarded = remember { mutableStateOf(false) }
        if (!rewarded.value) {
            MenuManager.coins += reward
            logEarnedCurrencyReward(reward)
            DataManager.save()
            rewarded.value = true
        }
        disableTopRowButtons.value = true
        Box(Modifier.fillMaxSize(), Alignment.Center) {
            Box(
                Modifier.fillMaxSize(0.95f), Alignment.Center
            ) {
                Card(
                    Modifier.alpha(0.9f), elevation = CardDefaults.cardElevation(20.dp)
                ) {
                    Column(
                        Modifier.padding(16.dp), horizontalAlignment = Alignment.CenterHorizontally
                    ) {
                        Text(text = title, fontSize = 25.sp, fontWeight = FontWeight.ExtraBold)
                        Text(text = description, textAlign = TextAlign.Center)
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
                                        if (!adLoaded.value) Text(stringResource(R.string.loading)) else Text(
                                            "+200%"
                                        )
                                    }
                                }
                            }
                        }
                        sceneSpecific()
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
                                    disableTopRowButtons.value = false
                                }) {
                                    Text(stringResource(R.string.play_again))
                                }
                            }
                            if (onClickReturnToMenu != null) {
                                Button({
                                    onClickReturnToMenu()
                                    disableTopRowButtons.value = false
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
    }

    fun logEarnedCurrencyReward(reward: Int) {
        Logger.logEvent(FirebaseAnalytics.Event.EARN_VIRTUAL_CURRENCY) {
            param(FirebaseAnalytics.Param.VIRTUAL_CURRENCY_NAME, "Coin")
            param(FirebaseAnalytics.Param.VALUE, reward.toDouble())
            param(FirebaseAnalytics.Param.CURRENCY, "EUR")
        }
    }

    @Composable
    fun GameHelp(titleId: Int, descriptionId: Int) {
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
                                helpOpened.value = false
                                disableTopRowButtons.value = false
                            }, Modifier.align(Alignment.TopEnd)
                        ) {
                            Icon(Icons.Default.Close, contentDescription = "Close")
                        }
                        Column(
                            Modifier.padding(16.dp),
                            horizontalAlignment = Alignment.CenterHorizontally
                        ) {
                            Text(text = title, fontSize = 32.sp, fontWeight = FontWeight.ExtraBold)
                            Spacer(Modifier.height(16.dp))
                            Text(text = description, textAlign = TextAlign.Center)
                        }
                    }
                }
            }
        }
    }
}