package com.vanbrusselgames.mindmix

import android.app.Activity
import android.widget.Toast
import androidx.compose.runtime.MutableState
import androidx.core.content.ContextCompat.getString
import com.google.android.gms.games.AuthenticationResult
import com.google.android.gms.games.GamesSignInClient
import com.google.android.gms.games.PlayGames
import com.google.android.gms.games.PlayGamesSdk
import com.google.android.gms.tasks.Task
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.auth.FirebaseUser
import com.google.firebase.auth.PlayGamesAuthProvider
import com.google.firebase.auth.ktx.auth
import com.google.firebase.crashlytics.ktx.crashlytics
import com.google.firebase.ktx.Firebase

class AuthManager {
    companion object {
        var isAuthenticated = false
        private val auth: FirebaseAuth = Firebase.auth
        var currentUser: FirebaseUser? = auth.currentUser
        private lateinit var gamesSignInClient: GamesSignInClient
        private lateinit var activity: MainActivity

        fun start(activity: MainActivity) {
            this.activity = activity
            PlayGamesSdk.initialize(activity)
            gamesSignInClient = PlayGames.getGamesSignInClient(activity)
            gamesSignInClient.isAuthenticated.addOnCompleteListener { isAuthenticatedTask: Task<AuthenticationResult> ->
                val isAuthenticated =
                    isAuthenticatedTask.isSuccessful && isAuthenticatedTask.result.isAuthenticated
                this.isAuthenticated = isAuthenticated

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

        private fun firebaseAuthWithPlayGames(
            activity: Activity, serverAuthCode: String, signedIn: MutableState<Boolean>? = null
        ) {
            val auth = Firebase.auth
            val credential = PlayGamesAuthProvider.getCredential(serverAuthCode)
            auth.signInWithCredential(credential).addOnCompleteListener(activity) { task ->
                if (task.isSuccessful) {
                    Logger.d("signInWithCredential:success")
                    currentUser = auth.currentUser
                    currentUser?.uid?.let { Firebase.crashlytics.setUserId(it) }
                    if (signedIn != null) signedIn.value = true
                } else {
                    // If sign in fails, display a message to the user.
                    Logger.w("signInWithCredential:failure", task.exception!!)
                    Toast.makeText(
                        activity,
                        "Authentication failed.",
                        Toast.LENGTH_SHORT,
                    ).show()
                }
            }
        }

        fun signIn(signedIn: MutableState<Boolean>) {
            gamesSignInClient.signIn()
                .addOnCompleteListener { isAuthenticatedTask: Task<AuthenticationResult> ->
                    val isAuthenticated =
                        isAuthenticatedTask.isSuccessful && isAuthenticatedTask.result.isAuthenticated
                    this.isAuthenticated = isAuthenticated

                    if (isAuthenticated) {
                        gamesSignInClient.requestServerSideAccess(
                            getString(activity, R.string.default_web_client_id), false
                        ).addOnCompleteListener { task ->
                            if (task.isSuccessful) {
                                val serverAuthToken = task.result
                                firebaseAuthWithPlayGames(activity, serverAuthToken, signedIn)
                            }
                        }
                    }
                }
        }
    }
}
