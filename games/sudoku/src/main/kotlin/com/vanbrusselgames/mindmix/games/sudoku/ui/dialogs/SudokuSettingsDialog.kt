package com.vanbrusselgames.mindmix.games.sudoku.ui.dialogs

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.IntrinsicSize
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.heightIn
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.Checkbox
import androidx.compose.material3.CheckboxDefaults
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import androidx.navigation.NavController
import androidx.navigation.compose.rememberNavController
import com.vanbrusselgames.mindmix.core.ui.EnumDropdown
import com.vanbrusselgames.mindmix.core.utils.constants.Difficulty
import com.vanbrusselgames.mindmix.feature.settings.SettingsDialog
import com.vanbrusselgames.mindmix.games.sudoku.R
import com.vanbrusselgames.mindmix.games.sudoku.model.Sudoku
import com.vanbrusselgames.mindmix.games.sudoku.model.enabledDifficulties
import com.vanbrusselgames.mindmix.games.sudoku.viewmodel.ISudokuViewModel
import com.vanbrusselgames.mindmix.games.sudoku.viewmodel.MockSudokuViewModel

@Composable
fun SudokuSettingsDialog(viewModel: ISudokuViewModel, navController: NavController) {
    SettingsDialog(navController) {
        Column(
            Modifier
                .padding(24.dp)
                .width(IntrinsicSize.Max),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            Text(
                text = stringResource(R.string.sudoku_name),
                Modifier.fillMaxWidth(),
                fontSize = 36.sp,
                fontWeight = FontWeight.ExtraBold,
                textAlign = TextAlign.Center
            )
            val loadedState = viewModel.preferencesLoaded.collectAsStateWithLifecycle()
            if (!loadedState.value) return@SettingsDialog
            Spacer(Modifier.height(20.dp))
            DifficultyDropdownRow(viewModel)
            Spacer(Modifier.height(2.dp))
            Button(
                { viewModel.onClickUpdateAutoEditNotes() },
                Modifier.fillMaxWidth(),
                shape = RoundedCornerShape(6.dp)
            ) {
                Row(
                    Modifier.heightIn(max = 36.dp), verticalAlignment = Alignment.CenterVertically
                ) {
                    Text(stringResource(R.string.auto_edit_notes))
                    Spacer(Modifier.weight(1f))
                    Checkbox(
                        checked = viewModel.autoEditNotes.value,
                        onCheckedChange = { viewModel.onClickUpdateAutoEditNotes() },
                        colors = CheckboxDefaults.colors()
                            .copy(uncheckedBorderColor = MaterialTheme.colorScheme.onPrimary)
                    )
                }
            }
            Spacer(Modifier.height(2.dp))
            Button(
                { viewModel.onClickUpdateCheckConflictingCells() },
                Modifier.fillMaxWidth(),
                shape = RoundedCornerShape(6.dp)
            ) {
                Row(
                    Modifier.heightIn(max = 36.dp), verticalAlignment = Alignment.CenterVertically
                ) {
                    Text(stringResource(R.string.double_number_warning))
                    Spacer(Modifier.weight(1f))
                    Checkbox(
                        checked = viewModel.checkConflictingCells.value,
                        onCheckedChange = { viewModel.onClickUpdateCheckConflictingCells() },
                        colors = CheckboxDefaults.colors()
                            .copy(uncheckedBorderColor = MaterialTheme.colorScheme.onPrimary)
                    )
                }
            }
        }
    }
}

@Composable
fun DifficultyDropdownRow(viewModel: ISudokuViewModel) {
    val defaultButtonColors = ButtonDefaults.buttonColors()
    Row(
        Modifier
            .fillMaxWidth()
            .background(defaultButtonColors.containerColor, RoundedCornerShape(6.dp)),
        Arrangement.Center,
        Alignment.CenterVertically
    ) {
        Text(
            stringResource(R.string.difficulty_label),
            Modifier.padding(20.dp),
            defaultButtonColors.contentColor
        )
        Spacer(Modifier.weight(1f))
        val modifier = Modifier
            .padding(4.dp)
            .width(IntrinsicSize.Min)
        val callback = { diff: Difficulty -> viewModel.setDifficulty(diff) }
        EnumDropdown(modifier, viewModel.difficulty, callback, enabledDifficulties)
    }
}

@Preview(locale = "nl")
@Preview
@Composable
fun Prev_Settings() {
    val vm = remember { MockSudokuViewModel() }
    SudokuSettingsDialog(vm, rememberNavController())
}