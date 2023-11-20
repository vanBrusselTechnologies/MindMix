package com.vanbrusselgames.mindmix.sudoku

import androidx.compose.runtime.MutableState
import androidx.compose.runtime.mutableStateOf

class SudokuManager {
    companion object Instance {
        private lateinit var solution: IntArray
        lateinit var clues: Array<Int>
        var inputMode: InputMode = InputMode.Normal
        private lateinit var puzzle: SudokuPuzzle
        val sudokuFinished = mutableStateOf(false)

        enum class PuzzleType {
            Classic
        }

        enum class InputMode {
            Normal, Note
        }

        fun loadPuzzle() {
            if (SudokuData.Clues.isEmpty()) {
                puzzle = createPuzzle()
                solution = SudokuPuzzle.getSolution(puzzle)
                SudokuData.Clues = SudokuPuzzle.createClues(puzzle, 60)
                clues = SudokuData.Clues.toTypedArray()
            } else {
                solution = SudokuPuzzle.getSolution(SudokuData.Clues)
                clues = SudokuData.Clues.toTypedArray()
                puzzle = SudokuPuzzle(solution)
            }
        }

        private fun createPuzzle(
            type: PuzzleType = PuzzleType.Classic, size: Int = 9
        ): SudokuPuzzle {
            return if (type == PuzzleType.Classic) SudokuPuzzle.randomGrid(size)
            else SudokuPuzzle.randomGrid(0)
        }

        fun autoChangeNotes(index: Int, cellRememberValueList: Array<MutableState<Int>>) {
            if (index >= 81) return
            if (SudokuData.Input[index] == 0) return
            val indices: IntArray = puzzle.peers(index)
            val number: Int = SudokuData.Input[index]
            for (n: Int in indices) {
                if (index == n) continue
                val note: Int = SudokuData.InputNotes[n][number - 1]
                if (note != number) continue
                SudokuData.InputNotes[n][number - 1] = 0
                if (SudokuData.Input[n] != 0) continue
                cellRememberValueList[n].value = -SudokuData.InputNotes[n].sum()
            }
        }

        fun checkConflictingCell(
            index: Int = 0,
            cellColorList: Array<MutableState<Int>>,
            selectedCellIndex: MutableState<Int>,
            isSecondary: Boolean = false
        ) {
            if (index >= 81) return
            val indices: IntArray = puzzle.peers(index)
            var isConflicting = false
            for (n: Int in indices) {
                if (index == n) continue
                if (SudokuData.Input[index] == 0) {
                    var i = 0
                    while (i < 9) {
                        val note: Int = SudokuData.InputNotes[index][i]
                        i++
                        if (note == 0 || (note != SudokuData.Input[n] && note != clues[n])) continue
                        cellColorList[index].value = -1
                        return
                    }
                } else if (SudokuData.Input[index] == SudokuData.Input[n] || SudokuData.Input[index] == clues[n]) {
                    cellColorList[index].value = -1
                    isConflicting = true
                    if (!isSecondary) checkConflictingCell(
                        n, cellColorList, selectedCellIndex, true
                    )
                } else if (!isSecondary && cellColorList[n].value == -1) {
                    checkConflictingCell(n, cellColorList, selectedCellIndex, true)
                }
            }
            if (!isConflicting) cellColorList[index].value =
                if (selectedCellIndex.value == index) 1 else 0
        }

        fun checkFinished() {
            SudokuData.Finised = isFinished()
            sudokuFinished.value = SudokuData.Finised
        }

        private fun isFinished(): Boolean{
            try {
                var i = 0
                val input = IntArray(SudokuData.Input.size){0}
                while (i < SudokuData.Input.size){
                    input[i] = SudokuData.Input[i] + SudokuData.Clues[i]
                    if(input[i] == 0) {
                        return false
                    }
                    i++
                }
                val solution = SudokuPuzzle.getSolution(input)
                return solution.contentEquals(input)
            } catch(_:Exception) {
                return false
            }
        }
    }
}