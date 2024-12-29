using UnityEngine;
using System;

namespace TextSpeech
{
    public class TextToSpeech : MonoBehaviour
    {
        private AndroidJavaObject ttsObject;
        private AndroidJavaObject currentActivity;

        void Start()
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            ttsObject = new AndroidJavaObject("android.speech.tts.TextToSpeech", currentActivity, new TextToSpeechListener());
        }

        public void Speak(string text)
        {
            if (ttsObject != null)
            {
                ttsObject.Call<int>("setLanguage", new AndroidJavaClass("java.util.Locale").GetStatic<AndroidJavaObject>("US"));
                ttsObject.Call<int>("speak", text, 0, null, null);
            }
        }

        public void Stop()
        {
            if (ttsObject != null)
            {
                ttsObject.Call("stop");
            }
        }

        void OnDestroy()
        {
            if (ttsObject != null)
            {
                ttsObject.Call("stop");
                ttsObject.Call("shutdown");
            }
        }

        private class TextToSpeechListener : AndroidJavaProxy
        {
            public TextToSpeechListener() : base("android.speech.tts.TextToSpeech$OnInitListener") { }

            public void onInit(int status)
            {
                if (status == 0)
                {
                    Debug.Log("TextToSpeech initialized successfully.");
                }
                else
                {
                    Debug.LogError("Failed to initialize TextToSpeech.");
                }
            }
        }
    }
}