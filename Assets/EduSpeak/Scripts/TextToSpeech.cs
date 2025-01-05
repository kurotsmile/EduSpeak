using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
public class TextToSpeech : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;
    public AudioSource audioSource;
    public bool is_status_enable=false;

    public void On_Load()
    {

    }

    public void Speak(string text)
    {
        StartCoroutine(GetAudioFromText(text));
    }

    public void Stop()
    {
        this.audioSource.Stop();
    }

    void OnDestroy()
    {

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
}