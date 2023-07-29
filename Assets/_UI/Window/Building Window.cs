using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BuildingWindow : MonoBehaviour
{
    public RectTransform Content;
    public GameObject ButtonPattern;

    List<Button> OptionButtons = new List<Button>();
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
    public static Type SelectedType { get; private set; }
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
}
