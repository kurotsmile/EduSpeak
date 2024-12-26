using System.Collections;
using Carrot;
using UnityEngine;

public class Units : MonoBehaviour
{
    public App app;
    public Transform tr_all_item;
    public int index_unit=0;

    public void Show(){
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

            Carrot_Box_Item box_Item = obj.GetComponent<Carrot_Box_Item>();
            box_Item.set_title(data["name"].ToString());
            box_Item.set_tip(data["tip"].ToString());
            box_Item.set_icon_white(this.app.sp_class);
            box_Item.check_type();
            box_Item.set_act(()=>{
                this.index_unit=index;
                this.app.list_vocabulary.Show();
            });
        }
    }
}
