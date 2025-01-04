using System.Collections;
using Carrot;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Favourite : MonoBehaviour
{
    [Header("Object Main")]
    public App app;
    public Sprite banner;

    private int length_favourite=0;

    public void On_Load(){
        this.length_favourite=PlayerPrefs.GetInt("length_favourite",0);
    }

    public void Show(){
        this.app.Set_index_menu_cur(4);
        this.app.Check_ui_menu(3);
        this.app.box.Show("Favourite",this.banner);
        this.Load_ui_list();
    }

    private void Load_ui_list(){
        this.app.carrot.clear_contain(this.app.box.tr_all_item);
        int count_item=0;
        for (int i = 0; i < this.length_favourite; i++)
        {
            if(PlayerPrefs.GetString("f_"+i,"")=="") continue;
            IDictionary data_item=(IDictionary)Json.Deserialize(PlayerPrefs.GetString("f_"+i));
            var index=i;
            var s_Vocabulary=data_item["s_key"].ToString();

            GameObject obj = Instantiate(this.app.box_item_prefab);
            obj.transform.SetParent(this.app.box.tr_all_item);
            obj.transform.localScale = new Vector3(1, 1, 1);
            if(count_item%2==0) obj.GetComponent<Image>().color=this.app.color_a;

            Carrot_Box_Item box_Item = obj.GetComponent<Carrot_Box_Item>();
            box_Item.set_title(s_Vocabulary);
            box_Item.set_tip(s_Vocabulary);
            box_Item.set_icon_white(this.app.sp_vocabulary);
            box_Item.check_type();
            box_Item.set_act(() => {
                V_item v_item = new();
                v_item.s_key = s_Vocabulary;
                v_item.s_Translate = data_item["s_Translate"].ToString();
                this.app.play_sound();
                this.app.v.On_Show(v_item);
            });

            Carrot_Box_Btn_Item btn_del=box_Item.create_item();
            btn_del.set_icon(this.app.carrot.sp_icon_del_data);
            btn_del.set_color(this.app.carrot.color_highlight);
            btn_del.set_act(()=>{
                this.app.carrot.play_sound_click();
                this.Delete(index,Load_ui_list);
            });
            count_item++;
        }

        if(count_item==0){
            GameObject obj = Instantiate(this.app.box_item_prefab);
            obj.transform.SetParent(this.app.box.tr_all_item);
            obj.transform.localScale = new Vector3(1, 1, 1);
            Carrot_Box_Item box_Item = obj.GetComponent<Carrot_Box_Item>();
            box_Item.set_title("List is empty");
            box_Item.set_tip("No favorite words have been added yet!");
            box_Item.set_icon_white(this.app.sp_sad);
            box_Item.check_type();
        }
    }

    public void On_back(){
        this.app.play_sound();
        this.app.box.Hide();
        this.app.Btn_show_home();
    }

    public void Add(V_item v){
        string s_data=JsonUtility.ToJson(v);
        PlayerPrefs.SetString("f_"+this.length_favourite,s_data);
        PlayerPrefs.SetString("f_key_"+this.length_favourite,v.s_key);
        this.length_favourite++;
        PlayerPrefs.SetInt("length_favourite",this.length_favourite);
        this.app.carrot.Show_msg("Favourite","Add to favourite success!");
    }

    public void Delete(int index,UnityAction act_done_del=null){
        string s_key=PlayerPrefs.GetString("f_key_"+index);
        this.app.carrot.Show_msg("Delete "+s_key,"Are you sure you want to remove this word from your favorites list?",()=>{
            PlayerPrefs.DeleteKey("f_"+index);
            PlayerPrefs.DeleteKey("f_key_"+index);
            act_done_del?.Invoke();
        });
    }

    public int Check_key(string s_key){
        for (int i = 0; i < this.length_favourite; i++)
        {
            if(PlayerPrefs.GetString("f_"+i,"")=="") continue;
            if(PlayerPrefs.GetString("f_key_"+i)==s_key){
                return i;
            };
        }
        return -1;
    }
}
