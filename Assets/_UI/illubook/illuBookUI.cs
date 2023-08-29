using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//ʵ������Ϸ�д򿪺͹رձ�����UI���桢���±�����UI���桢ѡ�б����е���Ʒ�ȹ���
public class illuBookUI : MonoBehaviour
{
    
    public BuildingDetailUI BuildingDetailUI;
    public BarUI BarPrefab;               //��ű������ӵ�UIԤ����
    public Transform RuleItemRoot;          //��ű������ӵĸ�����
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
    //����������ʱ����������
    private void OnEnable()
    {
        
    }

    //���������ر�ʱ���Ƴ�����
    private void OnDisable()
    {
        
        currentSelectedBar = null;
    }

    //ͨ����Ӽ�����ť�ĵ���¼��ķ�ʽ��ʵ����ͨ����ť�򿪺͹رձ���UI����Ĺ���
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
    
    //����location����������λ�ã���list����Ʒ�б���ʵʱ���±�����UI����
    private void OnUpdateilluBookUI(string name)
    {
        if(illuBookSystem.GetBuildingDetails(name) == null) { return; }
        if (illuBookSystem.GetBuildingDetails(name).unclock) return;
        illuBookSystem.Unlock(name);
        //��ʱ������б����ڵ���Ʒ
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
        //ʵ����BarUI������������

    }
    
    //ͨ������BarUI��ѡ�е�ǰ��BarUI������BarUI��ͼ���Ϊ��ɫ
    private void OnBarSelectedChange(BarUI BarUI)
    {
        if (currentSelectedBar != null)
        {
            /*����ͨ���޸�Color(Transparency, R, G, B)��ֵ�ﵽ��ͬ����ʾЧ����
            ��������a��alpha������Transparency��͸���ȣ�*/
            currentSelectedBar.image.color = new Color(1, 1, 1, 1);    
            currentSelectedBar.Selected = false;
        }
        currentSelectedBar = BarUI;
    }

    //��֡���ã�����û��Ƿ���"B"������/�رձ���
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
    
    //�����򿪵ļ��
    void OpenilluBookUI()
    {
        illuBookPanel.SetActive(true);
        timeSystem.Pause();
        isOpen = true;
    }
    
    //�����رյļ��
    void CloseilluBookUI()
    {
        illuBookPanel.SetActive(false);
        timeSystem.Continue();
        isOpen = false;
    }
}
