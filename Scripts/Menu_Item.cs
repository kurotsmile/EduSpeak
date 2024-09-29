using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Menu_Item : MonoBehaviour
{
    public Text txt;
    public Image icon;
    public UnityAction act;

    public void On_click()
    {
        if(act != null) act();
    }
}
