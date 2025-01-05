using System.Collections;
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
    public Image Img_panel_heart;
    public Image Img_panel_heart_border;
    public Image Img_panel_heart_icon;
    public Color32 color_heart_pin;

    [Header("Audio")]
    public AudioSource audioSource_Recording;
    public AudioSource audioSource_Speech;
    private AudioClip speechClip;
    private V_item data_v_cur=null;
    private int index_favourite_found=-1;

    public void On_Load()
    {
        this.panel_vocabulary.SetActive(false);
        this.panel_Recording.SetActive(false);
        this.panel_vocabulary_true.SetActive(false);
        this.panel_vocabulary_false.SetActive(false);
    }

    public void On_Show(V_item v)
    {
        this.panel_vocabulary.SetActive(true);
        this.panel_vocabulary.transform.SetAsLastSibling();
        this.data_v_cur=v;
        this.app.Set_index_menu_cur(5);
        if(this.app.is_sell==false) this.app.ads.On_show_interstitial();
        this.txt_Status.text = "";
        if(this.app.is_sell){
            if (this.audioSource_Speech.isPlaying) this.audioSource_Speech.Stop();
        }else{
            this.app.texttospeech.Stop();
        }
        
        this.s_vocabulary = v.s_key;
        this.txt_Vocabulary.text = v.s_key;
        if(this.app.is_sell)
            this.txt_vocabulary_index.text = "Week " + (v.index_week + 1)+" #"+(v.index_v_in_week+1);
        else
            this.txt_vocabulary_index.text = "Level "+(v.index_l+1)+" - Week " + (v.index_week + 1)+" #"+(v.index_v_in_week+1);
        this.txt_vocabulary_translate.text = v.s_Translate;
        this.panel_vocabulary.SetActive(true);
        this.app.speechtotext.StopRecording();
        this.panel_Recording.SetActive(false);
        if(v.s_file!="") this.speechClip = Resources.Load<AudioClip>("voice/" + v.s_file.Replace(".WAV", ""));
        this.Check_status_favourite();
    }

    private void Check_status_favourite(){
        this.index_favourite_found=this.app.f.Check_key(this.data_v_cur.s_key);
        if(this.index_favourite_found!=-1){
            this.Img_panel_heart_icon.sprite=this.app.sp_broken_heart;
            this.Img_panel_heart.color=this.color_heart_pin;
            this.Img_panel_heart_border.color=this.color_heart_pin;
        }else{
            this.Img_panel_heart_icon.sprite=this.app.sp_heart;
            this.Img_panel_heart.color=this.app.color_menu_active;
            this.Img_panel_heart_border.color=this.app.color_menu_active;
        }
    }

    public void Close()
    {
        this.panel_vocabulary.SetActive(false);
    }

    public void On_start_check_voice()
    {
        this.txt_Status.text = "Listening...";

        if(this.app.is_sell){
            if (this.audioSource_Speech.isPlaying) this.audioSource_Speech.Stop();
        }else{
            this.app.texttospeech.Stop();
        }
        this.app.speechtotext.StartRecording();
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
        this.app.speechtotext.StopRecording();
        if (Random.Range(0, 3) == 1)
            this.show_vocabulary_result(false);
        else
            this.show_vocabulary_result(true);
    }

    public void On_stop_check_voice()
    {
        StopAllCoroutines();
        this.app.speechtotext.StopRecording();
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
            this.app.texttospeech.Speak(this.s_vocabulary);
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
            this.app.carrot.play_vibrate();
            this.app.play_sound(2);
            this.panel_vocabulary_false.SetActive(true);
        }
    }

    public void close_vocabulary_result()
    {
        this.txt_Status.text="";
        this.panel_vocabulary_true.SetActive(false);
        this.panel_vocabulary_false.SetActive(false);
        this.panel_Recording.SetActive(false);
    }

    public void On_Back()
    {
        this.app.play_sound();
        this.panel_vocabulary.SetActive(false);
        this.app.list_vocabulary.Show();
    }

    public void On_add_favourite()
    {
        this.app.play_sound();
        if(index_favourite_found==-1){
            this.app.f.Add(this.data_v_cur);
            Check_status_favourite();
        }else{
            this.app.f.Delete(this.index_favourite_found,Check_status_favourite);
        }
    }

    public string Get_vocabulary()
    {
        return this.s_vocabulary;
    }

    public void OnFinalResult(string result)
    {
        if (result.Trim().ToLower() == this.app.v.s_vocabulary.ToLower())
            this.show_vocabulary_result(true);
        else
            this.show_vocabulary_result(false);
    }

    public void OnEndOfSpeech(string serror)
    {
        this.txt_Status.text = "";
        this.panel_Recording.SetActive(false);
    }
}
