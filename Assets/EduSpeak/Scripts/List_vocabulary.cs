using System.Collections;
using Carrot;
using UnityEngine;
using UnityEngine.UI;

public class List_vocabulary : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;

    [Header("UI")]
    public GameObject panel_list_vocabulary;
    public Transform tr_all_item;
    public ScrollRect scrollRect;

    public int index_v_view=0;

    public void Show()
    {
        this.scrollRect.verticalNormalizedPosition = 1f;
        this.app.Check_ui_menu(2);
        this.panel_list_vocabulary.SetActive(true);
        this.app.carrot.clear_contain(this.tr_all_item);
        IDictionary data_level = (IDictionary)this.app.list_data[this.app.l.index_level];
        IList list_units = (IList)data_level["units"];
        IList text = (IList)((IDictionary)list_units[this.app.u.index_unit])["text"];
        IList vi = (IList)((IDictionary)list_units[this.app.u.index_unit])["vi"];
        for (int i = 0; i < text.Count; i++)
        {
            var index = i;
            var s_Vocabulary= text[i].ToString();
            var s_Translate= vi[i].ToString();

            IDictionary data_item=(IDictionary)list_units[i];

            GameObject obj = Instantiate(this.app.box_item_prefab);
            obj.transform.SetParent(this.tr_all_item);
            obj.transform.localScale = new Vector3(1, 1, 1);
            if(i%2==0) obj.GetComponent<Image>().color=this.app.color_a;

            Carrot_Box_Item box_Item = obj.GetComponent<Carrot_Box_Item>();
            box_Item.set_title(text[i].ToString());
            box_Item.set_tip(text[i].ToString());
            box_Item.set_icon_white(this.app.sp_vocabulary);
            box_Item.check_type();
            box_Item.set_act(() => {
                this.panel_list_vocabulary.SetActive(false);
                V_item v_item = new();
                v_item.s_key = s_Vocabulary;
                v_item.index_l=int.Parse(data_item["index_l"].ToString());
                v_item.index_week =this.app.u.index_unit;
                v_item.index_v_in_week = index;
                v_item.index_v = index+(10*int.Parse(data_item["index_week"].ToString()));
                v_item.s_Translate = s_Translate;
                this.index_v_view = v_item.index_v;
                this.app.Set_index_v_view(v_item.index_v);
                this.app.play_sound();
                this.app.v.On_Show(v_item);
            });
        }
    }

    public void On_back()
    {
        this.app.play_sound();
        this.panel_list_vocabulary.SetActive(false);
        this.app.u.Show();
    }
}
