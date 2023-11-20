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
        println("\n\n\n\nStart Test!\n")
        var t = 0
        val times = 100000
        val elapsed = measureTimeMillis {
            while (t < times) {
                t++
            }
        }
        println("average elapsed Time: ${elapsed / times}")
        println("\nEnd Test!\n\n\n\n")
    }
}