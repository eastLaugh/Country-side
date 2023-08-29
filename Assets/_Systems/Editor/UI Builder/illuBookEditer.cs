using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System;
using System.Linq;

public class illuBookEditer : EditorWindow
{
    private illuBookData_SO database;
    private List<BuildingDetails> BuildingList=new List<BuildingDetails>();
    private VisualTreeAsset BuildingRowTemplate;
    //获得VisualElement
    private ListView BuildingListView;
    private ScrollView BuildingDetailsSection;
    private Sprite defaultIcon;
    private BuildingDetails activeBuilding;
    private VisualElement iconPreview;
    [MenuItem("CustomEditer/BuildingEditer")]
    
    public static void ShowExample()
    {
        illuBookEditer wnd = GetWindow<illuBookEditer>();
        wnd.titleContent = new GUIContent("BuildingEditer");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;



        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_Systems/Editor/UI Builder/illubookEditer.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);
        //拿到模板数据
        BuildingRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_Systems/Editor/UI Builder/ItemRowTemplate.uxml");
        //拿到默认Icon图片
        defaultIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Art/40+ Simple Icons - Free/CheckMark_Simple_Icons_UI.png");
        //变量赋值
        BuildingListView = root.Q<VisualElement>("BackGround").Q<ListView>("BuildingList");
        BuildingDetailsSection = root.Q<ScrollView>("BuildingDetails");
        iconPreview = BuildingDetailsSection.Q<VisualElement>("IconView");
        //获得按键
        root.Q<Button>("AddButton").clicked+=OnAddBuildingClicked;
        root.Q<Button>("DeleteButton").clicked += OnDeleteClicked;
        //加载数据
        LoadDataBase();
        //生成ListView
        GenerateListView();
    }
    #region 按键事件
    private void OnDeleteClicked()
    {
        BuildingList.Remove(activeBuilding);
        BuildingListView.Rebuild();
        BuildingDetailsSection.visible = false;

    }
    private void OnAddBuildingClicked()
    {
        BuildingDetails newBuilding = new BuildingDetails();
        //newBuilding.BuildingName = "NEW Building";
        //newBuilding.BuildingID = 1000 + BuildingList.Count;
        //newBuilding.BuildingIcon = defaultIcon;
        BuildingList.Add(newBuilding);
        BuildingListView.Rebuild();
        GetBuildingDetails();
        BuildingDetailsSection.visible = true;
    }
    #endregion
    private void LoadDataBase()//加载数据
    {
        var dataArray=AssetDatabase.FindAssets("illuBook Database");
        if (dataArray.Length == 1)
        {
            var path=AssetDatabase.GUIDToAssetPath(dataArray[0]);
            database = AssetDatabase.LoadAssetAtPath(path, typeof(illuBookData_SO)) as illuBookData_SO;
        }
        BuildingList=database.illuBookList;
        //如果不标记则无法记录数据
        EditorUtility.SetDirty(database);
        //Debug.Log(BuildingList[0].ID);
    }
    private void GenerateListView()
    {
        Func<VisualElement> makeBuilding = () => BuildingRowTemplate.CloneTree();
        Action<VisualElement, int> bindBuilding = (e, i) =>
        {
            if (i < BuildingList.Count)
            {
                if (BuildingList[i].icon!=null)
                  e.Q<VisualElement>("Icon").style.backgroundImage = BuildingList[i].icon.texture;
                else e.Q<VisualElement>("Icon").style.backgroundImage = defaultIcon.texture;
                e.Q<Label>("Name").text = BuildingList[i].name == null ? "NO Building" : BuildingList[i].name;
            }
        };
        BuildingListView.itemsSource = BuildingList;
        BuildingListView.makeItem = makeBuilding;
        BuildingListView.bindItem = bindBuilding;
        BuildingListView.selectionChanged += OnlistSelectiontChange;
        BuildingListView.itemsRemoved += OnlistRemoved;
        BuildingListView.itemsAdded += OnlistAdded;
        BuildingListView.visible = true;
        //右侧信息面板不可见
        BuildingDetailsSection.visible = false;
    }
    private void OnlistSelectiontChange(IEnumerable<object> selectedBuilding)
    {
        activeBuilding = (BuildingDetails)selectedBuilding.First();
        GetBuildingDetails();
        BuildingDetailsSection.visible = true;
    }
    private void OnlistRemoved(IEnumerable<int> index)
    {
        if(activeBuilding != null)
        {
            GetBuildingDetails();
            BuildingDetailsSection.visible = true;
        }   
    }
    private void OnlistAdded(IEnumerable<int> index)
    {
            Debug.Log(index.ToString());
            GetBuildingDetails();
            BuildingDetailsSection.visible = true;
    }
    private void GetBuildingDetails()
    {
        BuildingDetailsSection.MarkDirtyRepaint();
        BuildingDetailsSection.Q<IntegerField>("BuildingID").value = activeBuilding.ID;
        BuildingDetailsSection.Q<IntegerField>("BuildingID").RegisterValueChangedCallback(evt =>
        {
            activeBuilding.ID = evt.newValue;
        });
        BuildingDetailsSection.Q<TextField>("BuildingName").value = activeBuilding.name;
        BuildingDetailsSection.Q<TextField>("BuildingName").RegisterValueChangedCallback(evt =>
        {
            activeBuilding.name = evt.newValue;
            BuildingListView.Rebuild();
        });
        iconPreview.style.backgroundImage = activeBuilding.icon == null ? defaultIcon.texture : activeBuilding.icon.texture;
        BuildingDetailsSection.Q<ObjectField>("BuildingIcon").value = activeBuilding.icon;
        BuildingDetailsSection.Q<ObjectField>("BuildingIcon").RegisterValueChangedCallback(evt =>
        {
            Sprite newIcon=evt.newValue as Sprite;
            activeBuilding.icon = newIcon;
            iconPreview.style.backgroundImage= newIcon==null?defaultIcon.texture:newIcon.texture;
            BuildingListView.Rebuild();
        });
        //其他变量绑定

        //BuildingDetailsSection.Q<EnumField>("BuildingType").Init(activeBuilding.buildingType);
        //BuildingDetailsSection.Q<EnumField>("BuildingType").value = activeBuilding.buildingType;
        //BuildingDetailsSection.Q<EnumField>("BuildingType").RegisterValueChangedCallback(evt =>
        //{
        //    activeBuilding.buildingType = (BuildingType)evt.newValue;
        //});
        //
        BuildingDetailsSection.Q<TextField>("BuildingDescription").value = activeBuilding.description;
        BuildingDetailsSection.Q<TextField>("BuildingDescription").RegisterValueChangedCallback(evt =>
        {
            activeBuilding.description = evt.newValue;
        });
        BuildingDetailsSection.Q<TextField>("BuildingRequire").value = activeBuilding.requirement;
        BuildingDetailsSection.Q<TextField>("BuildingRequire").RegisterValueChangedCallback(evt =>
        {
            activeBuilding.requirement = evt.newValue;
        });
        BuildingDetailsSection.Q<TextField>("BuildingFunction").value = activeBuilding.function;
        BuildingDetailsSection.Q<TextField>("BuildingFunction").RegisterValueChangedCallback(evt =>
        {
            activeBuilding.function = evt.newValue;
        });
    }
}