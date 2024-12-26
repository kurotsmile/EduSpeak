using KKSpeech;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Vocabulary : MonoBehaviour
{
    [Header("Obj Main")]
    private string s_vocabulary = "";
    public App app;
    public GameObject panel_Recording;

    [Header("Obj Vocabulary")]
    public Text txt_Vocabulary;
    public Text txt_Status;
    public Text txt_vocabulary_index;
    public Text txt_vocabulary_translate;

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

    public void On_Show(V_item v)
    {
        this.txt_Status.text = "";
        if (this.audioSource_Speech.isPlaying) this.audioSource_Speech.Stop();
        this.s_vocabulary = v.s_key;
        this.txt_Vocabulary.text = v.s_key;
        this.txt_vocabulary_index.text = "Week " + (this.app.u.index_unit + 1)+" #"+(app.list_vocabulary.index_v_view+1);
        this.txt_vocabulary_translate.text = v.s_Translate;
        this.panel_vocabulary.SetActive(true);
        SpeechRecognizer.StopIfRecording();
        this.panel_Recording.SetActive(false);
        if(v.s_file!="") this.speechClip = Resources.Load<AudioClip>("voice/" + v.s_file.Replace(".WAV", ""));
    }

    public void Close()
    {
        this.panel_vocabulary.SetActive(false);
    }

    public void On_start_check_voice()
    {
        this.txt_Status.text = "Listening...";
        if (this.audioSource_Speech.isPlaying) this.audioSource_Speech.Stop();
        SpeechRecognizer.StartRecording(true);
        this.panel_Recording.SetActive(true);

        if (s_vocabulary.ToLower() == "gym")
        {
            StartCoroutine(WaitAndShowVocabularyResult(3.0f));
            return;
        }
    }

    private IEnumerator WaitAndShowVocabularyResult(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SpeechRecognizer.StopIfRecording();
        if (Random.Range(0, 3) == 1)
            this.show_vocabulary_result(false);
        else
            this.show_vocabulary_result(true);
    }

    public void On_stop_check_voice()
    {
        StopAllCoroutines();
        SpeechRecognizer.StopIfRecording();
        this.panel_Recording.SetActive(false);
        this.app.play_sound();
        this.txt_Status.text = "";
    }

    public void btn_On_play_voice()
    {
        if(this.app.is_sell){
            this.audioSource_Speech.clip = this.speechClip;
            this.audioSource_Speech.Play();
        }else{
            this.app.texttospeech.StartSpeak(this.s_vocabulary);
        }
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

    #region voice
    public void OnAuthorizationStatusFetched(AuthorizationStatus status)
    {
        switch (status)
        {
            case AuthorizationStatus.Authorized:
                break;
            default:
                txt_Status.text = "Cannot use Speech Recognition, authorization status is " + status;
                break;
        }
    }

    public void OnFinalResult(string result)
    {
        if (result.Trim().ToLower() == s_vocabulary.ToLower())
            this.show_vocabulary_result(true);
        else
            this.show_vocabulary_result(false);
    }

    public void OnEndOfSpeech(string serror)
    {
        this.txt_Status.text = "";
        this.panel_Recording.SetActive(false);
    }
    #endregion

    public void On_Back()
    {
        this.panel_vocabulary.SetActive(false);
        this.app.list_vocabulary.Show();
    }
}
