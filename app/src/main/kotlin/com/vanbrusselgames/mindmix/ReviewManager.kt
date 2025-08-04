package com.vanbrusselgames.mindmix

import android.app.Activity
import android.content.Context
import com.google.android.play.core.review.ReviewException
import com.google.android.play.core.review.ReviewManagerFactory
import com.google.android.play.core.review.model.ReviewErrorCode
import com.vanbrusselgames.mindmix.core.logging.Logger
import dagger.hilt.android.qualifiers.ApplicationContext
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class ReviewManager @Inject constructor(@ApplicationContext ctx: Context) {
    private var reviewManager: com.google.android.play.core.review.ReviewManager =
        ReviewManagerFactory.create(ctx)
    private var shown = false

    fun requestReview(activity: Activity) {
        if (shown) return
        val request = reviewManager.requestReviewFlow()
        request.addOnCompleteListener { task ->
            if (task.isSuccessful) {
                val reviewInfo = task.result
                val flow = reviewManager.launchReviewFlow(activity, reviewInfo)
                flow.addOnCompleteListener { _ -> shown = true }
            } else {
                @ReviewErrorCode val reviewErrorCode = (task.exception as ReviewException).errorCode
                Logger.w("Review Request error: $reviewErrorCode")
            }
        }
    }
}