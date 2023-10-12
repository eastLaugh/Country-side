using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BuildingWindow : MonoBehaviour
{
    Map map;
    public bool isMaploaded = false;
    public static event Action<Type> OnUpdate;
    public RectTransform Content;
    public GameObject ButtonPattern;
    [SerializeField] Button btnRemove;
    List<BuildingBtn> OptionButtons = new List<BuildingBtn>();

    private void OnEnable()
    {
        GameManager.OnMapLoaded += OnMapLoaded;
        GameManager.OnMapUnloaded += OnMapUnloaded;
        SlotRender.OnAnySlotEnter += OnAnySlotEnter;
    }

    private void OnDisable()
    {
        Debug.Log("disable");
        SlotRender.OnAnySlotEnter -= OnAnySlotEnter;
        GameManager.OnMapUnloaded -= OnMapUnloaded;
    }
    private void OnMapUnloaded()
    {
        isMaploaded = false;
        EventHandler.MoneyUpdate -= CheckCost;
        EventHandler.BuildingWindowUpdate -= UpdateWindow;
        EventHandler.PhaseUpdate -= CheckPhase;
    }
    private void OnMapLoaded(Map map)
    {
        this.map = map;
        isMaploaded = true;
        EventHandler.MoneyUpdate += CheckCost;
        EventHandler.BuildingWindowUpdate += UpdateWindow;
        EventHandler.PhaseUpdate += CheckPhase;
        UpdateWindow(ConstructType.House);
    }
    SlotRender enteredSlotRender;
    private void OnAnySlotEnter(SlotRender render)
    {
        enteredSlotRender = render;
    }

    // Start is called before the first frame update
    void CheckCost(float money)
    {
        if(SelectedType!=null)
        {
            var ins = Activator.CreateInstance(SelectedType) as IConstruction;
            if (ins.Cost > money && BuildMode.hasEntered)
            {
                lastSelectedButton.GetComponent<Image>().color = Color.white;
                MouseAnimator.SetTrigger("BuildEnd");
                SelectedType = null;
                OnUpdate?.Invoke(null);
            }
        }      
        for (int i = 0; i < OptionButtons.Count; i++)
        {
            OptionButtons[i].SetCurrentData(money: money);
            if (!OptionButtons[i].gameObject.activeSelf)
            {
                OptionButtons[i].gameObject.SetActive(true);
            }
        }
    }
    void CheckPhase(int phase)
    {
        for (int i = 0; i < OptionButtons.Count; i++)
        {
            OptionButtons[i].SetCurrentData(phase: phase);
            
        }
    }
    void Start()
    {
        Debug.Log("Start");
        Debug.Log(btnRemove);
        btnRemove.onClick.AddListener(() =>
        {
            OnButtonClick(typeof(MapObjects.DeletingFlag), btnRemove);
        });
        ButtonPattern.gameObject.SetActive(false);

        
    }
    private void UpdateWindow(ConstructType constructType)
    {
        //TODO
        if (BuildMode.hasEntered)
        {
            MouseAnimator.SetTrigger("BuildEnd");
            SelectedType = null;
            OnUpdate?.Invoke(null);

        }
        lastSelectedButton = null;       
        for (int i = 0;i<OptionButtons.Count;i++)
        {
            Destroy(OptionButtons[i].gameObject);
        }
        OptionButtons.Clear();
        foreach (Type mapObjectType in typeof(MapObjects).GetNestedTypes())
        {
            if (!mapObjectType.IsAbstract && typeof(IConstruction).IsAssignableFrom(mapObjectType))
            {
                //Debug.Log("entered");
                var ins = Activator.CreateInstance(mapObjectType) as Slot.MapObject;
                var insConstructInfo = ins as IConstruction;
                if(insConstructInfo.constructType == constructType && ins is not MapObjects.Administration)
                {                 
                    GameObject buttonGameObject = Instantiate(ButtonPattern, Vector3.zero, Quaternion.identity, Content);
                    var buildingBtn = buttonGameObject.GetComponent<BuildingBtn>();
                    buildingBtn.Initialize(ins, map.Phase, map.MainData.Money, (button) =>
                    {
                        EventHandler.CallInitSoundEffect(SoundName.BtnClick1);
                        OnButtonClick(mapObjectType, button);
                    });
                    OptionButtons.Add(buildingBtn);
                  
                }
#if UNITY_EDITOR
                if(insConstructInfo.constructType == constructType && ins is MapObjects.Administration)
                {

                    GameObject buttonGameObject = Instantiate(ButtonPattern, Vector3.zero, Quaternion.identity, Content);
                    var buildingBtn = buttonGameObject.GetComponent<BuildingBtn>();
                    buildingBtn.Initialize(ins, map.Phase, map.MainData.Money, (button) =>
                    {
                        EventHandler.CallInitSoundEffect(SoundName.BtnClick1);
                        OnButtonClick(mapObjectType, button);
                    });
                    OptionButtons.Add(buildingBtn);
                    buttonGameObject.SetActive(true);
                }
#endif
            }

        }
        CheckPhase(map.Phase);
        CheckCost(map.MainData.Money);    
    }
    public Animator MouseAnimator;
    static Type SelectedType { get; set; }
    static Button lastSelectedButton;
    void OnButtonClick(Type builtType, Button selectedButton)
    {
        if (BuildMode.hasEntered)
        {
            lastSelectedButton.GetComponent<Image>().color = Color.white;
            if (builtType == SelectedType)
            {
                //反选
                MouseAnimator.SetTrigger("BuildEnd");
                SelectedType = null;
                OnUpdate?.Invoke(null);
                

            }
            else
            {
                //另选
                EnterBuildMode();
            }
        }
        else
        {
            EnterBuildMode();
        }

        void EnterBuildMode()
        {
            selectedButton.GetComponent<Image>().color = Color.green;
            SelectedType = builtType;
            lastSelectedButton = selectedButton;
            MouseAnimator.Play("Build Mode");

            OnUpdate?.Invoke(SelectedType);
        }
    }

    public static int selectedDirection { get; private set; } = 2;

    public MouseIndicator mouseIndicator;

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            selectedDirection = (selectedDirection + 1) % 4;
            enteredSlotRender?.OnPointerEnter(null);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            selectedDirection = (selectedDirection - 1 + 4) % 4;
            enteredSlotRender?.OnPointerEnter(null);
        }


    }
    private void OnGUI()
    {
        //GUI.Label(new Rect(0, 0, 100, 100), selectedDirection.ToString());
    }

    public static void Foreach(Vector2 center, Vector2 size, Action<int, int> action)
    {
        Vector2 delta = size;
        //向右旋转的次数(按E的次数)
        for (int i = 0; i < selectedDirection; i++)
        {
            delta = delta.x * Vector2.down + delta.y * Vector2.right;
            //↓→
            //R = | 0  1 |
            //    | -1 0 |
        }

        for (int x = (int)center.x; x != center.x + delta.x; x += (int)Mathf.Sign(delta.x))
        {
            for (int y = (int)center.y; y != center.y + delta.y; y += (int)Mathf.Sign(delta.y))
            {
                action?.Invoke(x, y);
            }
        }
    }


    // Recommend
    public static bool TryGetSelectedTypeConfig(out Type type, out MapObjectDatabase.Config config)
    {
        if (SelectedType == null || BuildMode.hasEntered == false)
        {
            config = default;
            type = null;
            return false;
        }

        try
        {
            config = GameManager.current.MapObjectDatabase[SelectedType];
            type = SelectedType;
            Debug.Assert(config.Size.x > 0 && config.Size.y > 0, $"检测到Config.Size非法。在MapObject Database中为{SelectedType.Name}配置Size。");
            return true;
        }
        catch (KeyNotFoundException)
        {
            //没有配置Config

            config = new MapObjectDatabase.Config() { Size = Vector2Int.one };
            type = SelectedType;
            return true;
        }


    }


}
