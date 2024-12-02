using KKSpeech;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vocabulary : MonoBehaviour
{
    [Header("Obj Main")]
    public string s_vocabulary = "";
    public App app;
    public GameObject panel_Recording;

    [Header("Obj Vocabulary")]
    public Text txt_Vocabulary;
    public Text txt_Status;
    public GameObject panel_vocabulary;
    public GameObject panel_vocabulary_true;
    public GameObject panel_vocabulary_false;

    [Header("Audio")]
    public AudioSource audioSource_Recording;
    public AudioSource audioSource_Speech;
    private AudioClip speechClip;


    public void On_Load()
    {
        this.panel_vocabulary.SetActive(false);
        this.panel_Recording.SetActive(false);
        this.panel_vocabulary_true.SetActive(false);
        this.panel_vocabulary_false.SetActive(false);
    }

    public void On_Show(string s_Vocabulary,string s_file_audio)
    {
        this.s_vocabulary= s_Vocabulary;
        this.txt_Vocabulary.text = s_Vocabulary;
        this.panel_vocabulary.SetActive(true);
        SpeechRecognizer.StopIfRecording();
        this.panel_Recording.SetActive(false);
        this.speechClip = Resources.Load<AudioClip>("voice/" + s_file_audio.Replace(".WAV",""));
    }

    public void Close()
    {
        this.panel_vocabulary.SetActive(false);
    }

    public void On_start_check_voice()
    {
        this.panel_Recording.SetActive(true);
    }

    public void On_stop_check_voice()
    {
        SpeechRecognizer.StopIfRecording();
        this.panel_Recording.SetActive(false);
        this.app.play_sound();
        this.txt_Status.text = "";
    }

    public void btn_On_play_voice()
    {
        this.audioSource_Speech.clip = this.speechClip;
        this.audioSource_Speech.Play();
    }

    public void show_vocabulary_result(bool is_true)
    {
        if (is_true)
        {
            this.panel_vocabulary_true.SetActive(true);
            this.app.play_sound(1);
        }
        else
        {
            this.app.play_Vibrate();
            this.app.play_sound(2);
            this.panel_vocabulary_false.SetActive(true);
        }
    }

    public void close_vocabulary_result()
    {
        this.panel_vocabulary_true.SetActive(false);
        this.panel_vocabulary_false.SetActive(false);
        this.panel_Recording.SetActive(false);
    }
}
