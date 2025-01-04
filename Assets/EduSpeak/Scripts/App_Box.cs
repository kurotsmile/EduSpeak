using UnityEngine;
using UnityEngine.UI;
public class App_Box : MonoBehaviour
{
    [Header("Object Main")]
    public App app;

    [Header("UI")]
    public GameObject panel_box;
    public Image image_baner;
    public Text txt_title;
    public Transform tr_all_item;
    public ScrollRect scrollRect;

    public void On_load(){
        this.panel_box.SetActive(false);
    }

    public void Show(string title, Sprite baner){
        this.scrollRect.verticalNormalizedPosition = 1f;
        this.app.carrot.clear_contain(this.tr_all_item);
        this.panel_box.SetActive(true);
        this.panel_box.transform.SetAsLastSibling();
        this.app.tr_menu.SetAsLastSibling();
        this.txt_title.text = title;
        this.image_baner.sprite = baner;
    }

    public void Hide(){
        this.panel_box.SetActive(false);
    }
}
