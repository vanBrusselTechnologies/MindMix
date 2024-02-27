package com.vanbrusselgames.mindmix

import com.google.android.play.core.review.ReviewException
import com.google.android.play.core.review.ReviewManagerFactory
import com.google.android.play.core.review.model.ReviewErrorCode

class ReviewManager {
    companion object {
        private lateinit var activity: MainActivity
        private lateinit var reviewManager: com.google.android.play.core.review.ReviewManager
        private var shown = false

        fun start(
            activity: MainActivity
        ) {
            this.activity = activity
            reviewManager = ReviewManagerFactory.create(activity)
        }

        fun requestReview() {
            if (shown) return
            val request = reviewManager.requestReviewFlow()
            request.addOnCompleteListener { task ->
                if (task.isSuccessful) {
                    val reviewInfo = task.result
                    val flow = reviewManager.launchReviewFlow(activity, reviewInfo)
                    flow.addOnCompleteListener { _ -> shown = true }
                } else {
                    @ReviewErrorCode val reviewErrorCode =
                        (task.exception as ReviewException).errorCode
                    Logger.w("Review Request error: $reviewErrorCode")
                }
            }
        }
    }
}
