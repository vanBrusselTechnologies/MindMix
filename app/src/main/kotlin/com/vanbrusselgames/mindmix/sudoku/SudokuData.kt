package com.vanbrusselgames.mindmix.sudoku

object SudokuData {
    var Clues = intArrayOf()
    var Input = IntArray(81){0}
    var InputNotes = Array(81){IntArray(9){0} }
    var CheckConflictingCells = true
    var AutoEditNotes = true
    var Finised = false
}