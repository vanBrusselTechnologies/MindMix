package com.vanbrusselgames.mindmix.games.sudoku

import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.mutableStateOf

data class SudokuPuzzleCell(
    val id: Int,
    private val _isClue: Boolean,
    private val _value: Int = 0,
    val size: Int = 9,
    val notes: Array<Boolean> = Array(size) { false }
) {

    val mutableIsIncorrect = mutableStateOf(false)
    var isIncorrect = false
        set(value) {
            field = value
            mutableIsIncorrect.value = value
        }

    val mutableIsSelected = mutableStateOf(false)
    var isSelected = false
        set(value) {
            field = value
            mutableIsSelected.value = value
        }

    val mutableCellValue = mutableIntStateOf(_value)
    var value = _value
        set(value) {
            field = value
            mutableCellValue.intValue = value
        }
    val mutableCellNotes = mutableStateListOf(*notes)

    val mutableIsClue = mutableStateOf(_isClue)
    var isClue = _isClue
        set(value) {
            field = value
            mutableIsClue.value = _isClue
        }

    fun setNote(value: Int) {
        val i = value - 1
        val noteSet = notes[i]
        notes[i] = !noteSet
        mutableCellNotes[i] = !noteSet
    }

    fun hasNote(value: Int): Boolean = notes[value - 1]

    fun reset() {
        notes.fill(false)
        mutableCellNotes.fill(false)
        isClue = false
        isSelected = false
        isIncorrect = false
    }

    override fun equals(other: Any?): Boolean {
        if (this === other) return true
        if (javaClass != other?.javaClass) return false

        other as SudokuPuzzleCell

        return id == other.id
    }

    override fun hashCode(): Int {
        return id
    }
}
