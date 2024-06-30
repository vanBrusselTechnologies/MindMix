package com.vanbrusselgames.mindmix.utils.encode

import java.math.BigInteger

class Encode {
    companion object {
        fun <T> base94(input: Collection<T>): String {
            var bigint = BigInteger.ZERO
            if (input.isEmpty()) return base94(bigint)
            val first = input.first()
            if (first is Int) {
                bigint = BigInteger(input.ifEmpty { listOf(0) }.joinToString(""))
            } else if (first is Boolean) {
                input as Collection<Boolean>
                for ((index, value) in input.withIndex()) {
                    if (value) {
                        bigint = bigint.setBit(index)
                    }
                }
            }
            return base94(bigint)
        }

        fun base94(input: BigInteger): String {
            val encoding = BigInteger("94")
            var workingInput = input
            val output = StringBuilder()
            while (workingInput !== BigInteger.ZERO) {
                val rest = workingInput % encoding
                output.append((rest.toInt() + 33).toChar())
                workingInput = (workingInput - rest) / encoding
            }
            return output.toString().reversed().trim()
        }
    }
}

