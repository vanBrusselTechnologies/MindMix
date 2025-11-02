package com.vanbrusselgames.mindmix.core.ui.dialogs.gamefinished

import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.sizeIn
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Share
import androidx.compose.material3.Card
import androidx.compose.material3.Icon
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.alpha
import androidx.compose.ui.platform.LocalDensity
import androidx.compose.ui.platform.LocalWindowInfo
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.style.TextAlign
import androidx.navigation.NavController
import com.vanbrusselgames.mindmix.core.ui.R
import com.vanbrusselgames.mindmix.core.ui.dialogs.DialogButton

@Composable
fun GameFinishedDialog(content: @Composable () -> Unit) {
    val localDensity = LocalDensity.current
    val containerSize = LocalWindowInfo.current.containerSize
    val screenWidth = with(localDensity) { containerSize.width.toDp() }
    val screenHeight = with(localDensity) { containerSize.height.toDp() }
    Card(
        Modifier
            .sizeIn(maxWidth = screenWidth * 0.95f, maxHeight = screenHeight * 0.95f)
            .alpha(0.9f)
    ) {
        content()
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
        Text(stringResource(R.string.back_to_menu), textAlign = TextAlign.Center)
    }
}