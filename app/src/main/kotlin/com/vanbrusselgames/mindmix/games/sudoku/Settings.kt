package com.vanbrusselgames.mindmix.games.sudoku

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
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.vanbrusselgames.mindmix.BaseUIHandler
import com.vanbrusselgames.mindmix.PREF_KEY_SUDOKU__AUTO_EDIT_NOTES
import com.vanbrusselgames.mindmix.PREF_KEY_SUDOKU__CHECK_CONFLICTING_CELLS
import com.vanbrusselgames.mindmix.R
import com.vanbrusselgames.mindmix.Settings
import com.vanbrusselgames.mindmix.savePreferences
import com.vanbrusselgames.mindmix.ui.tools.DifficultyDropdown
import com.vanbrusselgames.mindmix.utils.constants.Difficulty

@Composable
fun SudokuSettings() {
    Settings.Screen {
        Column(
            Modifier
                .padding(24.dp)
                .width(IntrinsicSize.Max),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            val ctx = LocalContext.current

            Text(
                text = stringResource(R.string.sudoku_name),
                Modifier.fillMaxWidth(),
                fontSize = 36.sp,
                fontWeight = FontWeight.ExtraBold,
                textAlign = TextAlign.Center
            )
            Spacer(Modifier.height(20.dp))

            DifficultyDropdownRow()

            Spacer(Modifier.height(2.dp))

            Button(
                { SudokuManager.autoEditNotes.value = !SudokuManager.autoEditNotes.value },
                Modifier.fillMaxWidth(),
                shape = RoundedCornerShape(6.dp)
            ) {
                Row(
                    Modifier.heightIn(max = 36.dp), verticalAlignment = Alignment.CenterVertically
                ) {
                    Text(stringResource(R.string.auto_edit_notes))
                    Spacer(Modifier.weight(1f))
                    Checkbox(
                        checked = SudokuManager.autoEditNotes.value,
                        onCheckedChange = {
                            SudokuManager.autoEditNotes.value = it
                            savePreferences(ctx, PREF_KEY_SUDOKU__AUTO_EDIT_NOTES, value = it)
                        },
                        colors = CheckboxDefaults.colors()
                            .copy(uncheckedBorderColor = MaterialTheme.colorScheme.onPrimary)
                    )
                }
            }

            Spacer(Modifier.height(2.dp))

            Button(
                {
                    SudokuManager.checkConflictingCells.value =
                        !SudokuManager.checkConflictingCells.value
                }, Modifier.fillMaxWidth(), shape = RoundedCornerShape(6.dp)
            ) {
                Row(
                    Modifier.heightIn(max = 36.dp), verticalAlignment = Alignment.CenterVertically
                ) {
                    Text(stringResource(R.string.double_number_warning))
                    Spacer(Modifier.weight(1f))
                    Checkbox(
                        checked = SudokuManager.checkConflictingCells.value,
                        onCheckedChange = { checked ->
                            SudokuManager.checkConflictingCells.value = checked
                            savePreferences(
                                ctx, PREF_KEY_SUDOKU__CHECK_CONFLICTING_CELLS, value = checked
                            )

                            if (checked) {
                                SudokuManager.cells.forEach { SudokuManager.checkConflictingCell(it) }
                            } else SudokuManager.cells.forEach { it.isIncorrect.value = false }
                        },
                        colors = CheckboxDefaults.colors()
                            .copy(uncheckedBorderColor = MaterialTheme.colorScheme.onPrimary)
                    )
                }
            }
        }
    }
}

@Composable
fun DifficultyDropdownRow() {
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
        val callback = { diff: Difficulty -> SudokuManager.setDifficulty(diff) }
        DifficultyDropdown(modifier, SudokuManager.difficulty, callback, enabledDifficulties)
    }
}

@Preview(locale = "nl")
@Preview
@Composable
fun Prev_Settings_Screen() {
    BaseUIHandler.openSettings()
    SudokuSettings()
}