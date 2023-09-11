using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


//自定义继承了MonoBehaviour的BuildingDetailUI类，用于实现物品详细信息的UI界面
public class BuildingDetailUI : MonoBehaviour
{
    //定义物品的属性：名称、描述、图标
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI MainText;
    public Image icon;

    //传入物品的Building，判断并显示该物品的名称、描述、图标
    public void Display(BuildingDetails Building)
    {
        nameText.text = Building.chineseName;
        MainText.text = "工程原理：" + Building.description + "\n\n" + "建造条件:" + Building.requirement + "\n\n" + "建筑功能：" + Building.function;
        icon.sprite = Building.icon;
        
        //物品图标不存在时，隐藏该图标
        if (Building.icon != null)
            icon.gameObject.SetActive(true);
    }

    //判断并清除显示的物品信息，如果物品图标不存在，则继续隐藏图标
    public void Clear()
    {
        nameText.text = null;
        MainText.text = null;
        icon.sprite = null;
        icon.gameObject.SetActive(false);
    }
}
