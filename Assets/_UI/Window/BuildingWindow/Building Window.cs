using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private struct BuildingBtn
    {
        public Button btn;
        public int phase;
    }
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
    }
    SlotRender enteredSlotRender;
    private void OnAnySlotEnter(SlotRender render)
    {
        enteredSlotRender = render;
    }

    // Start is called before the first frame update
    void CheckCost(float money)
    {
        for (int i = 0; i < OptionButtons.Count; i++)
        {
            if (GameManager.current.map.Phase < OptionButtons[i].phase) continue;
            var pricetext = OptionButtons[i].btn.GetComponentsInChildren<TMPro.TextMeshProUGUI>()[1];
            if (money < float.Parse(pricetext.text.Split("万")[0]))
            {
                if (OptionButtons[i].btn == lastSelectedButton)
                {
                    lastSelectedButton.onClick?.Invoke();
                    lastSelectedButton = null;
                }
                pricetext.color = Color.red;
                OptionButtons[i].btn.interactable = false;
            }
            else if (pricetext.color == Color.red)
            {
                pricetext.color = Color.black;
                OptionButtons[i].btn.interactable = true;
            }
        }
    }
    void CheckPhase(int phase)
    {
        for (int i = 0; i < OptionButtons.Count; i++)
        {
            var pricetext = OptionButtons[i].btn.GetComponentsInChildren<TMPro.TextMeshProUGUI>()[1];
            if (GameManager.current.map.Phase < OptionButtons[i].phase)
                OptionButtons[i].btn.interactable = false;
            else if(pricetext.color!=Color.red)
                OptionButtons[i].btn.interactable = true;
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
    Button NewOption(string title, float cost,int phase, UnityAction<Button> callback)
    {
        GameObject buttonGameObject = Instantiate(ButtonPattern, Vector3.zero, Quaternion.identity, Content);
        Button button = buttonGameObject.GetComponent<Button>();
        TMPro.TextMeshProUGUI[] Texts = buttonGameObject.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        Texts[0].text = title;
        Texts[1].text = cost.ToString() + "万";
        if (GameManager.current.map.Phase < phase)
            button.interactable = false;
        buttonGameObject.SetActive(true);
        button.onClick.AddListener(() => callback(button));
        return button;
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
            Destroy(OptionButtons[i].btn.gameObject);
        }
        OptionButtons.Clear();
        foreach (Type mapObjectType in typeof(MapObjects).GetNestedTypes())
        {
            if (!mapObjectType.IsAbstract && typeof(IConstruction).IsAssignableFrom(mapObjectType))
            {
                //Debug.Log("entered");
                var ins = Activator.CreateInstance(mapObjectType) as IConstruction;
                if(ins.constructType == constructType && ins is not MapObjects.Administration)
                {                 

                    Button button = NewOption(ins.Name, ins.Cost,ins.phase, button =>
                    {
                        EventHandler.CallInitSoundEffect(SoundName.BtnClick1);
                        OnButtonClick(mapObjectType, button);
                    });
                    OptionButtons.Add(new BuildingBtn { btn = button,phase = ins.phase});
                }
#if UNITY_EDITOR
                if(ins.constructType == constructType && ins is MapObjects.Administration)
                {

                    Button button = NewOption(ins.Name, ins.Cost, ins.phase, button =>
                    {
                        EventHandler.CallInitSoundEffect(SoundName.BtnClick1);
                        OnButtonClick(mapObjectType, button);
                    });
                    OptionButtons.Add(new BuildingBtn { btn = button, phase = ins.phase });
                }
#endif
            }

        }
        //!!!!!!
        CheckCost(GameManager.current.map.MainData.Money);
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
