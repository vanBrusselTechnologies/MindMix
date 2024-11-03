package com.vanbrusselgames.mindmix.games.sudoku

import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.mutableStateOf

data class SudokuPuzzleCell(
    val id: Int,
    private val _isClue: Boolean,
    private val _value: Int,
    val size: Int,
    private val _notes: Array<Boolean> = Array(size) { false }
) {
    var isClue = mutableStateOf(_isClue)
    var value = mutableIntStateOf(_value)
    val notes = mutableStateListOf(*_notes)
    var isIncorrect = mutableStateOf(false)
    var isSelected = mutableStateOf(false)

    fun setNote(value: Int) {
        val i = value - 1
        val noteSet = notes[i]
        notes[i] = !noteSet
    }

    fun hasNote(value: Int): Boolean = notes[value - 1]

    fun reset() {
        notes.fill(false)
        isClue.value = false
        isSelected.value = false
        isIncorrect.value = false
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
