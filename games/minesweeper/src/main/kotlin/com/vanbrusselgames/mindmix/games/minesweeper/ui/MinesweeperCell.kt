package com.vanbrusselgames.mindmix.games.minesweeper.ui

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.PaddingValues
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.text.BasicText
import androidx.compose.foundation.text.TextAutoSize
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.drawBehind
import androidx.compose.ui.draw.scale
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.AnnotatedString
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
        contentAlignment = Alignment.Center,
        modifier = Modifier
            .size(cellSize)
            .padding(PaddingValues(0.5f.dp))
            .drawBehind {
                drawRect(if (cell.background.value == Color.Red) cs.errorContainer else cs.secondaryContainer)
            }) {
        MinesweeperCellContent(cell)
    }
}

@Composable
fun MinesweeperCellContent(cell: MinesweeperCell) {
    when (cell.mutableCellState.value) {
        CellState.Empty -> return
        CellState.Bomb -> {
            if (cell.pressed) cell.background.value = Color.Red
            MinesweeperMineCell()
        }

        CellState.Flag -> MinesweeperFlagCell()
        CellState.Number -> MinesweeperTextCell(cell.mineCount)
    }
}

@Composable
private fun MinesweeperTextCell(value: Int) {
    BasicText(
        text = AnnotatedString(if (value == 0) "-" else value.toString()),
        modifier = Modifier.scale(1.125f),
        style = TextStyle(
            color = MaterialTheme.colorScheme.onSecondaryContainer,
            fontWeight = FontWeight.Bold,
            fontFeatureSettings = "tnum",
            textAlign = TextAlign.Center,
            lineHeightStyle = LineHeightStyle(
                alignment = LineHeightStyle.Alignment.Center, trim = LineHeightStyle.Trim.Both
            )
        ),
        maxLines = 1,
        autoSize = TextAutoSize.StepBased(maxFontSize = 250.sp, minFontSize = 5.sp)
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