package com.vanbrusselgames.mindmix.core.authentication

import android.app.Activity
import android.content.Context
import android.widget.Toast
import androidx.compose.runtime.mutableStateOf
import androidx.core.content.ContextCompat.getString
import com.google.android.gms.games.AuthenticationResult
import com.google.android.gms.games.GamesSignInClient
import com.google.android.gms.games.PlayGames
import com.google.android.gms.games.PlayGamesSdk
import com.google.android.gms.tasks.Task
import com.google.firebase.Firebase
import com.google.firebase.auth.PlayGamesAuthProvider
import com.google.firebase.auth.auth
import com.google.firebase.crashlytics.crashlytics
import com.vanbrusselgames.mindmix.core.logging.Logger
import dagger.hilt.android.qualifiers.ApplicationContext
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class AuthManager @Inject constructor(@param:ApplicationContext private val ctx: Context) {
    private var isAuthenticated = false
    val signedIn = mutableStateOf(false)
    val userId = mutableStateOf("")

    init {
        PlayGamesSdk.initialize(ctx)

        Firebase.auth.currentUser?.uid?.let {
            userId.value = it
            Firebase.crashlytics.setUserId(it)
        }
    }

    // This method shows how to get the client when you have an activity
    private fun getGamesSignInClient(activity: Activity): GamesSignInClient {
        return PlayGames.getGamesSignInClient(activity)
    }

    // Call this from your Activity/ViewModel when it's ready
    fun initialAuthCheck(activity: Activity) {
        CoroutineScope(Dispatchers.IO).launch {
            getGamesSignInClient(activity).isAuthenticated.addOnCompleteListener { isAuthenticatedTask: Task<AuthenticationResult> ->
                val isAuthenticated =
                    isAuthenticatedTask.isSuccessful && isAuthenticatedTask.result.isAuthenticated
                this@AuthManager.isAuthenticated = isAuthenticated

                // Update signedIn state based on both Firebase and Play Games
                signedIn.value = isAuthenticated && Firebase.auth.currentUser != null

                if (isAuthenticated && Firebase.auth.currentUser == null) {
                    // Authenticated with Play Games but not Firebase, try to link
                    Logger.d("Authenticated with Play Games, attempting Firebase link.")
                    requestAndLinkFirebase(activity)
                } else if (isAuthenticated && Firebase.auth.currentUser != null) {
                    Logger.d("Already authenticated with Play Games and Firebase.")
                    Firebase.auth.currentUser?.uid?.let {
                        userId.value = it
                        Firebase.crashlytics.setUserId(it)
                    }
                }
            }
        }
    }

    private fun firebaseAuthWithPlayGames(activity: Activity, serverAuthCode: String) {
        val credential = PlayGamesAuthProvider.getCredential(serverAuthCode)
        Firebase.auth.signInWithCredential(credential).addOnCompleteListener(activity) { task ->
            if (task.isSuccessful) {
                Logger.i("signInWithCredential:success")
                signedIn.value = isAuthenticated && Firebase.auth.currentUser != null
                Firebase.auth.currentUser?.uid?.let {
                    Firebase.crashlytics.setUserId(it)
                }
            } else {
                // If sign in fails, display a message to the user.
                Logger.w("signInWithCredential:failure", task.exception!!)
                //todo: localize error
                Toast.makeText(
                    activity,
                    "Authentication failed.",
                    Toast.LENGTH_SHORT,
                ).show()
            }
        }
    }

    private fun requestAndLinkFirebase(activity: Activity) {
        getGamesSignInClient(activity).requestServerSideAccess(
            getString(ctx, R.string.default_web_client_id), false
        ).addOnCompleteListener { task ->
            if (task.isSuccessful) {
                val serverAuthToken = task.result
                firebaseAuthWithPlayGames(activity, serverAuthToken)
            } else {
                Logger.w("requestServerSideAccess:failure", task.exception)
                Toast.makeText(activity, "Failed to get server auth code.", Toast.LENGTH_SHORT)
                    .show()
                signedIn.value = false // Indicate failure
            }
        }
    }

    fun signIn(activity: Activity) {
        with(getGamesSignInClient(activity)) {
            signIn().addOnCompleteListener { isAuthenticatedTask: Task<AuthenticationResult> ->
                val isAuthenticated =
                    isAuthenticatedTask.isSuccessful && isAuthenticatedTask.result.isAuthenticated
                this@AuthManager.isAuthenticated = isAuthenticated

                signedIn.value = isAuthenticated && Firebase.auth.currentUser != null

                if (isAuthenticated) {
                    requestServerSideAccess(
                        getString(ctx, R.string.default_web_client_id), false
                    ).addOnCompleteListener { task ->
                        if (task.isSuccessful) {
                            val serverAuthToken = task.result
                            firebaseAuthWithPlayGames(activity, serverAuthToken)
                        }
                    }
                }
            }
        }
    }
}