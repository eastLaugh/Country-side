using System.Collections;           //该命名空间包含C#中许多常用的数据结构和算法
using System.Collections.Generic;   //同上
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;


//自定义命名空间，该类用于管理玩家背包中的物品，供其他模块系统引用
//继承Singleton<InventoryManager>，便于其他类引用这两个类的内容
public class illuBookSystem : MonoBehaviour
{
    public BuildingDetailUI BuildingDetailUI;
    public BarUI BarPrefab;               //存放背包格子的UI预制体
    public Transform RuleItemRoot;          //存放背包格子的父物体
    private BarUI currentSelectedBar;
    public GameObject illuBookPanel;
    private bool isOpen = false;
    [SerializeField] private TimeController timeController;
    [SerializeField] private GameObject illuBookHint;
    public Button btnClose;
    public Button btnOpen;
    private List<BuildingDetails> illuBookList => GameManager.current.illuBookData.illuBookList;
    GlobalData globalData=>GameManager.globalData;
    public BuildingDetails GetBuildingDetails(string name)       
    {
        return illuBookList.Find(i => i.name == name);
    }
    private void OnDisable()
    {
        currentSelectedBar = null;
    }

    private void Awake()
    {
        //if (globalData == null) { return; }
        Debug.Log("Awake");
        for (int i = 0; i < illuBookList.Count; i++)
        {
            if (globalData.unlockIlluBookName.Contains(illuBookList[i].name))
                illuBookList[i].unclock = true;
            else
                illuBookList[i].unclock = false;
        }
        EventHandler.BarSelectedChanged += OnBarSelectedChange;
        EventHandler.illuBookUnlocked += OnUpdateilluBookUI;
        for (int i = 0; i < illuBookList.Count; i++)
        {
            //var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
            var Bar = Instantiate(BarPrefab, RuleItemRoot);
            Bar.UpdateBuildingBar(illuBookList[i]);
            Bar.gameObject.SetActive(true);
        }
        btnOpen.onClick.AddListener(() =>
        {
            OpenilluBookUI();
            illuBookHint.SetActive(false);
        });
        //EventHandler.illuBookUnlocked += Unlock;
    }

    public void Unlock(string name)
    {
        globalData.unlockIlluBookName.Add(name);
        GameManager.SaveGlobalData();
        var detail = GetBuildingDetails(name);
        detail.unclock = true;
        illuBookHint.SetActive(true);
        
    }
    private void Start()
    {
        btnClose.onClick.AddListener(() =>
        {
            if (isOpen)
            {
                CloseilluBookUI();
            }
        });
    }
    private void OnUpdateilluBookUI(string name)
    {
        if (GetBuildingDetails(name) == null) { return; }
        if (GetBuildingDetails(name).unclock) return;
        Unlock(name);
        if (RuleItemRoot.childCount > 0)
        {
            for (int i = 0; i < RuleItemRoot.childCount; i++)
            {
                Destroy(RuleItemRoot.GetChild(i).gameObject);
            }
        }
        BuildingDetailUI.Clear();
        for (int i = 0; i < illuBookList.Count; i++)
        {
            //var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
            var Bar = Instantiate(BarPrefab, RuleItemRoot);
            Bar.UpdateBuildingBar(illuBookList[i]);
            Bar.gameObject.SetActive(true);
        }
    }
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
    void OpenilluBookUI()
    {
        illuBookPanel.SetActive(true);
        timeController.Pause();
        isOpen = true;
    }

    void CloseilluBookUI()
    {
        illuBookPanel.SetActive(false);
        timeController.Continue();
        isOpen = false;
    }
}

