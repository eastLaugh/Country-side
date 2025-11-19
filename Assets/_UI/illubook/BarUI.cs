using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;     //引入Unity官方的基础框架，可以方便处理UI的基础事件（比如点击、拖拽……）
using UnityEngine.UI;
//using UnityEngine.UIElements;


//自定义VGF.Inventory，提供鼠标点击、悬停和离开时触发事件的接口

    //继承多个基类方便调用实现多种功能
public class BarUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    //定义背包物品的多个判定和显示指标
    private BuildingDetails BuildingDetails;
    public bool Selected = false;
    public Image image;
    public Image Mask;
    public TextMeshProUGUI BuildingName;
    public BuildingType slotType;
    public BuildingDetailUI BuildingDetailUI;
    public int slotIndex;
    
    //将image初始化为该Slot子物体的Image组件
    private void Start()
    {
        //BuildingDetails = new BuildingDetails();
    }

    //可加入个性化自定义的刷新内容
    void Update()
    {
        //
    }
    
    /// <summary>
    /// 更新Bar
    /// </summary>
    public void UpdateBuildingBar(BuildingDetails Building)     //更新并获取物品的名称和数量
    {
        BuildingDetails = Building;
        BuildingName.text = BuildingDetails.chineseName;
        image.sprite = null;
        if(BuildingDetails.unclock)
        {
            Mask.gameObject.SetActive(false); 
        }
        else
        {
            Mask.gameObject.SetActive(true);
        }
    }
    
    //将BuildingDetils置空，便于下一个物品的显示
    private void OnDestroy()
    {
        BuildingDetails = null;
    }

    //在背包槽位被点击时触发，将物品的BuildingDetails对象传递给Display()函数
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!BuildingDetails.unclock) return;
        EventHandler.CallInitSoundEffect(SoundName.BtnClick2);
        Selected = true;
        var color = image.color;
        color.a = 0.5f;                     //设置image的透明度设置为50%
        image.color = color;
        BuildingDetailUI.Display(BuildingDetails);

    }

    //当鼠标悬停在背包槽位时触发，设置图像显示的透明度，突出显示
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!BuildingDetails.unclock) return;
        if (!Selected)
        {
            var color = image.color;
            color.a = 0.8f;                 //设置image的透明度为80%
            image.color = color;
        }
    }

    //当鼠标从槽位上移开的时候触发，设置图像的透明度，恢复正常显示的样子
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!BuildingDetails.unclock) return;
        if (!Selected)
        {
            var color = image.color;
            color.a = 1f;                   //设置image的透明度为100%，即不透明
            image.color = color;
        }
    }
}

