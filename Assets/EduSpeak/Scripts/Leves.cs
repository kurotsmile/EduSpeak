using System.Collections;
using Carrot;
using UnityEngine;
using UnityEngine.UI;

public class Leves : MonoBehaviour
{
    public App app;
    public int index_level=0;

    [Header("UI")]
    public GameObject panel_level;
    public Transform tr_all_item;

    public void Show()
    {
        this.app.Set_index_menu_cur(1);
        this.app.Check_ui_menu(1);
        this.panel_level.SetActive(true);
        this.app.carrot.clear_contain(this.tr_all_item);
        for (int i = 0; i < this.app.list_data.Count; i++)
        {
            var index = i;
            IDictionary data = (IDictionary)this.app.list_data[i];
            GameObject obj = Instantiate(this.app.box_item_prefab);
            obj.transform.SetParent(this.tr_all_item);
            obj.transform.localScale = new Vector3(1, 1, 1);
            if(i%2==0) obj.GetComponent<Image>().color=this.app.color_a;
            
            Carrot_Box_Item box_Item = obj.GetComponent<Carrot_Box_Item>();
            box_Item.set_title(data["name"].ToString());
            box_Item.set_tip(data["tip"].ToString());
            box_Item.set_icon_white(this.app.sp_class);
            box_Item.check_type();
            box_Item.set_act(()=>{
                this.panel_level.SetActive(false);
                this.index_level = index;
                this.app.u.Show();
                this.app.Btn_sel_menu(2);
            });
        }
    }

    public void On_Back(){
        this.app.Btn_show_home();
    }
}
