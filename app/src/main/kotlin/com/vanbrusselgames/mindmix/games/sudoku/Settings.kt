package com.vanbrusselgames.mindmix.games.sudoku

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
import androidx.compose.material3.Checkbox
import androidx.compose.material3.CheckboxDefaults
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.vanbrusselgames.mindmix.PREF_KEY_SUDOKU__AUTO_EDIT_NOTES
import com.vanbrusselgames.mindmix.PREF_KEY_SUDOKU__CHECK_CONFLICTING_CELLS
import com.vanbrusselgames.mindmix.R
import com.vanbrusselgames.mindmix.Settings
import com.vanbrusselgames.mindmix.savePreferences

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

            val autoEditNotesTicked = remember { mutableStateOf(SudokuManager.autoEditNotes) }
            Button(
                { autoEditNotesTicked.value = !autoEditNotesTicked.value },
                Modifier.fillMaxWidth(),
                shape = RoundedCornerShape(6.dp)
            ) {
                Row(
                    Modifier.heightIn(max = 36.dp), verticalAlignment = Alignment.CenterVertically
                ) {
                    Text(stringResource(R.string.auto_edit_notes))
                    Spacer(Modifier.width(8.dp))
                    Spacer(Modifier.weight(1f))
                    Checkbox(checked = autoEditNotesTicked.value, onCheckedChange = {
                        autoEditNotesTicked.value = it
                        SudokuManager.autoEditNotes = it
                        savePreferences(ctx, PREF_KEY_SUDOKU__AUTO_EDIT_NOTES, value = it)
                    }, colors = CheckboxDefaults.colors().copy(uncheckedBorderColor = MaterialTheme.colorScheme.onPrimary))
                }
            }

            Spacer(Modifier.height(2.dp))

            val checkConflictingCellsTicked =
                remember { mutableStateOf(SudokuManager.checkConflictingCells) }
            Button(
                { checkConflictingCellsTicked.value = !checkConflictingCellsTicked.value },
                Modifier.fillMaxWidth(),
                shape = RoundedCornerShape(6.dp)
            ) {
                Row(
                    Modifier.heightIn(max = 36.dp), verticalAlignment = Alignment.CenterVertically
                ) {
                    Text(stringResource(R.string.double_number_warning))
                    Spacer(Modifier.width(8.dp))
                    Spacer(Modifier.weight(1f))
                    Checkbox(checked = checkConflictingCellsTicked.value, onCheckedChange = {
                        checkConflictingCellsTicked.value = it
                        SudokuManager.checkConflictingCells = it
                        savePreferences(ctx, PREF_KEY_SUDOKU__CHECK_CONFLICTING_CELLS, value = it)

                        SudokuManager.cells.forEach { c ->
                            if (SudokuManager.checkConflictingCells) {
                                SudokuManager.checkConflictingCell(c.id)
                            } else {
                                c.isIncorrect = false
                            }
                        }
                    }, colors = CheckboxDefaults.colors().copy(uncheckedBorderColor = MaterialTheme.colorScheme.onPrimary))
                }
            }
        }
    }
}

@Preview(locale = "nl")
@Preview
@Composable
fun Prev_Settings_Screen() {
    Settings.visible.value = true
    SudokuSettings()
}