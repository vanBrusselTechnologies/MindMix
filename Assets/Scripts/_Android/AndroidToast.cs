using UnityEngine;

namespace VBG.UnityAndroid
{
    /// <summary>
    /// A toast is a view containing a quick little message for the user.<br/>
    /// Based on <a href="https://developer.android.com/reference/android/widget/Toast">AndroidStudio - Toast</a>
    /// </summary>
    public class AndroidToast
    {
        public enum DurationType {
            LengthShort = 0,
            LengthLong = 1
        }
        private static AndroidJavaObject UnityContext
        {
            get
            {
                AndroidJavaClass unityPlayer = new("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                return activity.Call<AndroidJavaObject>("getApplicationContext");
            }
        }

        private AndroidJavaObject _context;
        private AndroidJavaObject _toast;

        public AndroidToast(): this(UnityContext){}

        public AndroidToast(AndroidJavaObject context)
        {
            _context = context;
            _toast = new AndroidJavaObject("android.widget.Toast");
        }

        /// <summary>
        /// Make a standard toast that just contains text.
        /// </summary>
        /// <param name="text">The text to show. Can be formatted text.</param>
        /// <param name="duration">How long to display the message.</param>
        /// <returns></returns>
        public AndroidToast MakeText(string text, DurationType duration)
        {
            _toast = _toast.CallStatic<AndroidJavaObject>("makeText", _context, text, (int)duration);
            return this;
        }

        /// <summary>
        /// Show the toast for the specified duration.
        /// </summary>
        public void Show()
        {
            _toast.Call("show");
        }
    }
}