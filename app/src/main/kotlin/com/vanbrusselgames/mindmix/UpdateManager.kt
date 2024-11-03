package com.vanbrusselgames.mindmix

import android.app.Activity
import android.content.Context
import androidx.activity.result.ActivityResultLauncher
import androidx.activity.result.IntentSenderRequest
import com.google.android.play.core.appupdate.AppUpdateInfo
import com.google.android.play.core.appupdate.AppUpdateManager
import com.google.android.play.core.appupdate.AppUpdateManagerFactory
import com.google.android.play.core.appupdate.AppUpdateOptions
import com.google.android.play.core.install.InstallStateUpdatedListener
import com.google.android.play.core.install.model.AppUpdateType
import com.google.android.play.core.install.model.InstallStatus
import com.google.android.play.core.install.model.UpdateAvailability
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch

class UpdateManager {
    companion object {
        //private const val DAYS_FOR_FLEXIBLE_UPDATE = 3
        private lateinit var appUpdateManager: AppUpdateManager

        fun start(
            activity: Activity, activityResultLauncher: ActivityResultLauncher<IntentSenderRequest>
        ) {
            if (this::appUpdateManager.isInitialized) return
            appUpdateManager = AppUpdateManagerFactory.create(activity)
            val appUpdateInfoTask = appUpdateManager.appUpdateInfo
            appUpdateInfoTask.addOnSuccessListener { appUpdateInfo ->
                if (appUpdateInfo.installStatus() == InstallStatus.DOWNLOADED) {
                    // If the update is downloaded but not installed,
                    // notify the user to complete the update.
                    popupSnackbarForCompleteUpdate(activity)
                } else if (appUpdateInfo.updateAvailability() == UpdateAvailability.DEVELOPER_TRIGGERED_UPDATE_IN_PROGRESS) {
                    // If an in-app update is already running, resume the update.
                    requestUpdate(
                        activity, appUpdateInfo, activityResultLauncher, AppUpdateType.IMMEDIATE
                    )
                } else if (appUpdateInfo.updateAvailability() == UpdateAvailability.UPDATE_AVAILABLE && appUpdateInfo.updatePriority() >= 4 /* high priority */ && appUpdateInfo.isUpdateTypeAllowed(
                        AppUpdateType.IMMEDIATE
                    )
                ) {
                    requestUpdate(
                        activity, appUpdateInfo, activityResultLauncher, AppUpdateType.IMMEDIATE
                    )
                } else if (appUpdateInfo.updateAvailability() == UpdateAvailability.UPDATE_AVAILABLE && /*(appUpdateInfo.clientVersionStalenessDays() ?: -1) >= DAYS_FOR_FLEXIBLE_UPDATE &&*/
                    appUpdateInfo.isUpdateTypeAllowed(AppUpdateType.FLEXIBLE)
                ) {
                    requestUpdate(
                        activity, appUpdateInfo, activityResultLauncher, AppUpdateType.FLEXIBLE
                    )
                }
            }
        }

        private fun requestUpdate(
            ctx: Context,
            appUpdateInfo: AppUpdateInfo,
            activityResultLauncher: ActivityResultLauncher<IntentSenderRequest>,
            appUpdateType: Int
        ) {
            if (appUpdateType == AppUpdateType.FLEXIBLE) {
                // Create a listener to track request state updates.
                val listener = InstallStateUpdatedListener { state ->
                    // (Optional) Provide a download progress bar.
                    /*if (state.installStatus() == InstallStatus.DOWNLOADING) {
                        val bytesDownloaded = state.bytesDownloaded()
                        val totalBytesToDownload = state.totalBytesToDownload()
                        // Show update progress bar.
                    }*/
                    if (state.installStatus() == InstallStatus.DOWNLOADED) {
                        // If the update is downloaded but not installed,
                        // notify the user to complete the update.
                        popupSnackbarForCompleteUpdate(ctx)
                    }
                    //else if (state.installStatus() == InstallStatus.INSTALLED) {
                    //    appUpdateManager.unregisterListener(listener)
                    //}
                }
                appUpdateManager.registerListener(listener)
            }

            appUpdateManager.startUpdateFlowForResult(
                appUpdateInfo,
                activityResultLauncher,
                AppUpdateOptions.newBuilder(appUpdateType).build()
            )
        }

        private fun popupSnackbarForCompleteUpdate(ctx: Context) {
            CoroutineScope(Dispatchers.Main).launch{
                /*todo:
                    val result = MainActivity.snackbarHostState.showSnackbar(
                        message = ctx.resources.getString(R.string.flexible_update_downloaded),
                        actionLabel = "Restart",
                        duration = SnackbarDuration.Indefinite,
                        withDismissAction = false
                    )
                    when (result) {
                        SnackbarResult.ActionPerformed -> {
                            DataManager.save(false)
                            appUpdateManager.completeUpdate()
                            MainActivity.snackbarHostState.currentSnackbarData?.dismiss()
                        }
                        SnackbarResult.Dismissed -> {}
                    }
                 */
            }
        }
    }
}
