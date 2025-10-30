package com.vanbrusselgames.mindmix.feature.menu.model

import androidx.compose.animation.core.Animatable
import androidx.compose.animation.core.Spring
import androidx.compose.animation.core.spring
import com.vanbrusselgames.mindmix.feature.menu.viewmodel.IMenuScreenViewModel
import kotlin.math.roundToInt

data class GameWheel(val viewModel: IMenuScreenViewModel, val gameCount: Int) {
    private val withDuplicates = true
    val wheelItemCount = gameCount * if (withDuplicates) 2 else 1
    val angleStep = 360f / wheelItemCount
    val radius = 200f

    var selectedIndex =
        viewModel.games.filter { it.value == viewModel.selectedGame.value }.keys.first()
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

        // Set selected index and game to improve Accessibility
        val unsafeCurrentItem = (rotationAngle / angleStep).roundToInt()
        selectedIndex = (unsafeCurrentItem.mod(wheelItemCount) + wheelItemCount).mod(wheelItemCount)
        val wheelItem = items[selectedIndex]
        viewModel.selectedGame.value = wheelItem.game

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
        viewModel.selectedGame.value = wheelItem.game
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
}