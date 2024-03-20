package com.vanbrusselgames.mindmix.games.sudoku

import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.mutableStateOf

data class SudokuPuzzleCell(
    val id: Int,
    var isClue: Boolean,
    private var _value: Int = 0,
    val size: Int = 9,
    val notes: Array<Boolean> = Array(size) { false }
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
    val mutableCellValue = mutableIntStateOf(_value)
    var value
        get() = _value
        set(value) = setNumber(value)
    val mutableCellNotes = mutableStateListOf(*notes)

    fun setNumber(value: Int) {
        _value = value
        mutableCellValue.intValue = value
    }

    fun setNote(value: Int) {
        val i = value - 1
        val noteSet = notes[i]
        notes[i] = !noteSet
        mutableCellNotes[i] = !noteSet
    }

    fun hasNote(value: Int): Boolean = notes[value - 1]

    fun reset(){
        setNumber(0)
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
