package com.vanbrusselgames.mindmix


import org.junit.Test
import kotlin.system.measureTimeMillis

/**
 * Example local unit test, which will execute on the development machine (host).
 *
 * See [testing documentation](http://d.android.com/tools/testing).
 */
class ExampleUnitTest {
    @Test
    fun benchmark() {

        val times = 1_000_000

        println("Start Test!\n")
        var t = 0
        val elapsed = measureTimeMillis {
            while (t < times) {
                t++
            }
        }

        println("(average) elapsed Time: ${elapsed / times}")
        println("\nEnd Test!\n")

    }
}