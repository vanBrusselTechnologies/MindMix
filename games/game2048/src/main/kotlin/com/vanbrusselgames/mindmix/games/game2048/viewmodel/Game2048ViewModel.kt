package com.vanbrusselgames.mindmix.games.game2048.viewmodel

import android.app.Activity
import androidx.compose.runtime.MutableState
import androidx.compose.runtime.mutableLongStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.snapshots.SnapshotStateList
import androidx.compose.ui.geometry.Offset
import androidx.compose.ui.util.fastAny
import androidx.compose.ui.util.fastFilter
import androidx.compose.ui.util.fastForEach
import androidx.compose.ui.util.fastRoundToInt
import androidx.lifecycle.viewModelScope
import androidx.navigation.NavController
import com.google.firebase.analytics.FirebaseAnalytics
import com.vanbrusselgames.mindmix.core.advertisement.AdManager
import com.vanbrusselgames.mindmix.core.common.viewmodel.BaseGameViewModel
import com.vanbrusselgames.mindmix.core.games.ui.minimumDurationLoadingScreen
import com.vanbrusselgames.mindmix.core.logging.Logger
import com.vanbrusselgames.mindmix.core.model.SceneRegistry
import com.vanbrusselgames.mindmix.games.game2048.data.Game2048Repository
import com.vanbrusselgames.mindmix.games.game2048.data.preferences.Game2048Preferences
import com.vanbrusselgames.mindmix.games.game2048.data.preferences.Game2048PreferencesRepository
import com.vanbrusselgames.mindmix.games.game2048.model.FinishedGame
import com.vanbrusselgames.mindmix.games.game2048.model.Game2048Progress
import com.vanbrusselgames.mindmix.games.game2048.model.GridCell2048
import com.vanbrusselgames.mindmix.games.game2048.model.GridSize2048
import com.vanbrusselgames.mindmix.games.game2048.model.SuccessType
import com.vanbrusselgames.mindmix.games.game2048.navigation.navigateToGame2048GameFinished
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.combine
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.flow.onStart
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import javax.inject.Inject
import kotlin.math.abs
import kotlin.math.log2
import kotlin.math.pow
import kotlin.random.Random

