package com.vanbrusselgames.mindmix

import org.junit.Test
import kotlin.time.measureTime

/**
 * Example local unit test, which will execute on the development machine (host).
 *
 * See [testing documentation](http://d.android.com/tools/testing).
 */
class ExampleUnitTest {
    @Test
    fun benchmark() {
        println("Start Test!\n")
        val times = 1_000_000f
        var t = 0
        val elapsed = measureTime {
            while (t < times) {
                // assert(input == output)
                t++
            }
        }
        println("elapsed Time: $elapsed, average (μs): ${elapsed.inWholeMicroseconds / times}")
        println("\nEnd Test!\n")
    }
}