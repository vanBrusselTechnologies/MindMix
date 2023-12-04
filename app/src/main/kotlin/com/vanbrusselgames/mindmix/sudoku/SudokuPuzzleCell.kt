package com.vanbrusselgames.mindmix.sudoku

import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.mutableStateOf

data class SudokuPuzzleCell(
    val id: Int,
    val isClue: Boolean,
    private var _value: Int = 0,
    var notes: Array<Boolean> = Array(9) { false }
) {
    private var _isIncorrect = false
    val mutableIsIncorrect = mutableStateOf(_isIncorrect)
    var isIncorrect
        get() = _isIncorrect
        set(value) {
            mutableIsIncorrect.value = value
            _isIncorrect = value
        }
    private var _isSelected = false
    val mutableIsSelected = mutableStateOf(_isSelected)
    var isSelected
        get() = _isSelected
        set(value) {
            mutableIsSelected.value = value
            _isSelected = value
        }
    val mutableCellValue = mutableStateOf(_value)
    var value
        get() = _value
        set(value) {
            mutableCellValue.value = value
            _value = value
        }
    val mutableCellNotes = mutableStateListOf(*notes)

    fun setNumber(value: Int) {
        _value = value
        mutableCellValue.value = value
    }

    fun setNote(value: Int) {
        val i = value - 1
        val noteSet = notes[i]
        notes[i] = !noteSet
        mutableCellNotes[i] = !noteSet
    }

    fun hasNote(index: Int = -1): Boolean = notes[index - 1]

    fun clearNotes(){
        notes.fill(false)
        mutableCellNotes.fill(false)
    }

    override fun equals(other: Any?): Boolean {
        if (this === other) return true
        if (javaClass != other?.javaClass) return false

        other as SudokuPuzzleCell

        if (id != other.id) return false

        return true
    }

    override fun hashCode(): Int {
        return id
    }
}
