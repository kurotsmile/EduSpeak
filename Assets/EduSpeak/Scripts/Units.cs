using System.Collections;
using Carrot;
using UnityEngine;
using UnityEngine.UI;

public class Units : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;
    public Sprite sp_banner;
    public int index_unit=0;

    public void Show(){
        string s_title="Units";
        if(this.app.is_sell){
            s_title="Select week to start";
        }else{
            s_title="Units";
        }
        this.app.box.Show(s_title, this.sp_banner,On_Back);
        this.app.Set_index_menu_cur(2);
        this.app.Check_ui_menu(2);
        IDictionary data_level= (IDictionary)this.app.list_data[this.app.l.index_level];
        IList list_units = (IList)data_level["units"];
        for (int i = 0; i < list_units.Count; i++)
        {
            var index = i;
            IDictionary data = (IDictionary)list_units[i];
            GameObject obj = Instantiate(this.app.box_item_prefab);
            obj.transform.SetParent(this.app.box.tr_all_item);
            obj.transform.localScale = new Vector3(1, 1, 1);
            if(i%2==0) obj.GetComponent<Image>().color=this.app.color_a;

            Carrot_Box_Item box_Item = obj.GetComponent<Carrot_Box_Item>();
            box_Item.set_title(data["name"].ToString());
            if(data["tip"]==null) 
                box_Item.set_tip("Click to view vocabulary");
            else
                box_Item.set_tip(data["tip"].ToString());
            box_Item.set_icon_white(this.app.sp_unit);
            box_Item.check_type();
            box_Item.set_act(()=>{
                this.app.play_sound();
                this.index_unit=index;
                this.app.list_vocabulary.Show();
            });
        }
    }
    
    public void On_Back(){
        if(this.app.is_sell){
            this.app.Btn_show_home();
        }else{
            this.app.play_sound();
            this.app.box.Hide();
            this.app.l.Show();
        }
    }
}
