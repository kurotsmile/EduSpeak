using UnityEngine;

public class Favourite : MonoBehaviour
{
    [Header("Object Main")]
    public App app;

    [Header("UI")]
    public GameObject panel_favourite;

    public void Show(){
        this.panel_favourite.SetActive(true);
    }

    public void On_back(){
        this.panel_favourite.SetActive(false);
        this.app.Btn_show_home();
    }

    public void Add(){
        this.app.carrot.Show_msg("Favourite","Add to favourite success!");
    }
}
