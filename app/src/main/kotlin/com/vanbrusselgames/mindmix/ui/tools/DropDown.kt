package com.vanbrusselgames.mindmix.ui.tools

import androidx.compose.foundation.layout.IntrinsicSize
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.widthIn
import androidx.compose.material3.DropdownMenuItem
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.ExposedDropdownMenuBox
import androidx.compose.material3.ExposedDropdownMenuDefaults
import androidx.compose.material3.Text
import androidx.compose.material3.TextField
import androidx.compose.runtime.Composable
import androidx.compose.runtime.MutableState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.dp
import com.vanbrusselgames.mindmix.utils.constants.Difficulty

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun DifficultyDropdown(
    modifier: Modifier = Modifier,
    difficultyState: MutableState<Difficulty>,
    setDifficultyCallback: (Difficulty) -> Unit,
    difficultyOptions: List<Difficulty>
) {
    var dropdownExpanded by remember { mutableStateOf(false) }
    ExposedDropdownMenuBox(
        dropdownExpanded,
        onExpandedChange = { dropdownExpanded = !dropdownExpanded },
        modifier.widthIn(1.dp, Dp.Infinity)
    ) {
        difficultyState.value.getStringResource()
        TextField(stringResource(difficultyState.value.getStringResource()),
            {},
            Modifier
                .menuAnchor()
                .widthIn(1.dp, Dp.Infinity),
            readOnly = true,
            singleLine = true,
            trailingIcon = {
                ExposedDropdownMenuDefaults.TrailingIcon(dropdownExpanded)
            })
        ExposedDropdownMenu(
            expanded = dropdownExpanded,
            onDismissRequest = { dropdownExpanded = false },
            modifier.height(IntrinsicSize.Min)
        ) {
            for (option in difficultyOptions) {
                DropdownMenuItem(text = { Text(stringResource(option.getStringResource())) },
                    onClick = {
                        setDifficultyCallback(option)
                        dropdownExpanded = false
                    })
            }
        }
    }
}