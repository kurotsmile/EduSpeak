using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class App : MonoBehaviour
{
    public Carrot.Carrot carrot;

    void Start()
    {
        this.carrot.Load_Carrot();
    }

    
    void Update()
    {
        
    }

    public void btn_show_setting()
    {
        this.carrot.Create_Setting();
    }
}
