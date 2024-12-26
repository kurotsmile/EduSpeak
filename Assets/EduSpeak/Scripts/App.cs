using Carrot;
using KKSpeech;
using System.Collections;
using System.Collections.Generic;
using TextSpeech;
using UnityEngine;
using UnityEngine.UI;

public class V_item
{
    public string s_key;
    public string s_file="";
    public string s_Translate;
    public int index_v = 0;
    public int index_v_in_week = 0;
    public int index_week = 0;
}

public class App : MonoBehaviour
{
    [Header("Config")]
    public bool is_sell = true;

    [Header("Object Main")]
    public Carrot.Carrot carrot;
    public Carrot_ads_manage ads;
    public GameObject box_item_prefab;
    public GameObject lesson_item_prefab;
    public GameObject Vocabulary_item_prefab;
    public TextToSpeech texttospeech;
    public List_vocabulary list_vocabulary;
    public Vocabulary v;
    public Leves l;
    public Units u;

    [Header("UI")]
    public Image img_app_title;
    public GameObject img_logo_company;
    public Color32 color_a;
    public Color32 color_menu_normal;
    public Color32 color_menu_active;
    public GameObject panel_home;
    public Text txt_total_vocabulary;
    public Text txt_total_lesson;
    public Text txt_total_voice;
    public Image[] img_menu;
    public Text[] txt_menu;

    [Header("vocabulary")]
    public Text txt_lesson_title;

    [Header("Sound")]
    public AudioSource[] sound;
    public AudioSource sound_voice;

    [Header("Asset")]
    public Sprite sp_checkbox_true;
    public Sprite sp_checkbox_false;
    public Sprite sp_app_title_sell;
    public Sprite sp_app_title_public;
    public Sprite sp_class;
    public Sprite sp_unit;
    public Sprite sp_vocabulary;

    private IList<V_item> list_v = null;
    private int index_v_view = 0;
    private int index_menu_cur = 0;
    public IList list_data;

