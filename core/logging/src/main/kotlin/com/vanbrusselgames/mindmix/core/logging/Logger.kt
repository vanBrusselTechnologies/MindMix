package com.vanbrusselgames.mindmix.core.logging

import android.util.Log
import com.google.firebase.Firebase
import com.google.firebase.analytics.ParametersBuilder
import com.google.firebase.analytics.analytics
import com.google.firebase.analytics.logEvent
import com.google.firebase.crashlytics.crashlytics
import org.jetbrains.annotations.TestOnly

object Logger {
    private const val TAG = "MindMix"

    fun logEvent(eventName: String, block: ParametersBuilder.() -> Unit) {
        Firebase.analytics.logEvent(eventName, block)
    }

    @TestOnly
    fun d(msg: String) {
        Log.d(TAG, msg)
    }

    fun i(msg: String) {
        Log.i(TAG, msg)
        Firebase.crashlytics.log(msg)
    }

    fun i(msg: String, tr: Throwable) {
        Log.d(TAG, msg, tr)
        Firebase.crashlytics.log(msg)
    }

    fun e(msg: String) {
        Log.e(TAG, msg)
        Firebase.crashlytics.recordException(RuntimeException(msg))
    }

    fun e(msg: String, tr: Throwable) {
        Log.e(TAG, msg, tr)
        Firebase.crashlytics.recordException(RuntimeException(msg, tr))
    }

    fun w(msg: String) {
        Log.w(TAG, msg)
        Firebase.crashlytics.recordException(RuntimeException(msg))
    }

    fun w(msg: String, tr: Throwable) {
        Log.w(TAG, msg, tr)
        Firebase.crashlytics.recordException(RuntimeException(msg, tr))
    }
}