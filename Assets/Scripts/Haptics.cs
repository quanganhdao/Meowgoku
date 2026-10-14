using UnityEngine;

public static class Haptics
{
#if UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaObject s_vibrator;
#endif

    public static void Tap(long ms = 20)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (s_vibrator == null)
        {
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            s_vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");
        }

        if (s_vibrator == null)
        {
            Handheld.Vibrate();
            return;
        }

        s_vibrator.Call("vibrate", ms);
#endif
    }
}
