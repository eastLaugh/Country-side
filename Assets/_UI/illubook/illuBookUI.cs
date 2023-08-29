using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//实现在游戏中打开和关闭背包的UI界面、更新背包的UI界面、选中背包中的物品等功能
public class illuBookUI : MonoBehaviour
{
    
    public BuildingDetailUI BuildingDetailUI;
    public BarUI BarPrefab;               //存放背包格子的UI预制体
    public Transform RuleItemRoot;          //存放背包格子的父物体
    private BarUI currentSelectedBar;
    public GameObject illuBookPanel;
    private bool isOpen = false;
    //public Button btnOpen;
    public Button btnClose;

    private illuBookSystem illuBookSystem;
    private TimeSystem timeSystem;
    private List<BuildingDetails> BuildingList => GameManager.current.illuBookData.illuBookList;
    public void Initialize(illuBookSystem illuBookSystem,TimeSystem timeSystem)
    {
        this.illuBookSystem = illuBookSystem;
        this.timeSystem = timeSystem;
        EventHandler.BarSelectedChanged += OnBarSelectedChange;
        EventHandler.illuBookUnlocked += OnUpdateilluBookUI;
        for (int i = 0; i < BuildingList.Count; i++)
        {
            //var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
            var Bar = Instantiate(BarPrefab, RuleItemRoot);
            Bar.UpdateBuildingBar(BuildingList[i]);
            Bar.gameObject.SetActive(true);
        }
    }
    //当背包被打开时，触发监听
    private void OnEnable()
    {
        
    }

    //当背包被关闭时，移除监听
    private void OnDisable()
    {
        
        currentSelectedBar = null;
    }

    //通过添加监听按钮的点击事件的方式，实现了通过按钮打开和关闭背包UI界面的功能
    void Start()
    {
        //btnOpen.onClick.AddListener(() =>
        //{
        //    if (!isOpen)
        //    {
        //        OpenilluBookUI();
        //    }
        //});
        btnClose.onClick.AddListener(() =>
        {
            if (isOpen)
            {
                CloseilluBookUI();
            }
        });

    }
    
    //传入location（背包所在位置）和list（物品列表），实时更新背包的UI界面
    private void OnUpdateilluBookUI(string name)
    {
        if(illuBookSystem.GetBuildingDetails(name) == null) { return; }
        if (illuBookSystem.GetBuildingDetails(name).unclock) return;
        illuBookSystem.Unlock(name);
        //暂时清除所有背包内的物品
        if (RuleItemRoot.childCount > 0)
        {
            for (int i = 0; i < RuleItemRoot.childCount; i++)
            {
                Destroy(RuleItemRoot.GetChild(i).gameObject);
            }
        }
        BuildingDetailUI.Clear();
        for (int i = 0; i < BuildingList.Count; i++)
        {
                //var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                var Bar = Instantiate(BarPrefab, RuleItemRoot);
                Bar.UpdateBuildingBar(BuildingList[i]);
                Bar.gameObject.SetActive(true);
        }
        //实例化BarUI并更新其内容

    }
    
    //通过传入BarUI来选中当前的BarUI，并将BarUI的图像改为灰色
    private void OnBarSelectedChange(BarUI BarUI)
    {
        if (currentSelectedBar != null)
        {
            /*可以通过修改Color(Transparency, R, G, B)的值达到不同的显示效果，
            代码中用a（alpha）代替Transparency（透明度）*/
            currentSelectedBar.image.color = new Color(1, 1, 1, 1);    
            currentSelectedBar.Selected = false;
        }
        currentSelectedBar = BarUI;
    }

    //逐帧调用，检测用户是否按下"B"键来打开/关闭背包
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (!isOpen)
            {
                OpenilluBookUI();
            }
            else
            {
                CloseilluBookUI();
            }
        }

    }
    
    //背包打开的监测
    void OpenilluBookUI()
    {
        illuBookPanel.SetActive(true);
        timeSystem.Pause();
        isOpen = true;
    }
    
    //背包关闭的监测
    void CloseilluBookUI()
    {
        illuBookPanel.SetActive(false);
        timeSystem.Continue();
        isOpen = false;
    }
}
