package com.vanbrusselgames.mindmix.core.ui

import androidx.compose.foundation.layout.RowScope
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Button
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Shape
import androidx.compose.ui.unit.dp

@Composable
fun DialogButton(onClick: () -> Unit, modifier: Modifier = Modifier, enabled: Boolean = true, shape: Shape = RoundedCornerShape(6.dp), content: @Composable (RowScope.() -> Unit)) {
    Button(onClick, modifier, enabled, shape, content = content)
}