package com.vanbrusselgames.mindmix

import android.app.Activity
import androidx.activity.result.ActivityResultLauncher
import androidx.activity.result.IntentSenderRequest
import androidx.compose.material3.SnackbarDuration
import androidx.compose.material3.SnackbarResult
import com.google.android.play.core.appupdate.AppUpdateInfo
import com.google.android.play.core.appupdate.AppUpdateManager
import com.google.android.play.core.appupdate.AppUpdateManagerFactory
import com.google.android.play.core.appupdate.AppUpdateOptions
import com.google.android.play.core.install.model.AppUpdateType
import com.google.android.play.core.install.model.InstallStatus
import com.google.android.play.core.install.model.UpdateAvailability
import kotlinx.coroutines.launch

class UpdateManager {
    companion object {
        private const val DAYS_FOR_FLEXIBLE_UPDATE = 0
        private lateinit var appUpdateManager: AppUpdateManager

        fun start(
            activity: Activity, activityResultLauncher: ActivityResultLauncher<IntentSenderRequest>
        ) {
            if(this::appUpdateManager.isInitialized) return
            appUpdateManager = AppUpdateManagerFactory.create(activity)
            val appUpdateInfoTask = appUpdateManager.appUpdateInfo
            appUpdateInfoTask.addOnSuccessListener { appUpdateInfo ->
                if (appUpdateInfo.installStatus() == InstallStatus.DOWNLOADED) {
                    // If the update is downloaded but not installed,
                    // notify the user to complete the update.
                    popupSnackbarForCompleteUpdate()
                } else if (appUpdateInfo.updateAvailability() == UpdateAvailability.DEVELOPER_TRIGGERED_UPDATE_IN_PROGRESS) {
                    // If an in-app update is already running, resume the update.
                    requestUpdate(appUpdateInfo, activityResultLauncher, AppUpdateType.IMMEDIATE)
                } else if (appUpdateInfo.updateAvailability() == UpdateAvailability.UPDATE_AVAILABLE && appUpdateInfo.updatePriority() >= 4 /* high priority */ && appUpdateInfo.isUpdateTypeAllowed(
                        AppUpdateType.IMMEDIATE
                    )
                ) {
                    requestUpdate(appUpdateInfo, activityResultLauncher, AppUpdateType.IMMEDIATE)
                } else if (appUpdateInfo.updateAvailability() == UpdateAvailability.UPDATE_AVAILABLE && (appUpdateInfo.clientVersionStalenessDays()
                        ?: -1) >= DAYS_FOR_FLEXIBLE_UPDATE && appUpdateInfo.isUpdateTypeAllowed(
                        AppUpdateType.FLEXIBLE
                    )
                ) {
                    requestUpdate(appUpdateInfo, activityResultLauncher, AppUpdateType.FLEXIBLE)
                }
            }
        }

        private fun requestUpdate(
            appUpdateInfo: AppUpdateInfo,
            activityResultLauncher: ActivityResultLauncher<IntentSenderRequest>,
            appUpdateType: Int
        ) {
            // Create a listener to track request state updates.
            /*val listener = InstallStateUpdatedListener { state ->
                // (Optional) Provide a download progress bar.
                /*if (state.installStatus() == InstallStatus.DOWNLOADING) {
                    val bytesDownloaded = state.bytesDownloaded()
                    val totalBytesToDownload = state.totalBytesToDownload()
                    // Show update progress bar.
                }*/
                // Log state or install the update.
                if(state.installStatus() == InstallStatus.INSTALLED) {
                    appUpdateManager.unregisterListener(this)
                }
            }
            appUpdateManager.registerListener(listener)*/
            Logger.d("Request update")

            appUpdateManager.startUpdateFlowForResult(
                appUpdateInfo,
                activityResultLauncher,
                AppUpdateOptions.newBuilder(appUpdateType).build()
            )
        }

        private fun popupSnackbarForCompleteUpdate() {
            MainActivity.scope.launch {
                val result = MainActivity.snackbarHostState.showSnackbar(
                    message = "An update has just been downloaded.",
                    actionLabel = "Restart",
                    duration = SnackbarDuration.Indefinite,
                    withDismissAction = false
                )
                when (result) {
                    SnackbarResult.ActionPerformed -> {
                        appUpdateManager.completeUpdate()
                        MainActivity.snackbarHostState.currentSnackbarData?.dismiss()
                    }

                    SnackbarResult.Dismissed -> {}
                }
            }
        }
    }
}
