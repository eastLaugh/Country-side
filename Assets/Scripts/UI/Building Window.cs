using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingWindow : MonoBehaviour
{
    public RectTransform Content;
    public GameObject ButtonPattern;

    // Start is called before the first frame update
    void Start()
    {

        foreach (Type mapObjectType in Slot.MapObject.BuiltMapObject)
        {
            GameObject buttonGameObject = Instantiate(ButtonPattern, Vector3.zero, Quaternion.identity, Content);
            Button button = buttonGameObject.GetComponent<Button>();
            buttonGameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText(mapObjectType.Name);

            button.onClick.AddListener(() => OnButtonClick(mapObjectType));
        }

        ButtonPattern.gameObject.SetActive(false);
    }

    public Animator MouseAnimator;
    public static Type SelectiveType{get;private set;}
    void OnButtonClick(Type builtType)
    {
        SelectiveType = builtType;
        // MouseAnimator.SetTrigger("Build");
        MouseAnimator.Play("Build Mode");
    }
}
