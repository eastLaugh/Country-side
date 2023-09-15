using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BuildingWindow : MonoBehaviour
{
    public static event Action<Type> OnUpdate;
    public RectTransform Content;
    public GameObject ButtonPattern;
    [SerializeField] Button btnRemove;
    List<Button> OptionButtons = new List<Button>();

    private void OnEnable()
    {
        SlotRender.OnAnySlotEnter += OnAnySlotEnter;
        EventHandler.MoneyUpdate += CheckCost;
    }

    private void OnDisable()
    {
        SlotRender.OnAnySlotEnter -= OnAnySlotEnter;
        EventHandler.MoneyUpdate += CheckCost;
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
            var pricetext = OptionButtons[i].GetComponentsInChildren<TMPro.TextMeshProUGUI>()[1];
            if (money < float.Parse(pricetext.text.Split("万")[0]))
            {
                if (OptionButtons[i] == lastSelectedButton)
                {
                    lastSelectedButton.onClick?.Invoke();
                    lastSelectedButton = null;
                }
                pricetext.color = Color.red;
                OptionButtons[i].interactable = false;
            }
            else if (pricetext.color == Color.red)
            {
                pricetext.color = Color.black;
                OptionButtons[i].interactable = true;
            }
        }
    }
    void Start()
    {

        OptionButtons.Clear();
        foreach (Type mapObjectType in typeof(MapObjects).GetNestedTypes())
        {
            if (!mapObjectType.IsAbstract && typeof(IConstruction).IsAssignableFrom(mapObjectType))
            {
                //Debug.Log("entered");
                var ins = Activator.CreateInstance(mapObjectType) as IConstruction;
                Button button = NewOption(ins.Name, ins.Cost, button =>
                {
                    OnButtonClick(mapObjectType, button);
                });
                OptionButtons.Add(button);

            }

        }
        btnRemove.onClick.AddListener(() =>
        {
            OnButtonClick(typeof(MapObjects.DeletingFlag), btnRemove);
        });
        ButtonPattern.gameObject.SetActive(false);

        Button NewOption(string title, float cost, UnityAction<Button> callback)
        {
            GameObject buttonGameObject = Instantiate(ButtonPattern, Vector3.zero, Quaternion.identity, Content);
            Button button = buttonGameObject.GetComponent<Button>();
            TMPro.TextMeshProUGUI[] Texts = buttonGameObject.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
            Texts[0].text = title;
            Texts[1].text = cost.ToString() + "万";
            buttonGameObject.SetActive(true);
            button.onClick.AddListener(() => callback(button));
            return button;
        }
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
        GUI.Label(new Rect(0, 0, 100, 100), selectedDirection.ToString());
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