@HiltViewModel
class Game2048ViewModel @Inject constructor(
    private val adManager: AdManager,
    private val game2048Repository: Game2048Repository,
    private val prefsRepository: Game2048PreferencesRepository
) : BaseGameViewModel(), IGame2048ViewModel {
    override val finishedGame = mutableStateOf(FinishedGame())
    override val gridSize = mutableStateOf(GridSize2048.FOUR)
    override val cellList =
        SnapshotStateList(gridSize.value.getMaxCellCount()) { GridCell2048(it, 0) }
    override val score = mutableLongStateOf(0L)

    private val _preferencesLoaded = MutableStateFlow(false)
    override val preferencesLoaded = _preferencesLoaded.onStart { loadPreferences() }
        .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000L), false)

    private val _minTimeElapsed = MutableStateFlow(false)

    private val _puzzleLoaded = MutableStateFlow(false)
    override val puzzleLoaded =
        _puzzleLoaded.onStart { loadData() }.combine(_minTimeElapsed) { a, b -> a && b }
            .stateIn(viewModelScope, SharingStarted.WhileSubscribed(5000L), false)

    override var finished = false

    private val threshold = 100

    private var cellCount = gridSize.value.getMaxCellCount()
    private var sideSize = gridSize.value.getSize()
    private var newId = cellCount
    private var saveJob: Job? = null

    private suspend fun loadPreferences() {
        applyPreferences(prefsRepository.getPreferences().first())
    }

    private suspend fun applyPreferences(preferences: Game2048Preferences) {
        Logger.d("[2048] applyPreferences")
        val size = GridSize2048.entries[preferences.size]
        gridSize.value = size
        sideSize = size.getSize()
        cellCount = size.getMaxCellCount()
        _preferencesLoaded.emit(true)
    }

    private fun guaranteeMinimumDuration() {
        viewModelScope.launch {
            _minTimeElapsed.value = false
            delay(minimumDurationLoadingScreen)
            _minTimeElapsed.value = true
        }
    }

    private suspend fun loadData() {
        withContext(Dispatchers.IO) {
            preferencesLoaded.first { it }
            guaranteeMinimumDuration()
            onPuzzleLoaded(
                game2048Repository.getPuzzleProgress(gridSize.value)
                    ?: game2048Repository.createNewPuzzle(gridSize.value)
            )
        }
    }

    private fun getTarget(): Long = when (gridSize.value) {
        GridSize2048.THREE -> 256
        GridSize2048.FOUR -> 2048
        GridSize2048.FIVE -> 8192
        GridSize2048.SIX -> 32768
        GridSize2048.SEVEN -> 131072
    }

    private fun getHighestTileValue() = cellList.maxOf { it.value }

    private fun reset() {
        finished = false
        cellList.forEach { it.value = 0 }
        score.longValue = 0L
        _puzzleLoaded.value = false
    }

    override fun startNewGame() {
        reset()
        game2048Repository.removeProgressForSize(gridSize.value)
        startPuzzle()
        Logger.logEvent(FirebaseAnalytics.Event.LEVEL_START) {
            param(FirebaseAnalytics.Param.LEVEL_NAME, SceneRegistry.Game2048.name)
        }
    }

    private fun startPuzzle() {
        Logger.d("[2048] startPuzzle")
        if (finished) reset()
        if (_puzzleLoaded.value) return
        guaranteeMinimumDuration()
        onPuzzleLoaded(
            game2048Repository.getPuzzleProgress(gridSize.value)
                ?: game2048Repository.createNewPuzzle(gridSize.value)
        )
    }

    private fun onPuzzleLoaded(p: Game2048Progress?) {
        Logger.d("[2048] onPuzzleLoaded")
        if (p == null) {
            _puzzleLoaded.value = false
            return
        }
        cellList.clear()
        cellList.addAll(p.cellValues.map { GridCell2048(newId++, it) })
        //moves = p.moves
        score.longValue = p.score
        _puzzleLoaded.value = true
    }

    override fun handleDragGestures(navController: NavController, totalDragOffset: Offset) {
        if (finished) return
        val x = totalDragOffset.x
        val y = totalDragOffset.y
        if (abs(x) < threshold && abs(y) < threshold) return
        if (abs(x) > abs(y)) { // Horizontal
            if (x > 0) swipeRight(navController) else swipeLeft(navController)
        } else { // Vertical
            if (y > 0) swipeDown(navController) else swipeUp(navController)
        }

        saveJob?.cancel()
        saveJob = CoroutineScope(Dispatchers.IO).launch {
            delay(2000)
            if (finished) return@launch
            game2048Repository.setPuzzleProgressForSize(
                gridSize.value, cellList.map { it.value }, moves = -1, score.longValue
            )
        }
    }

    fun swipeUp(navController: NavController) {
        val lastValues = cellList.map { it.value }
        val columns = getColumns()
        combineEqualCells(navController, columns)

        val tempCellList = mutableListOf<GridCell2048>()
        getColumns().forEachIndexed { cId, column ->
            val cells = column.sortedBy { if (it.value == 0L) 1 else 0 }.toMutableList()
            while (cells.size < sideSize) {
                cells.add(GridCell2048(newId++, 0))
            }
            tempCellList.addAll(cells)
        }

        for (i in 0 until cellCount) cellList[i] = tempCellList[i]
        if (lastValues == cellList.map { it.value }) return
        tryAddCell(navController)
    }

    fun swipeDown(navController: NavController) {
        val lastValues = cellList.map { it.value }
        val columns = getColumns().map { it.reversed() }
        combineEqualCells(navController, columns)

        val tempCellList = mutableListOf<GridCell2048>()
        getColumns().forEachIndexed { cId, column ->
            val cells = column.sortedBy { if (it.value == 0L) 0 else 1 }.toMutableList()
            while (cells.size < sideSize) {
                cells.add(0, GridCell2048(newId++, 0))
            }
            tempCellList.addAll(cells)
        }

        for (i in 0 until cellCount) cellList[i] = tempCellList[i]
        if (lastValues == cellList.map { it.value }) return
        tryAddCell(navController)
    }

    fun swipeLeft(navController: NavController) {
        val lastValues = cellList.map { it.value }
        val rows = getRows()
        combineEqualCells(navController, rows)

        val newRows = getRows().mapIndexed { rId, row ->
            val cells = row.sortedBy { if (it.value == 0L) 1 else 0 }.toMutableList()
            while (cells.size < sideSize) {
                cells.add(GridCell2048(newId++, 0))
            }
            cells
        }

        var i = 0
        for (cId in 0 until sideSize) {
            for (rId in 0 until sideSize) {
                cellList[i] = newRows[rId][cId]
                i++
            }
        }

        if (lastValues == cellList.map { it.value }) return
        tryAddCell(navController)
    }

    fun swipeRight(navController: NavController) {
        val lastValues = cellList.map { it.value }
        val rows = getRows().map { it.reversed() }
        combineEqualCells(navController, rows)

        val newRows = getRows().mapIndexed { rId, row ->
            val cells = row.sortedBy { if (it.value == 0L) 0 else 1 }.toMutableList()
            while (cells.size < sideSize) {
                cells.add(0, GridCell2048(newId++, 0))
            }
            cells
        }

        var i = 0
        for (cId in 0 until sideSize) {
            for (rId in 0 until sideSize) {
                cellList[i] = newRows[rId][cId]
                i++
            }
        }

        if (lastValues == cellList.map { it.value }) return
        tryAddCell(navController)
    }

    private fun getColumns(): List<List<GridCell2048>> {
        return cellList.chunked(sideSize)
    }

    private fun getRows(): List<List<GridCell2048>> {
        val rows = List(sideSize) { mutableListOf<GridCell2048>() }
        getColumns().forEach { c ->
            c.forEachIndexed { rId, r ->
                rows[rId].add(r)
            }
        }
        return rows
    }

    private fun combineEqualCells(navController: NavController, cells: List<List<GridCell2048>>) {
        var reachedTarget = false
        var points = 0L
        for (r in cells) {
            var v = 0L
            var cId = -1
            for (c in r) {
                if (c.value == 0L) continue
                if (v == 0L || v != c.value) {
                    v = c.value
                    cId = c.id
                    continue
                }
                val cell = r.find { it.id == cId }
                if (cell == null) continue
                if (v * 2 == getTarget() && getHighestTileValue() == v) reachedTarget = true
                c.value = v * 2
                cell.value = 0
                points += v * 2
                v = 0
            }
        }
        addScore(points)
        if (reachedTarget) viewModelScope.launch {
            finished = true
            Logger.logEvent(FirebaseAnalytics.Event.LEVEL_END) {
                param(FirebaseAnalytics.Param.LEVEL_NAME, SceneRegistry.Game2048.name)
                param(FirebaseAnalytics.Param.SUCCESS, 1)
            }
            delay(500)
            onGameFinished(navController, true, false)
        }
    }

    private fun tryAddCell(navController: NavController) {
        val options = cellList.fastFilter { it.value == 0L }
        if (options.isEmpty()) {
            checkStuck(navController)
            return
        }
        val cell = options.random()
        cellList[cellList.indexOf(cell)] =
            GridCell2048(newId++, if (Random.Default.nextFloat() < 0.9) 2 else 4)
        checkStuck(navController)
    }

    private fun checkStuck(navController: NavController) {
        if (!canMove()) viewModelScope.launch {
            finished = true
            Logger.logEvent(FirebaseAnalytics.Event.LEVEL_END) {
                param(FirebaseAnalytics.Param.LEVEL_NAME, SceneRegistry.Game2048.name)
                param(
                    FirebaseAnalytics.Param.SUCCESS,
                    if (getHighestTileValue() >= getTarget()) 1 else 0
                )
            }
            game2048Repository.removeProgressForSize(gridSize.value)
            delay(1000)
            onGameFinished(navController, getHighestTileValue() >= getTarget(), true)
        }
    }

    private fun canMove(): Boolean {
        if (cellList.fastAny { it.value == 0L }) return true
        val rows = getRows()
        rows.fastForEach {
            var value = 0L
            for (c in it) {
                if (c.value == value) return true
                value = c.value
            }
        }
        val columns = getColumns()
        columns.fastForEach {
            var value = 0L
            for (c in it) {
                if (c.value == value) return true
                value = c.value
            }
        }
        return false
    }

    private fun addScore(points: Long) {
        score.longValue += points
    }

    private fun onGameFinished(
        navController: NavController, reachedTarget: Boolean, isStuck: Boolean
    ) {
        val successType = if (reachedTarget) {
            if (isStuck) SuccessType.SUCCESS
            else SuccessType.REACHED_TARGET
        } else SuccessType.GAME_OVER
        val reward = if (successType == SuccessType.SUCCESS) 10 + getBonusReward() else 0
        finishedGame.value = FinishedGame(
            successType, reward, getHighestTileValue(), getTarget(), score.longValue
        )
        navController.navigateToGame2048GameFinished()
    }

    private fun getBonusReward(): Int {
        return 1.5.pow((log2(getHighestTileValue().toDouble()) - log2(getTarget().toDouble())))
            .fastRoundToInt()
    }


    override fun continueGame() {
        finished = false
    }

    override fun forceSave() {
        game2048Repository.forceSave()
    }

    override fun checkAdLoaded(activity: Activity, adLoaded: MutableState<Boolean>) {
        adManager.checkAdLoaded(activity, adLoaded)
    }

    override fun showAd(
        activity: Activity, adLoaded: MutableState<Boolean>, onAdWatched: (Int) -> Unit
    ) {
        adManager.showAd(activity, adLoaded, onAdWatched)
    }

    override fun setSize(value: GridSize2048) {
        saveAndLoadProgress(gridSize.value, value)
        gridSize.value = value
        sideSize = value.getSize()
        cellCount = value.getMaxCellCount()
        viewModelScope.launch {
            prefsRepository.savePreferences(Game2048Preferences(value.ordinal))
        }
    }

    private fun saveAndLoadProgress(prevSize: GridSize2048, size: GridSize2048) {
        Logger.d("[2048] saveAndLoadProgress")
        if (prevSize == size) return
        if (_puzzleLoaded.value && !finished) game2048Repository.setPuzzleProgressForSize(
            prevSize, cellList.map { it.value }, 0, score.longValue
        )

        reset()
        guaranteeMinimumDuration()
        onPuzzleLoaded(
            game2048Repository.getPuzzleProgress(size) ?: game2048Repository.createNewPuzzle(size)
        )
    }
}