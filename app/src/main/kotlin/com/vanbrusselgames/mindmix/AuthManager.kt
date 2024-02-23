package com.vanbrusselgames.mindmix

import android.app.Activity
import androidx.core.content.ContextCompat.getString
import com.google.android.gms.games.AuthenticationResult
import com.google.android.gms.games.PlayGames
import com.google.android.gms.games.PlayGamesSdk
import com.google.android.gms.tasks.Task
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.auth.PlayGamesAuthProvider
import com.google.firebase.auth.ktx.auth
import com.google.firebase.ktx.Firebase

class AuthManager {
    companion object {
        var isAuthenticated = false
        private lateinit var auth: FirebaseAuth

        fun start(activity: Activity) {
            PlayGamesSdk.initialize(activity)
            auth = Firebase.auth

            // Check if user is signed in (non-null) and update UI accordingly.
            //val currentUser = auth.currentUser
            //updateUI(currentUser)

            //val gso = GoogleSignInOptions.Builder(GoogleSignInOptions.DEFAULT_GAMES_SIGN_IN)
            //    .requestServerAuthCode(getString(activity, R.string.default_web_client_id)).build()

            val gamesSignInClient = PlayGames.getGamesSignInClient(activity)

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
                }/*else {
                    // Disable your integration with Play Games Services or show a
                    // login button to ask  players to sign-in. Clicking it should
                    // call gamesSignInClient.signIn().
                }*/
            }
        }

        private fun firebaseAuthWithPlayGames(activity: Activity, serverAuthCode: String) {
            val auth = Firebase.auth
            val credential = PlayGamesAuthProvider.getCredential(serverAuthCode)
            auth.signInWithCredential(credential).addOnCompleteListener(activity) { task ->
                if (task.isSuccessful) {
                    // Sign in success, update UI with the signed-in user's information
                    //Logger.d("signInWithCredential:success")
                    val user = auth.currentUser
                    //updateUI(user)
                } else {
                    // If sign in fails, display a message to the user.
                    //Logger.w("signInWithCredential:failure", task.exception)
                    /*Toast.makeText(
                        baseContext,
                        "Authentication failed.",
                        Toast.LENGTH_SHORT,
                    ).show()*/
                    //updateUI(null)
                }

                // ...
            }
        }
    }
}
