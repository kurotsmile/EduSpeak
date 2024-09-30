using Carrot;
using System.Collections;
using System.Collections.Generic;
using TextSpeech;
using UnityEngine;
using UnityEngine.UI;

public class App : MonoBehaviour
{
    [Header("Object Main")]
    public GameObject lesson_item_prefab;
    public GameObject Vocabulary_item_prefab;
    public TextToSpeech texttospeech;
    public SpeechToText speechToText;

    [Header("UI")]
    public GameObject panel_home;
    public GameObject panel_main;
    public GameObject panel_view;
    public GameObject panel_vocabulary;
    public GameObject panel_setting;
    public Transform area_all_lesson;
    public Transform area_all_vocabulary;

    [Header("vocabulary")]
    public Text txt_lesson_title;
    public Text txt_vocabulary_title;
    public Text txt_vocabulary_translate;
    public GameObject panel_vocabulary_true;
    public GameObject panel_vocabulary_false;

    [Header("Setting")]
    public Image[] checkBox_setting_img;

    [Header("Sound")]
    public AudioSource[] sound;
    public AudioSource sound_voice;

    [Header("Asset")]
    public Sprite sp_checkbox_true;
    public Sprite sp_checkbox_false;

    private string s_vocabulary;
    private bool[] list_setting_val=null;

    void Start()
    {
        this.panel_home.SetActive(true);
        this.panel_main.SetActive(false);
        this.panel_view.SetActive(false);
        this.panel_vocabulary.SetActive(false);
        this.panel_vocabulary_true.SetActive(false);
        this.panel_vocabulary_false.SetActive(false);
        this.panel_setting.SetActive(false);

        this.list_setting_val = new bool[this.checkBox_setting_img.Length];

        for(int i = 0; i < this.checkBox_setting_img.Length; i++)
        {
            if (PlayerPrefs.GetInt("setting_item_"+i, 0) == 1)
            {
                this.list_setting_val[i] = true;
                this.checkBox_setting_img[i].sprite = this.sp_checkbox_true;
            }
            else
            {
                this.list_setting_val[i] = false;
                this.checkBox_setting_img[i].sprite = this.sp_checkbox_false;
            }
                
        }

        TextAsset jsonFile = Resources.Load<TextAsset>("data");
        this.Clear_all_item(area_all_lesson);
        if (jsonFile != null)
        {
            IList list_data = (IList)Json.Deserialize(jsonFile.text);
            for (int i = 0; i < list_data.Count; i++)
            {
                IDictionary dataLesson = (IDictionary) list_data[i];
                GameObject objLesson = Instantiate(this.lesson_item_prefab);
                objLesson.transform.SetParent(this.area_all_lesson);
                objLesson.transform.localScale = new Vector3(1, 1, 1);

                objLesson.GetComponent<Menu_Item>().txt.text = dataLesson["name"].ToString();
                objLesson.GetComponent<Menu_Item>().act = () =>
                {
                    this.On_Show_view(dataLesson);
                };
            }
        }

        SpeechToText.Instance.Setting("en-US");
        SpeechToText.Instance.onResultCallback = this.On_Check_voice;
        this.speechToText.onResultCallback = this.On_Check_voice;
    }

    public void btn_on_start()
    {
        this.play_sound();
        this.panel_main.SetActive(true);
    }

    public void On_Show_view(IDictionary data)
    {
        this.txt_lesson_title.text = data["name"].ToString();
        this.Clear_all_item(this.area_all_vocabulary);
        IList list_txt = (IList) data["text"];
        IList list_vi = null;
        if(data["vi"]!=null) list_vi = (IList)data["vi"];
        for (int i = 0; i < list_txt.Count; i++)
        {
            var s_Vocabulary = list_txt[i].ToString();
            var s_Translate ="";

            if (this.list_setting_val[2])
            {
                if (list_vi != null)
                {
                    if (list_vi[i] != null) s_Translate = list_vi[i].ToString();
                }
            }
            
            GameObject objVocabulary = Instantiate(this.Vocabulary_item_prefab);
            objVocabulary.transform.SetParent(this.area_all_vocabulary);
            objVocabulary.transform.localScale = new Vector3(1, 1, 1);

            objVocabulary.GetComponent<Menu_Item>().txt.text = (i+1).ToString();
            objVocabulary.GetComponent<Menu_Item>().act = () =>
            {
                this.play_sound();
                this.panel_vocabulary.SetActive(true);
                this.txt_vocabulary_title.text = s_Vocabulary;
                this.s_vocabulary = s_Vocabulary;
                this.txt_vocabulary_translate.text = s_Translate;
            };
        }
        this.play_sound();
        this.panel_view.SetActive(true);
    }

    public void Clear_all_item(Transform tr)
    {
        foreach(Transform t in tr)
        {
            Destroy(t.gameObject);
        }
    }

    public void Back_home()
    {
        this.panel_main.SetActive(false);
        this.panel_view.SetActive(false);
        this.play_sound();
    }

    public void play_sound(int index = 0)
    {
        if (list_setting_val[0]) this.sound[index].Play();
    }

    public void Back_List_Lesson()
    {
        this.panel_view.SetActive(true);
        this.panel_vocabulary.SetActive(false);
        this.play_sound();
    }

    public void show_setting()
    {
        this.play_sound();
        this.panel_setting.SetActive(true);
    }

    public void close_setting()
    {
        this.play_sound();
        this.panel_setting.SetActive(false);
    }

    public void btn_On_voice_recognition()
    {
        this.speechToText.onResultCallback = this.On_Check_voice;
        SpeechToText.Instance.StartRecording("Start speaking!");
    }

    public void btn_On_play_audio()
    {
        AudioClip clip = Resources.Load<AudioClip>("voice/"+s_vocabulary);
        if (clip != null)
        {
            this.sound_voice.clip=clip;
            this.sound_voice.Play();
        }
        else
        {
            this.texttospeech.StartSpeak(this.s_vocabulary);
        }
    }

    public void show_vocabulary_result(bool is_true)
    {
        if (is_true)
        {
            this.panel_vocabulary_true.SetActive(true);
        }
        else
        {
            this.play_Vibrate();
            this.panel_vocabulary_false.SetActive(true);
        }
    }

    public void close_vocabulary_result()
    {
        this.panel_vocabulary_true.SetActive(false);
        this.panel_vocabulary_false.SetActive(false);
    }

    public void On_click_item_setting(int index)
    {
        if (this.list_setting_val[index]) {
            this.checkBox_setting_img[index].sprite = this.sp_checkbox_false;
            this.list_setting_val[index] = false;
            PlayerPrefs.SetInt("setting_item_"+index,0);
        }
        else
        {
            this.checkBox_setting_img[index].sprite = this.sp_checkbox_true;
            this.list_setting_val[index] = true;
            if (index == 1) this.play_Vibrate();
            PlayerPrefs.SetInt("setting_item_" + index, 1);
        }
        this.play_sound();
    }

    public void play_Vibrate()
    {
        if (this.list_setting_val[1]) Handheld.Vibrate();
    }

    public void On_Check_voice(string s_data)
    {
        this.txt_vocabulary_translate.text = s_data;
        if (s_data.Trim().ToLower() == this.s_vocabulary.Trim().ToLower())
            this.show_vocabulary_result(true);
        else
            this.show_vocabulary_result(false);
    }
}
