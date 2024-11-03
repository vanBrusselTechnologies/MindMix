package com.vanbrusselgames.mindmix.core.advertisement

import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.width
import androidx.compose.material3.Icon
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.unit.dp
import com.vanbrusselgames.mindmix.core.ui.DialogButton

@Composable
fun AdDoublerButton(
    modifier: Modifier = Modifier, adManager: AdManager, onAdWatched: (Int) -> Unit
) {
    val adLoaded = remember { mutableStateOf(false) }
    adManager.checkAdLoaded(adLoaded)
    DialogButton({ adManager.showAd(adLoaded, onAdWatched) }, modifier, enabled = adLoaded.value) {
        Icon(painterResource(R.drawable.outline_smart_display_24), "Advertisement")
        Spacer(Modifier.width(4.dp))
        Text(stringResource(if (!adLoaded.value) R.string.ad_loading else R.string.ad_tripler_percentage))
    }
}