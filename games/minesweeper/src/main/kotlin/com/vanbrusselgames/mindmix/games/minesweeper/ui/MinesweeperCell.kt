package com.vanbrusselgames.mindmix.games.minesweeper.ui

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.PaddingValues
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.material3.Icon
import androidx.compose.material3.LocalTextStyle
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.drawBehind
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.TextStyle
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.LineHeightStyle
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.Dp
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.vanbrusselgames.mindmix.games.minesweeper.R
import com.vanbrusselgames.mindmix.games.minesweeper.model.CellState
import com.vanbrusselgames.mindmix.games.minesweeper.model.MinesweeperCell

@Composable
fun MinesweeperCell(cell: MinesweeperCell, cellSize: Dp) {
    val cs = MaterialTheme.colorScheme
    Box(
        Modifier
            .size(cellSize)
            .padding(PaddingValues(0.5.dp))
            .drawBehind {
                drawRect(if (cell.background.value == Color.Red) cs.errorContainer else cs.secondaryContainer)
            }, contentAlignment = Alignment.Center
    ) {
        MinesweeperCellContent(cell, cellSize)
    }
}

@Composable
fun MinesweeperCellContent(cell: MinesweeperCell, cellSize: Dp) {
    when (cell.mutableCellState.value) {
        CellState.Empty -> return
        CellState.Bomb -> {
            if (cell.pressed) cell.background.value = Color.Red
            MinesweeperMineCell()
        }

        CellState.Flag -> MinesweeperFlagCell()
        CellState.Number -> MinesweeperTextCell(cell.mineCount, cellSize)
    }
}

@Composable
private fun MinesweeperTextCell(value: Int, cellSize: Dp) {
    val style = remember {
        TextStyle(
            fontWeight = FontWeight.Bold,
            fontFeatureSettings = "tnum",
            textAlign = TextAlign.Center,
            lineHeightStyle = LineHeightStyle(
                alignment = LineHeightStyle.Alignment.Center, trim = LineHeightStyle.Trim.Both
            )
        )
    }

    val valueText = when (value) {
        0 -> "-"
        1 -> "1"
        2 -> "2"
        3 -> "3"
        4 -> "4"
        5 -> "5"
        6 -> "6"
        7 -> "7"
        8 -> "8"
        else -> value.toString()
    }
    val fontSize = remember(cellSize) { (cellSize * 0.875f).value.sp }

    Text(
        text = valueText,
        color = MaterialTheme.colorScheme.onSecondaryContainer,
        fontSize = fontSize,
        maxLines = 1,
        style = LocalTextStyle.current.merge(style)
    )
}

@Composable
fun MinesweeperFlagCell() {
    Icon(painterResource(R.drawable.baseline_flag_24), "Flag", Modifier.fillMaxSize())
}

@Composable
fun MinesweeperMineCell() {
    Icon(painterResource(R.drawable.outline_bomb_24), "Bomb", Modifier.fillMaxSize())
}