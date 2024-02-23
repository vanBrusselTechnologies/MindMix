package com.vanbrusselgames.mindmix

import android.util.Log
import com.google.firebase.analytics.FirebaseAnalytics
import com.google.firebase.analytics.ParametersBuilder
import com.google.firebase.analytics.logEvent

@Suppress("unused")
class Logger {
    companion object {
        private lateinit var analytics: FirebaseAnalytics
        fun start(activity: MainActivity) {
            analytics = FirebaseAnalytics.getInstance(activity)
        }

        fun logEvent(eventName: String, block: ParametersBuilder.() -> Unit) {
            analytics.logEvent(eventName, block)
        }

        fun d(msg: String) {
            Log.d("MindMix", msg)
        }

        fun d(msg: String, tr: Throwable) {
            Log.d("MindMix", msg, tr)
        }

        fun e(msg: String) {
            Log.e("MindMix", msg)
        }

        fun e(msg: String, tr: Throwable) {
            Log.e("MindMix", msg, tr)
        }

        fun w(msg: String) {
            Log.w("MindMix", msg)
        }

        fun w(msg: String, tr: Throwable) {
            Log.w("MindMix", msg, tr)
        }
    }
}