    void Start()
    {
        this.carrot.Load_Carrot(On_check_exit_app);
        this.carrot.color_highlight = this.color_menu_active;
        if (this.is_sell == false) this.ads.On_Load();

        this.panel_home.SetActive(true);
        this.v.On_Load();

        if (this.is_sell)
        {
            this.img_app_title.sprite = this.sp_app_title_sell;
            this.img_logo_company.SetActive(true);
        }
        else
        {
            this.img_app_title.sprite = this.sp_app_title_public;
            this.img_logo_company.SetActive(false);
        }

        if (this.is_sell)
        {
            TextAsset jsonFile = Resources.Load<TextAsset>("data");
            this.list_v = new List<V_item>();

            if (jsonFile != null)
            {
                int count_vocabulary = 0;
                this.list_data = (IList)Json.Deserialize(jsonFile.text);
                for (int i = 0; i < list_data.Count; i++)
                {
                    IDictionary dataLesson = (IDictionary)list_data[i];
                    dataLesson["index_week"] = i;
                    if (dataLesson["text"] != null)
                    {
                        IList list_txt = (IList)dataLesson["text"];
                        IList list_vi = (IList)dataLesson["vi"];
                        IList list_file = (IList)dataLesson["file"];
                        count_vocabulary += list_txt.Count;
                        for (int k = 0; k < list_txt.Count; k++)
                        {
                            V_item v_item = new V_item();
                            v_item.s_key = list_txt[k].ToString();
                            v_item.s_Translate = list_vi[k].ToString();
                            v_item.s_file = list_file[k].ToString();
                            v_item.index_week = i;
                            v_item.index_v = this.list_v.Count;
                            v_item.index_v_in_week = k;
                            this.list_v.Add(v_item);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                this.txt_total_lesson.text = list_data.Count + "\nWeek";
                this.txt_total_vocabulary.text = count_vocabulary + "\nVocabulary";
                this.txt_total_voice.text = count_vocabulary + "\nReading test";
            }
        }else{
            TextAsset jsonFile = Resources.Load<TextAsset>("data_en");
            if (jsonFile != null)
            {
                int count_vocabulary = 0;
                this.list_data = (IList)Json.Deserialize(jsonFile.text);
                for(int i=0;i<this.list_data.Count;i++){
                    IDictionary data = (IDictionary)this.list_data[i];
                }
            }
        }

        if (SpeechRecognizer.ExistsOnDevice())
        {
            SpeechRecognizerListener listener = FindAnyObjectByType<SpeechRecognizerListener>();
            listener.onAuthorizationStatusFetched.AddListener(v.OnAuthorizationStatusFetched);
            listener.onFinalResults.AddListener(v.OnFinalResult);
            SpeechRecognizer.RequestAccess();
        }
        else
        {
            v.txt_Status.text = "Sorry, but this device doesn't support speech recognition";
        }
        SpeechRecognizer.SetDetectionLanguage("en-US");
        this.Check_ui_menu();
    }

    private void On_check_exit_app()
    {
        if (v.panel_vocabulary.activeInHierarchy)
        {
            v.Close();
            this.carrot.set_no_check_exit_app();
        }
    }

    public void Btn_sel_menu(int index)
    {
        this.index_menu_cur = index;
        this.Check_ui_menu();
        this.Check_func_menu();
    }

    public void Check_ui_menu()
    {
        for (int i = 0; i < this.img_menu.Length; i++)
        {
            if (i == this.index_menu_cur)
            {
                this.img_menu[i].color = this.color_menu_active;
                this.txt_menu[i].color = this.color_menu_active;
            }
            else
            {
                this.img_menu[i].color = this.color_menu_normal;
                this.txt_menu[i].color = this.color_menu_normal;
            }
        }
    }

    public void Check_func_menu()
    {
        if (this.index_menu_cur == 0) this.Btn_show_home();
        if (this.index_menu_cur == 1) this.l.Show();
        if (this.index_menu_cur == 2) this.btn_on_start();
    }

    public void btn_on_start()
    {
        this.play_sound();
        if(this.is_sell)
            this.u.Show();
        else
            this.l.Show();
    }

    public void On_Show_view(IDictionary data)
    {
        if (this.is_sell == false) this.ads.On_show_interstitial();
        this.txt_lesson_title.text = data["name"].ToString();
        //this.carrot.clear_contain(this.area_all_vocabulary);
        IList list_txt = (IList)data["text"];
        IList list_file = (IList)data["file"];
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
            objVocabulary.transform.localScale = new Vector3(1, 1, 1);

            objVocabulary.GetComponent<Menu_Item>().txt.text = "Vocabulary " + (i + 1).ToString();
            if (i % 2 == 0) objVocabulary.GetComponent<Image>().color = this.color_a;
            objVocabulary.GetComponent<Menu_Item>().act = () =>
            {
                V_item v_item = new();
                v_item.s_key = s_Vocabulary;
                v_item.s_file = list_file[index].ToString();
                v_item.index_week = int.Parse(data["index_week"].ToString());
                v_item.index_v_in_week = index;
                v_item.index_v = (v_item.index_week * 5) + v_item.index_v_in_week;
                v_item.s_Translate = s_Translate;
                this.index_v_view = v_item.index_v;
                this.play_sound();
                this.v.On_Show(v_item);
            };
        }
        this.play_sound();
    }

    public void Back_home()
    {
        this.panel_home.SetActive(true);
        this.play_sound();
    }

    public void play_sound(int index = 0)
    {
        if (this.carrot.get_status_sound()) this.sound[index].Play();
    }

    public void show_setting()
    {
        this.carrot.Create_Setting();
    }

    public void Btn_show_home()
    {
        this.play_sound();
        this.panel_home.SetActive(true);
        this.l.panel_level.SetActive(false);
        this.u.panel_units.SetActive(false);
        this.list_vocabulary.panel_list_vocabulary.SetActive(false);
    }

    public void Btn_heart()
    {
        this.play_sound();
    }

    public void Btn_show_login()
    {
        this.play_sound();
        this.carrot.show_login();
    }

    public void play_Vibrate()
    {
        if (this.carrot.get_status_vibrate()) Handheld.Vibrate();
    }

    public void btn_next_v()
    {
        this.play_sound();
        this.index_v_view++;
        if (this.index_v_view >= this.list_v.Count) this.index_v_view = 0;
        this.v.On_Show(this.list_v[this.index_v_view]);
    }

    public void btn_prev_v()
    {
        this.play_sound();
        this.index_v_view--;
        if (this.index_v_view < 0) this.index_v_view = this.list_v.Count - 1;
        this.v.On_Show(this.list_v[this.index_v_view]);
    }

}
