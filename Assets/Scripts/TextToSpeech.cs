using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class TextToSpeech : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;
    public AudioSource audioSource;
    public bool is_status_enable = false;
    public Image ImgSpeechAudio;

    private byte[] dataMp3;
    public void On_Load()
    {

    }

    public void Speak(string text,string name_file)
    {
        StartCoroutine(GetAudioFromText(text,"en",name_file));
    }

    public void Stop()
    {
        this.audioSource.Stop();
    }


    IEnumerator GetAudioFromText(string text, string language = "en",string nameFilemp3="")
    {
        string url = $"https://translate.google.com/translate_tts?ie=UTF-8&q={UnityWebRequest.EscapeURL(text)}&tl={language}&client=tw-ob";
        using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
        www.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
        this.ImgSpeechAudio.color = Color.blue;
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
                this.ImgSpeechAudio.color =Color.white;
                if (nameFilemp3 != "") app.carrot.get_tool().save_file(nameFilemp3, www.downloadHandler.data);
            }
        }
    }

    public void Play(string s_nameFile)
    {
        StartCoroutine(LoadAndPlayAudio(s_nameFile));
    }
    
    IEnumerator LoadAndPlayAudio(string nameFileMp3)
    {
        string name_file_audio;
        if (Application.isEditor)
            name_file_audio = Application.dataPath + "/" + nameFileMp3;
        else
            name_file_audio = Application.persistentDataPath + "/" + nameFileMp3;
        string url = "file://" + name_file_audio;

        using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error loading audio: " + www.error);
        }
        else
        {
            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            if (audioSource != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
                this.ImgSpeechAudio.color =Color.white;
            }
        }
    }
}