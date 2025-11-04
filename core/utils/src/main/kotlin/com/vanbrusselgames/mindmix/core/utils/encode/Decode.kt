package com.vanbrusselgames.mindmix.core.utils.encode

import java.math.BigInteger

object Decode {
    fun base94(s: String): BigInteger {
        val base = BigInteger.valueOf(94)
        var result = BigInteger.ZERO
        for (char in s) {
            val value = char.code - 33
            if (value !in 0..<94) throw IllegalArgumentException("Invalid character in base94 string: $char")
            result = result * base + value.toBigInteger()
        }
        return result
    }

    fun pad(num: BigInteger, size: Int): CharArray {
        val numStr = num.toString()
        val currentLength = numStr.length
        if (currentLength >= size) return numStr.toCharArray()
        val charArray = CharArray(size)
        val paddingZerosCount = size - currentLength
        for (i in 0 until paddingZerosCount) {
            charArray[i] = '0'
        }
        for (i in 0 until currentLength) {
            charArray[paddingZerosCount + i] = numStr[i]
        }
        return charArray
    }

    fun base94toIntList(s: String, size: Int): List<Int> {
        val paddedString = pad(base94(s), size)
        return List(paddedString.size) { paddedString[it].digitToInt() }
    }

    fun base94toBooleanList(s: String, size: Int): List<Boolean> {
        val bigint = base94(s)
        val bitLength = minOf(bigint.bitLength(), size)
        return List(size) { if (it < bitLength) bigint.testBit(it) else false }
    }
}