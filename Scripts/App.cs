using Carrot;
using KKSpeech;
using System.Collections;
using TextSpeech;
using UnityEngine;
using UnityEngine.UI;

public class App : MonoBehaviour
{
    [Header("Object Main")]
    public GameObject lesson_item_prefab;
    public GameObject Vocabulary_item_prefab;
    public TextToSpeech texttospeech;
    public Vocabulary v;

    [Header("UI")]
    public Color32 color_a;
    public GameObject panel_home;
    public GameObject panel_main;
    public GameObject panel_view;
    public GameObject panel_setting;
    public Transform area_all_lesson;
    public Transform area_all_vocabulary;
    public Text txt_total_vocabulary;
    public Text txt_total_vocabulary_index;
    public Text txt_total_lesson;
    public Text txt_total_voice;

    [Header("vocabulary")]
    public Text txt_lesson_title;
    public Text txt_vocabulary_translate;

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
        this.v.On_Load();
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
        this.On_check_val_setting();

        TextAsset jsonFile = Resources.Load<TextAsset>("data");
        this.Clear_all_item(area_all_lesson);
        if (jsonFile != null)
        {
            int count_vocabulary = 0;
            IList list_data = (IList)Json.Deserialize(jsonFile.text);
            for (int i = 0; i < list_data.Count; i++)
            {
                IDictionary dataLesson = (IDictionary) list_data[i];
                IList list_txt = (IList)dataLesson["text"];
                GameObject objLesson = Instantiate(this.lesson_item_prefab);
                objLesson.transform.SetParent(this.area_all_lesson);
                objLesson.transform.localScale = new Vector3(1, 1, 1);
                if (i % 2 == 0)
                    objLesson.GetComponent<Image>().color = this.color_a;
                else
                    objLesson.GetComponent<Image>().color = Color.white;

                objLesson.GetComponent<Menu_Item>().txt.text = dataLesson["name"].ToString();
                objLesson.GetComponent<Menu_Item>().act = () =>
                {
                    this.On_Show_view(dataLesson);
                };
                count_vocabulary += list_txt.Count;
            }
            this.txt_total_lesson.text = list_data.Count + "\nLesson";
            this.txt_total_vocabulary.text = count_vocabulary + "\nVocabulary";
            this.txt_total_voice.text = count_vocabulary + "\nReading test";
        }
        SpeechRecognizer.SetDetectionLanguage("en-US");
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
        IList list_txt = (IList)data["text"];
        IList list_file = (IList)data["label"];
        IList list_vi = null;
        if (data["vi"] != null) list_vi = (IList)data["vi"];
        for (int i = 0; i < list_txt.Count; i++)
        {
            var s_Vocabulary = list_txt[i].ToString();
            var s_Translate = "";
            var index = i;


            if (list_vi != null)
            {
                if (list_vi[i] != null) s_Translate = list_vi[i].ToString();
            }


            GameObject objVocabulary = Instantiate(this.Vocabulary_item_prefab);
            objVocabulary.transform.SetParent(this.area_all_vocabulary);
            objVocabulary.transform.localScale = new Vector3(1, 1, 1);

            objVocabulary.GetComponent<Menu_Item>().txt.text = (i + 1).ToString();
            objVocabulary.GetComponent<Menu_Item>().act = () =>
            {
                this.play_sound();
                this.v.On_Show(s_Vocabulary,list_file[index].ToString());
                this.s_vocabulary = s_Vocabulary;
                this.txt_vocabulary_translate.text = s_Translate;
                this.txt_total_vocabulary_index.text = "#" + (index + 1).ToString();
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
        this.v.Close();
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
        this.On_check_val_setting();
    }

    private void On_check_val_setting()
    {
        if (this.list_setting_val[2])
            this.txt_vocabulary_translate.gameObject.SetActive(true);
        else
            this.txt_vocabulary_translate.gameObject.SetActive(false);
    }

    public void play_Vibrate()
    {
        if (this.list_setting_val[1]) Handheld.Vibrate();
    }

}
