using System.Collections;
using Carrot;
using UnityEngine;
using UnityEngine.UI;

public class Units : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;

    [Header("UI")]
    public GameObject panel_units;
    public Transform tr_all_item;
    public int index_unit=0;

    public void Show(){
        this.panel_units.SetActive(true);
        this.app.carrot.clear_contain(this.tr_all_item);
        IDictionary data_level= (IDictionary)this.app.list_data[this.app.l.index_level];
        IList list_units = (IList)data_level["units"];
        for (int i = 0; i < list_units.Count; i++)
        {
            var index = i;
            IDictionary data = (IDictionary)list_units[i];
            GameObject obj = Instantiate(this.app.box_item_prefab);
            obj.transform.SetParent(this.tr_all_item);
            obj.transform.localScale = new Vector3(1, 1, 1);
            if(i%2==0) obj.GetComponent<Image>().color=this.app.color_a;

            Carrot_Box_Item box_Item = obj.GetComponent<Carrot_Box_Item>();
            box_Item.set_title(data["name"].ToString());
            box_Item.set_tip(data["tip"].ToString());
            box_Item.set_icon_white(this.app.sp_unit);
            box_Item.check_type();
            box_Item.set_act(()=>{
                this.index_unit=index;
                this.app.list_vocabulary.Show();
            });
        }
    }

    public void On_Back(){
        if(this.app.is_sell){
            this.app.Btn_show_home();
        }else{
            this.panel_units.SetActive(false);
            this.app.l.Show();
        }
    }
}
