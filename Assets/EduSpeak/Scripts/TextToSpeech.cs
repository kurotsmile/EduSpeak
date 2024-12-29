using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
public class TextToSpeech : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;
    public AudioSource audioSource;

    private AndroidJavaObject ttsObject;
    private AndroidJavaObject currentActivity;
    public bool is_status_enable=false;

    public void On_Load()
    {
        try{
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            ttsObject = new AndroidJavaObject("android.speech.tts.TextToSpeech", currentActivity, new TextToSpeechListener());
            this.is_status_enable=true;
        }catch{
            this.is_status_enable=false;
        }
    }

    public void Speak(string text)
    {
        if(this.is_status_enable){
            if (ttsObject != null)
            {
                ttsObject.Call<int>("setLanguage", new AndroidJavaClass("java.util.Locale").GetStatic<AndroidJavaObject>("US"));
                ttsObject.Call<int>("speak", text, 0, null, null);
            }
        }else{
            StartCoroutine(GetAudioFromText(text));
        }
    }

    public void Stop()
    {
        if(this.is_status_enable) ttsObject?.Call("stop");
        else this.audioSource.Stop();
    }

    void OnDestroy()
    {
        if(this.is_status_enable){
            if (ttsObject != null)
            {
                ttsObject.Call("stop");
                ttsObject.Call("shutdown");
            }
        }
    }

    IEnumerator GetAudioFromText(string text, string language="en")
    {
        string url = $"https://translate.google.com/translate_tts?ie=UTF-8&q={UnityWebRequest.EscapeURL(text)}&tl={language}&client=tw-ob";
        using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
        www.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error: {www.error}");
        }
        else
        {
            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            if (audioSource != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
    }

    public class TextToSpeechListener : AndroidJavaProxy
    {
        public TextToSpeechListener() : base("android.speech.tts.TextToSpeech$OnInitListener") { }

        public void onInit(int status)
        {
            if (status == 0)
            {
                GameObject.Find("App").GetComponent<TextToSpeech>().is_status_enable=true;
            }
            else
            {
                GameObject.Find("App").GetComponent<TextToSpeech>().is_status_enable=false;
            }
        }
    }
}