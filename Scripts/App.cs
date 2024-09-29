using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class App : MonoBehaviour
{

    public GameObject panel_main;
    public GameObject lesson_item_prefab;

    [Header("UI")]
    public Transform area_all_lesson;

    void Start()
    {
        this.Clear_all_titem(area_all_lesson);
        for(int i = 0; i < 36; i++)
        {
            GameObject objLesson = Instantiate(this.lesson_item_prefab);
            objLesson.transform.SetParent(this.area_all_lesson);
            objLesson.transform.localScale = new Vector3(1, 1, 1);

            objLesson.GetComponent<Menu_Item>().txt.text= "Lesson week "+(i+1);
        }
    }

    
    void Update()
    {
        
    }

    public void btn_show_setting()
    {

    }

    public void Clear_all_titem(Transform tr)
    {
        foreach(Transform t in tr)
        {
            Destroy(t.gameObject);
        }
    }
}
