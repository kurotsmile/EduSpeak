using System.Collections;
using System.Collections.Generic;
using Carrot;
using UnityEngine;
using UnityEngine.UI;

public class List_vocabulary : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;
    public Sprite sp_banner;

    public void Show()
    {
        this.app.Set_index_menu_cur(3);
        this.app.box.Show("Vocabulary", this.sp_banner,On_back);
        this.app.Check_ui_menu(2);
        IDictionary data_level = (IDictionary)this.app.list_data[this.app.l.index_level];
        IList list_units = (IList)data_level["units"];
        IList text = (IList)((IDictionary)list_units[this.app.u.index_unit])["text"];
        IList vi = (IList)((IDictionary)list_units[this.app.u.index_unit])["vi"];
        IList f = (IList)((IDictionary)list_units[this.app.u.index_unit])["file"];
        IList arr_index = (IList)((IDictionary)list_units[this.app.u.index_unit])["arr_index"];
        IDictionary data_item=(IDictionary)list_units[this.app.u.index_unit];
        for (int i = 0; i < text.Count; i++)
        {
            var index = i;
            var s_Vocabulary= text[i].ToString();
            var s_Translate= vi[i].ToString();
            var s_file= "";
            if(this.app.is_sell) s_file=f[i].ToString();

            
            GameObject obj = Instantiate(this.app.box_item_prefab);
            obj.transform.SetParent(this.app.box.tr_all_item);
            obj.transform.localScale = new Vector3(1, 1, 1);
            if(i%2==0) obj.GetComponent<Image>().color=this.app.color_a;

            Carrot_Box_Item box_Item = obj.GetComponent<Carrot_Box_Item>();
            box_Item.set_title(text[i].ToString());
            if(this.app.status_translate)
                box_Item.set_tip(vi[i].ToString());
            else
                box_Item.set_tip("Vocabulary " + (i+1).ToString());
            box_Item.set_icon_white(this.app.sp_vocabulary);
            box_Item.check_type();
            box_Item.set_act(() => {
                this.app.box.Hide();
                V_item v_item = new();
                v_item.s_key = s_Vocabulary;
                v_item.index_l=int.Parse(data_item["index_l"].ToString());
                v_item.index_week =this.app.u.index_unit;
                v_item.index_v_in_week = index;
                v_item.index_v = int.Parse(arr_index[index].ToString());
                v_item.s_Translate = s_Translate;
                v_item.s_file =s_file;
                this.app.Set_index_v_view(v_item.index_v);
                this.app.play_sound();
                this.app.v.On_Show(v_item);
            });
        }
    }

    public void On_back()
    {
        this.app.play_sound();
        this.app.box.Hide();
        this.app.u.Show();
    }
}
