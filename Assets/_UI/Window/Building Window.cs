using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BuildingWindow : MonoBehaviour
{
    public RectTransform Content;
    public GameObject ButtonPattern;

    List<Button> OptionButtons = new List<Button>();

    private void OnEnable() {
        SlotRender.OnAnySlotEnter += OnAnySlotEnter;
    }

    private void OnDisable() {
        SlotRender.OnAnySlotEnter -= OnAnySlotEnter;
    }

    SlotRender enteredSlotRender;
    private void OnAnySlotEnter(SlotRender render)
    {
        enteredSlotRender = render;
    }

    // Start is called before the first frame update
    void Start()
    {

        OptionButtons.Clear();
        foreach (Type mapObjectType in typeof(MapObjects).GetNestedTypes())
        {
            Button button = NewOption(mapObjectType.Name, button =>
            {
                OnButtonClick(mapObjectType, button);
            });
            OptionButtons.Add(button);
        }



        ButtonPattern.gameObject.SetActive(false);

        Button NewOption(string title, UnityAction<Button> callback)
        {
            GameObject buttonGameObject = Instantiate(ButtonPattern, Vector3.zero, Quaternion.identity, Content);
            Button button = buttonGameObject.GetComponent<Button>();
            buttonGameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText(title);

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
