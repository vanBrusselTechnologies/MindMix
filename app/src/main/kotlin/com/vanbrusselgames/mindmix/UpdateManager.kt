package com.vanbrusselgames.mindmix

import android.content.Context
import androidx.activity.result.ActivityResultLauncher
import androidx.activity.result.IntentSenderRequest
import androidx.compose.material3.SnackbarDuration
import androidx.compose.material3.SnackbarHostState
import androidx.compose.material3.SnackbarResult
import com.google.android.play.core.appupdate.AppUpdateInfo
import com.google.android.play.core.appupdate.AppUpdateManager
import com.google.android.play.core.appupdate.AppUpdateManagerFactory
import com.google.android.play.core.appupdate.AppUpdateOptions
import com.google.android.play.core.install.InstallStateUpdatedListener
import com.google.android.play.core.install.model.AppUpdateType
import com.google.android.play.core.install.model.InstallStatus
import com.google.android.play.core.install.model.UpdateAvailability
import dagger.hilt.android.qualifiers.ApplicationContext
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class UpdateManager @Inject constructor(@param:ApplicationContext private val ctx: Context) {
    //private const val DAYS_FOR_FLEXIBLE_UPDATE = 3
    private val appUpdateManager: AppUpdateManager = AppUpdateManagerFactory.create(ctx)

    fun checkForUpdates(
        activityResultLauncher: ActivityResultLauncher<IntentSenderRequest>,
        snackbarHostState: SnackbarHostState,
        saveData: () -> Unit
    ) {
        val appUpdateInfoTask = appUpdateManager.appUpdateInfo
        appUpdateInfoTask.addOnSuccessListener { appUpdateInfo ->
            if (appUpdateInfo.installStatus() == InstallStatus.DOWNLOADED) {
                // If the update is downloaded but not installed,
                // notify the user to complete the update.
                popupSnackbarForCompleteUpdate(snackbarHostState, saveData)
                return@addOnSuccessListener
            }
            when (appUpdateInfo.updateAvailability()) {
                UpdateAvailability.UNKNOWN, UpdateAvailability.UPDATE_NOT_AVAILABLE -> return@addOnSuccessListener

                UpdateAvailability.DEVELOPER_TRIGGERED_UPDATE_IN_PROGRESS -> {
                    // If an in-app update is already running, resume the update.
                    requestUpdate(
                        appUpdateInfo,
                        activityResultLauncher,
                        AppUpdateType.IMMEDIATE,
                        snackbarHostState,
                        saveData
                    )
                }

                UpdateAvailability.UPDATE_AVAILABLE -> {
                    val appUpdateType =
                        if (appUpdateInfo.updatePriority() >= 4 /* high priority */ && appUpdateInfo.isUpdateTypeAllowed(
                                AppUpdateType.IMMEDIATE
                            )
                        ) AppUpdateType.IMMEDIATE else AppUpdateType.FLEXIBLE
                    requestUpdate(
                        appUpdateInfo,
                        activityResultLauncher,
                        appUpdateType,
                        snackbarHostState,
                        saveData
                    )
                }
            }
        }
    }

    private fun requestUpdate(
        appUpdateInfo: AppUpdateInfo,
        activityResultLauncher: ActivityResultLauncher<IntentSenderRequest>,
        appUpdateType: Int,
        snackbarHostState: SnackbarHostState,
        saveData: () -> Unit
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
                    popupSnackbarForCompleteUpdate(snackbarHostState, saveData)
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

    private fun popupSnackbarForCompleteUpdate(
        snackbarHostState: SnackbarHostState, saveData: () -> Unit
    ) {
        CoroutineScope(Dispatchers.Main).launch {
            val result = snackbarHostState.showSnackbar(
                message = ctx.resources.getString(R.string.flexible_update_downloaded),
                actionLabel = "Restart",
                duration = SnackbarDuration.Indefinite,
                withDismissAction = false
            )
            when (result) {
                SnackbarResult.ActionPerformed -> {
                    saveData()
                    appUpdateManager.completeUpdate()
                    snackbarHostState.currentSnackbarData?.dismiss()
                }

                SnackbarResult.Dismissed -> {}
            }
        }
    }
}