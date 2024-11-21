package com.vanbrusselgames.mindmix.core.authentication

import android.app.Activity
import android.widget.Toast
import androidx.compose.runtime.mutableStateOf
import androidx.core.content.ContextCompat.getString
import com.google.android.gms.games.AuthenticationResult
import com.google.android.gms.games.GamesSignInClient
import com.google.android.gms.games.PlayGames
import com.google.android.gms.games.PlayGamesSdk
import com.google.android.gms.tasks.Task
import com.google.firebase.Firebase
import com.google.firebase.auth.FirebaseUser
import com.google.firebase.auth.PlayGamesAuthProvider
import com.google.firebase.auth.auth
import com.google.firebase.crashlytics.crashlytics
import com.vanbrusselgames.mindmix.core.logging.Logger
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch

class AuthManager(private val activity: Activity) {
    private var currentUser: FirebaseUser? = Firebase.auth.currentUser
    private val gamesSignInClient: GamesSignInClient
    private var isAuthenticated = false
    val signedIn = mutableStateOf(false)
    val userId = mutableStateOf("")

    init {
        PlayGamesSdk.initialize(activity)
        gamesSignInClient = PlayGames.getGamesSignInClient(activity)
        CoroutineScope(Dispatchers.IO).launch {
            gamesSignInClient.isAuthenticated.addOnCompleteListener { isAuthenticatedTask: Task<AuthenticationResult> ->
                val isAuthenticated =
                    isAuthenticatedTask.isSuccessful && isAuthenticatedTask.result.isAuthenticated
                this@AuthManager.isAuthenticated = isAuthenticated

                signedIn.value = isAuthenticated && currentUser != null

                currentUser?.uid?.let {
                    userId.value = it
                    Firebase.crashlytics.setUserId(it)
                }

                if (isAuthenticated) {
                    gamesSignInClient.requestServerSideAccess(
                        getString(activity, R.string.default_web_client_id), false
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

    private fun firebaseAuthWithPlayGames(activity: Activity, serverAuthCode: String) {
        val auth = Firebase.auth
        val credential = PlayGamesAuthProvider.getCredential(serverAuthCode)
        auth.signInWithCredential(credential).addOnCompleteListener(activity) { task ->
            if (task.isSuccessful) {
                Logger.i("signInWithCredential:success")
                currentUser = auth.currentUser
                signedIn.value = isAuthenticated && currentUser != null
                currentUser?.uid?.let {
                    userId.value = it
                    Firebase.crashlytics.setUserId(it)
                }
            } else {
                // If sign in fails, display a message to the user.
                Logger.w(
                    "signInWithCredential:failure", task.exception!!
                )
                //todo: localize error
                Toast.makeText(
                    activity,
                    "Authentication failed.",
                    Toast.LENGTH_SHORT,
                ).show()
            }
        }
    }

    fun signIn() {
        gamesSignInClient.signIn()
            .addOnCompleteListener { isAuthenticatedTask: Task<AuthenticationResult> ->
                val isAuthenticated =
                    isAuthenticatedTask.isSuccessful && isAuthenticatedTask.result.isAuthenticated
                this.isAuthenticated = isAuthenticated

                signedIn.value = isAuthenticated && currentUser != null

                if (isAuthenticated) {
                    gamesSignInClient.requestServerSideAccess(
                        getString(activity, R.string.default_web_client_id), false
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