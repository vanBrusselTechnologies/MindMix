package com.vanbrusselgames.mindmix.feature.gamefinished

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.IntrinsicSize
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.requiredWidth
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.layout.widthIn
import androidx.compose.foundation.layout.wrapContentWidth
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.MaterialTheme.colorScheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp

@Composable
fun Stats(content: @Composable () -> Unit) {
    Column(
        Modifier
            .width(IntrinsicSize.Min)
            .background(colorScheme.primary.copy(0.25f), RoundedCornerShape(6.dp))
            .padding(2.dp)
    ) {
        content()
    }
}

@Composable
fun StatRow(modifier: Modifier = Modifier, fieldText: String, valueText: String) {
    Row(
        modifier
            .height(IntrinsicSize.Min)
            .fillMaxWidth()
    ) {
        Text(
            fieldText,
            Modifier
                .padding(start = 4.dp)
                .wrapContentWidth(Alignment.Start)
                .requiredWidth(IntrinsicSize.Max)
        )
        Spacer(
            Modifier
                .widthIn(8.dp)
                .weight(1f)
        )
        Text(
            valueText,
            Modifier
                .padding(end = 4.dp)
                .wrapContentWidth(Alignment.End)
                .requiredWidth(IntrinsicSize.Max)
        )
    }
}