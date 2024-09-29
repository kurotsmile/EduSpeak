using Carrot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class App : MonoBehaviour
{
    [Header("Object Main")]
    public GameObject lesson_item_prefab;
    public GameObject Vocabulary_item_prefab;

    [Header("UI")]
    public GameObject panel_main;
    public GameObject panel_view;
    public GameObject panel_vocabulary;
    public Transform area_all_lesson;
    public Transform area_all_vocabulary;

    [Header("vocabulary")]
    public Text txt_vocabulary_title;

    [Header("Sound")]
    public AudioSource[] sound;

    void Start()
    {
        this.panel_view.SetActive(false);
        this.panel_vocabulary.SetActive(false);

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
        
    }

    public void On_Show_view(IDictionary data)
    {
        this.Clear_all_item(this.area_all_vocabulary);
        IList list_txt = (IList) data["text"];
        for (int i = 0; i < list_txt.Count; i++)
        {
            var s_Vocabulary = list_txt[i].ToString();
            GameObject objVocabulary = Instantiate(this.Vocabulary_item_prefab);
            objVocabulary.transform.SetParent(this.area_all_vocabulary);
            objVocabulary.transform.localScale = new Vector3(1, 1, 1);

            objVocabulary.GetComponent<Menu_Item>().txt.text = (i+1).ToString();
            objVocabulary.GetComponent<Menu_Item>().act = () =>
            {
                this.play_sound();
                this.panel_vocabulary.SetActive(true);
                this.txt_vocabulary_title.text = s_Vocabulary;
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
        this.panel_view.SetActive(false);
        this.play_sound();
    }

    public void play_sound(int index = 0)
    {
        this.sound[index].Play();
    }

    public void Back_List_Lesson()
    {
        this.panel_view.SetActive(true);
        this.panel_vocabulary.SetActive(false);
        this.play_sound();
    }
}
