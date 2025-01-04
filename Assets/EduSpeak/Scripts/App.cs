using Carrot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class V_item
{
    public string s_key;
    public string s_file = "";
    public string s_Translate;
    public int index_l = 0;
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
    public TextToSpeech texttospeech;
    public SpeechToText speechtotext;
    public List_vocabulary list_vocabulary;
    public Vocabulary v;
    public Leves l;
    public Units u;
    public Favourite f;
    public App_Box box;
    public Carrot_DeviceOrientationChange deviceOrientationChange;

    [Header("UI")]
    public Image img_app_title;
    public GameObject img_logo_company;
    public Color32 color_a;
    public Color32 color_menu_normal;
    public Color32 color_menu_active;
    public Text txt_total_vocabulary;
    public Text txt_total_level;
    public Text txt_total_lesson;
    public Text txt_total_voice;
    public Image[] img_menu;
    public Text[] txt_menu;
    public Transform tr_landscape;
    public Transform tr_landscape_left;
    public Transform tr_landscape_right;
    public Transform tr_portrait;
    public Transform tr_menu;
    public Transform tr_area_start;
    public Transform tr_area_banner;
    public Transform tr_area_info;

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
    public Sprite sp_translate;
    public Sprite sp_no_translate;
    public Sprite sp_heart;
    public Sprite sp_broken_heart;
    public Sprite sp_sad;
    private IList<V_item> list_v = null;
    private int index_v_view = 0;
    private int index_menu_cur = 0;
    public bool status_translate = true;
    public IList list_data;

    private string s_url_data = "https://raw.githubusercontent.com/kurotsmile/EduSpeak/refs/heads/main/Assets/Resources/data.json";

    void Start()
    {
        this.carrot.Load_Carrot(On_check_exit_app);
        this.carrot.color_highlight = this.color_menu_active;
        this.carrot.shop.onCarrotPaySuccess += onCarrotPaySuccess;
        if(carrot.os_app!=OS.Window){
            if (this.is_sell == false) this.ads.On_Load();
        }
        
        this.v.On_Load();
        this.Check_Resolution_change();
        TextAsset jsonFile;
        if (this.is_sell)
        {
            jsonFile = Resources.Load<TextAsset>("data");
            this.img_app_title.sprite = this.sp_app_title_sell;
            this.img_logo_company.SetActive(true);
        }
        else
        {
            jsonFile = Resources.Load<TextAsset>("data_en");
            this.img_app_title.sprite = this.sp_app_title_public;
            this.img_logo_company.SetActive(false);
        }

        if (jsonFile != null) this.Load_Data_App(jsonFile.text);

        this.speechtotext.On_Load();
        this.texttospeech.On_Load();

        if (PlayerPrefs.GetInt("status_translate", 1) == 1)
            this.status_translate = true;
        else
            this.status_translate = false;

        this.Check_status_translate();
        this.Check_ui_menu(0);
        this.f.On_Load();
        this.box.On_load();
    }

    private void Load_Data_App(string s_data)
    {
        int count_vocabulary = 0;
        int count_unit = 0;
        this.list_data = (IList)Json.Deserialize(s_data);

        this.list_v = new List<V_item>();
        for (int i = 0; i < list_data.Count; i++)
        {
            IDictionary dataLevel = (IDictionary)list_data[i];
            IList list_unit = (IList)dataLevel["units"];
            count_unit += list_unit.Count;
            for (int k = 0; k < list_unit.Count; k++)
            {
                IDictionary dataUnit = (IDictionary)list_unit[k];
                dataUnit["index_l"] = i;
                dataUnit["index_week"] = k;
                IList<int> arr_index = new List<int>();
                if (dataUnit["text"] != null)
                {
                    IList list_vocabulary = (IList)dataUnit["text"];
                    for (int j = 0; j < list_vocabulary.Count; j++)
                    {
                        V_item v_item = new();
                        v_item.s_key = list_vocabulary[j].ToString();
                        v_item.s_Translate = ((IList)dataUnit["vi"])[j].ToString();
                        v_item.index_v = count_vocabulary;
                        v_item.index_v_in_week = j;
                        v_item.index_week = k;
                        v_item.index_l = i;
                        list_v.Add(v_item);
                        arr_index.Add(count_vocabulary);
                        count_vocabulary++;
                    }
                }
                dataUnit["arr_index"] = arr_index;
            }
        }
        this.txt_total_level.text = list_data.Count + "\nLevel";
        if (this.is_sell)
            this.txt_total_lesson.text = count_unit + "\nWeek";
        else
            this.txt_total_lesson.text = count_unit + "\nUnit";
        this.txt_total_vocabulary.text = count_vocabulary + "\nVocabulary";
        this.txt_total_voice.text = count_vocabulary + "\nReading test";
    }

    private void On_check_exit_app()
    {
        if (index_menu_cur == 1)
        {
            this.l.On_Back();
            this.carrot.set_no_check_exit_app();
        }
        else if (index_menu_cur == 2)
        {
            this.u.On_Back();
            this.carrot.set_no_check_exit_app();
        }
        else if (index_menu_cur == 3)
        {
            this.list_vocabulary.On_back();
            this.carrot.set_no_check_exit_app();
        }
        else if (index_menu_cur == 4)
        {
            this.f.On_back();
            this.carrot.set_no_check_exit_app();
        }
        else if (index_menu_cur == 5)
        {
            this.v.Close();
            this.carrot.set_no_check_exit_app();
        }
    }

    public void Btn_sel_menu(int index)
    {
        this.play_sound();
        this.index_menu_cur = index;
        this.Check_func_menu();
    }

    private void Check_status_translate()
    {
        if (this.status_translate)
            this.v.txt_vocabulary_translate.gameObject.SetActive(true);
        else
            this.v.txt_vocabulary_translate.gameObject.SetActive(false);
    }

    public void Check_ui_menu(int index_show)
    {
        for (int i = 0; i < this.img_menu.Length; i++)
        {
            if (i == index_show)
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
        if (this.index_menu_cur == 2) this.u.Show();
        if (this.index_menu_cur == 3) this.f.Show();
    }

    public void btn_on_start()
    {
        this.play_sound();
        if (this.is_sell)
            this.u.Show();
        else
            this.l.Show();
    }

    public void play_sound(int index = 0)
    {
        if (this.carrot.get_status_sound()) this.sound[index].Play();
    }

    public void show_setting()
    {
        Carrot_Box box_setting = this.carrot.Create_Setting();
        Carrot_Box_Item item_translate = box_setting.create_item_of_top("item_tr");
        if (this.status_translate)
            item_translate.set_icon_white(this.sp_translate);
        else
            item_translate.set_icon_white(this.sp_no_translate);
        item_translate.set_title("Show translation");
        item_translate.set_tip("Turn vocabulary translation display on or off");
        item_translate.set_act(() =>
        {
            this.carrot.play_sound_click();
            if (this.status_translate)
            {
                this.status_translate = false;
                PlayerPrefs.SetInt("status_translate", 0);
                item_translate.set_icon(this.sp_no_translate);
                item_translate.img_icon.color = Color.black;
            }
            else
            {
                this.status_translate = true;
                PlayerPrefs.SetInt("status_translate", 1);
                item_translate.set_icon_white(this.sp_translate);
            }
            this.Check_status_translate();
        });
    }

    public void Btn_show_home()
    {
        this.box.Hide();
        this.play_sound();
        this.index_menu_cur = 0;
        this.Check_ui_menu(0);
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

    public void Set_index_v_view(int index)
    {
        this.index_v_view = index;
    }

    public void Set_index_menu_cur(int index)
    {
        this.index_menu_cur = index;
    }

    private void onCarrotPaySuccess(string id_p)
    {
        if (id_p == this.carrot.shop.get_id_by_index(1))
        {
            this.ads.RemoveAds();
            this.carrot.Show_msg("Remove Ads", "Ad removed successfully!", Msg_Icon.Success);
        }
    }

    public void Check_Resolution_change()
    {
        this.carrot.delay_function(1f, () =>
        {

            if (deviceOrientationChange.Get_status_portrait())
            {
                this.tr_area_start.SetParent(this.tr_portrait);
                this.tr_area_info.SetParent(this.tr_portrait);
                this.tr_area_banner.SetParent(this.tr_portrait);
                this.box.panel_box.transform.SetParent(this.tr_portrait);
                this.tr_menu.SetParent(this.tr_portrait);
                this.tr_menu.SetAsLastSibling();
                this.tr_area_banner.SetAsLastSibling();

                RectTransform rectTransform_start = this.tr_area_start.GetComponent<RectTransform>();
                rectTransform_start.anchorMin = new Vector2(0, 0);
                rectTransform_start.anchorMax = new Vector2(1, 0);
                rectTransform_start.offsetMin = new Vector2(0, 0);
                rectTransform_start.offsetMax = new Vector2(0, 317.87f);

                RectTransform rectTransform_banner = this.tr_area_banner.GetComponent<RectTransform>();
                rectTransform_banner.anchoredPosition = new Vector2(0, -160.35f);

                RectTransform rectTransform_info = this.tr_area_info.GetComponent<RectTransform>();
                rectTransform_info.anchorMin = new Vector2(0, 0);
                rectTransform_info.anchorMax = new Vector2(1, 0);
                rectTransform_info.offsetMin = new Vector2(0, 310);
                rectTransform_info.offsetMax = new Vector2(0, 466);
            }
            else
            {
                this.tr_area_start.SetParent(this.tr_landscape_right);
                this.tr_menu.SetParent(this.tr_landscape_left);
                this.tr_area_banner.SetParent(this.tr_landscape_left);
                this.tr_area_info.SetParent(this.tr_landscape_right);
                this.box.panel_box.transform.SetParent(this.tr_landscape_right);

                RectTransform rectTransform_start = this.tr_area_start.GetComponent<RectTransform>();
                rectTransform_start.anchorMin = new Vector2(0, 0);
                rectTransform_start.anchorMax = new Vector2(1, 0);
                rectTransform_start.offsetMin = new Vector2(0, 0);
                rectTransform_start.offsetMax = new Vector2(0, 220f);

                RectTransform rectTransform_banner = this.tr_area_banner.GetComponent<RectTransform>();
                rectTransform_banner.anchoredPosition = new Vector2(0, -120f);

                RectTransform rectTransform_info = this.tr_area_info.GetComponent<RectTransform>();

                rectTransform_info.anchorMin = new Vector2(0, 1);
                rectTransform_info.anchorMax = new Vector2(1, 1);
                rectTransform_info.offsetMin = new Vector2(0, -60f);
                rectTransform_info.offsetMax = new Vector2(0, 95.26f);
                rectTransform_info.pivot = new Vector2(0.5f, 1);
            }

            RectTransform rectTransform_menu = this.tr_menu.GetComponent<RectTransform>();
            rectTransform_menu.anchorMin = new Vector2(0, 0);
            rectTransform_menu.anchorMax = new Vector2(1, 0);
            rectTransform_menu.offsetMin = new Vector2(0, 0);
            rectTransform_menu.offsetMax = new Vector2(0, 93.7f);

            RectTransform rectTransform_box = this.box.panel_box.GetComponent<RectTransform>();
            rectTransform_box.anchorMin = new Vector2(0, 0);
            rectTransform_box.anchorMax = new Vector2(1, 1);
            rectTransform_box.offsetMin = new Vector2(0, 0);
            rectTransform_box.offsetMax = new Vector2(0, 0);
        });
    }
}
