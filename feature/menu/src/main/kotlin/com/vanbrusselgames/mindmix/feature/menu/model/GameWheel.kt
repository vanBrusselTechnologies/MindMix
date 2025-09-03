package com.vanbrusselgames.mindmix.feature.menu.model

import androidx.compose.animation.core.Animatable
import androidx.compose.animation.core.Spring
import androidx.compose.animation.core.spring
import androidx.compose.ui.platform.WindowInfo
import androidx.compose.ui.unit.Density
import com.vanbrusselgames.mindmix.feature.menu.viewmodel.IMenuScreenViewModel
import kotlin.math.roundToInt

data class GameWheel(val viewModel: IMenuScreenViewModel, val gameCount: Int) {
    private val withDuplicates = true
    val wheelItemCount = gameCount * if (withDuplicates) 2 else 1
    val angleStep = 360f / wheelItemCount
    val radius = 275f

    var selectedIndex = viewModel.games.filter { it.value == viewModel.selectedGame }.keys.first()
        private set
    var rotationAngle = selectedIndex * angleStep
        private set

    val anim = Animatable(rotationAngle)

    val items = List(wheelItemCount) { i ->
        val item = WheelItem(viewModel.games[i % gameCount]!!, radius, i * angleStep)
        item.isSelected.value = i == selectedIndex

        item
    }

    fun startRotate() {
        items.forEach { it.isSelected.value = false }
    }

    suspend fun rotate(delta: Float) {
        rotationAngle += delta / 8f
        rotateToTarget()
    }

    private suspend fun rotateToTarget() {
        anim.animateTo(
            rotationAngle,
            spring(Spring.DampingRatioLowBouncy, Spring.StiffnessVeryLow),
            block = { setItemVisibility() })
    }

    suspend fun setSelectedStartIndex(index: Int) {
        selectedIndex = index
        rotationAngle = index * angleStep
        anim.snapTo(rotationAngle)
        items.forEachIndexed { i, it -> it.isSelected.value = i == selectedIndex }
        setItemVisibility()
    }

    suspend fun updateSelectedIndex() {
        val unsafeCurrentItem = (rotationAngle / angleStep).roundToInt()
        rotationAngle = unsafeCurrentItem * angleStep
        selectedIndex = (unsafeCurrentItem.mod(wheelItemCount) + wheelItemCount).mod(wheelItemCount)
        val wheelItem = items[selectedIndex]
        wheelItem.isSelected.value = true
        viewModel.selectedGame = wheelItem.game
        rotateToTarget()
    }

    fun setItemVisibility() {
        val angle = anim.value
        val minimum = (angle - 90).mod(360f)
        val maximum = (angle + 90).mod(360f)
        for (wheelItem in items) {
            val a = wheelItem.angle
            wheelItem.visible.value =
                (maximum < minimum && (a > minimum || a < maximum)) || (maximum >= minimum && a in minimum..maximum)
        }
    }

    fun setGrowthFactor(localDensity: Density, localWindowInfo: WindowInfo) {
        val containerSize = localWindowInfo.containerSize
        val growthFactor = with(localDensity) {
            val screenHeight = containerSize.height.toDp().value
            val screenWidth = containerSize.width.toDp().value
            val minScreenSize = screenHeight.coerceAtMost(screenWidth) - 300f
            100f.coerceAtMost(minScreenSize)
        }
        items.forEach {
            it.growthFactor = growthFactor
            it.offsetY = -60f * (growthFactor / 100f)
        }
    }
}