package com.vanbrusselgames.mindmix.core.utils.encode

import java.math.BigInteger

object Decode {
    fun base94(s: String): BigInteger {
        val encoding = BigInteger("94")
        var output = BigInteger.ZERO
        val outputLength = s.length
        for (i in 0 until outputLength) {
            val x = (s[outputLength - i - 1].code - 33).toBigInteger()
            output += x * encoding.pow(i)
        }
        return output
    }

    fun pad(num: BigInteger, size: Int): String {
        val str = StringBuilder(num.toString().reversed())
        while (str.length < size) str.append("0")
        return str.toString().reversed()
    }

    fun base94toIntList(s: String, size: Int): List<Int> {
        return pad(base94(s), size).toCharArray().map { it.toString().toInt() }
    }

    fun base94toBooleanList(s: String, size: Int): List<Boolean> {
        val bigint = base94(s)
        val bitLength = minOf(bigint.bitLength(), size)
        val booleanArray = BooleanArray(size)

        for (bitIndex in 0 until bitLength) {
            booleanArray[bitIndex] = bigint.testBit(bitIndex)
        }

        return booleanArray.asList()
    }
}