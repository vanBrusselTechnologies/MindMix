package com.vanbrusselgames.mindmix.games.sudoku.model

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
    val isClue = mutableStateOf(_isClue)
    val value = mutableIntStateOf(_value)
    val notes = mutableStateListOf(*_notes)
    val isIncorrect = mutableStateOf(false)
    val isSelected = mutableStateOf(false)

    fun setNote(value: Int) {
        val i = value - 1
        val noteSet = notes[i]
        notes[i] = !noteSet
    }

    fun hasNote(value: Int): Boolean = notes[value - 1]

    fun reset() {
        isClue.value = false
        value.intValue = 0
        notes.fill(false)
        isIncorrect.value = false
        isSelected.value = false
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
