package com.vanbrusselgames.mindmix.core.ui

import androidx.compose.foundation.layout.IntrinsicSize
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.widthIn
import androidx.compose.material3.DropdownMenuItem
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.ExposedDropdownMenuAnchorType
import androidx.compose.material3.ExposedDropdownMenuBox
import androidx.compose.material3.ExposedDropdownMenuDefaults
import androidx.compose.material3.Text
import androidx.compose.material3.TextField
import androidx.compose.runtime.Composable
import androidx.compose.runtime.State
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.dp
import com.vanbrusselgames.mindmix.core.utils.constants.StringEnum

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun <T : StringEnum> EnumDropdown(
    modifier: Modifier = Modifier, state: State<T>, setStateCallback: (T) -> Unit, options: List<T>
) {
    var dropdownExpanded by remember { mutableStateOf(false) }
    ExposedDropdownMenuBox(
        dropdownExpanded,
        onExpandedChange = { dropdownExpanded = !dropdownExpanded },
        Modifier.padding(4.dp)
    ) {
        TextField(
            stringResource(state.value.getStringResource()),
            {},
            Modifier
                .menuAnchor(ExposedDropdownMenuAnchorType.PrimaryNotEditable, true)
                .widthIn(1.dp, Dp.Infinity),
            readOnly = true,
            singleLine = true,
            trailingIcon = { ExposedDropdownMenuDefaults.TrailingIcon(dropdownExpanded) })
        ExposedDropdownMenu(
            dropdownExpanded,
            onDismissRequest = { dropdownExpanded = false },
            modifier.height(IntrinsicSize.Min)
        ) {
            for (option in options) {
                DropdownMenuItem(
                    text = { Text(stringResource(option.getStringResource())) },
                    onClick = {
                        setStateCallback(option)
                        dropdownExpanded = false
                    })
            }
        }
    }
}