package com.vanbrusselgames.mindmix.core.utils.encode

import java.math.BigInteger

object Encode {
    fun base94(input: BigInteger): String {
        if (input == BigInteger.ZERO) return "!"
        val encoding = BigInteger.valueOf(94)
        var num = input
        val sb = StringBuilder()
        while (num != BigInteger.ZERO) {
            val remainder = num.mod(encoding)
            sb.append((remainder.toInt() + 33).toChar())
            num = num.divide(encoding)
        }
        return sb.reverse().toString()
    }

    /** Collection should only contain values inside range 0-9 */
    fun base94IntCollection(input: Collection<Int>): String {
        if (input.isEmpty()) return base94(BigInteger.ZERO)
        return base94(BigInteger(input.ifEmpty { listOf(0) }.joinToString("")))
    }

    fun base94BooleanCollection(input: Collection<Boolean>): String {
        if (input.isEmpty()) return base94(BigInteger.ZERO)
        val bigint = BigInteger.valueOf(
            input.reversed().fold(0L) { acc, bit -> (acc shl 1) or if (bit) 1 else 0 })
        return base94(bigint)
    }
}