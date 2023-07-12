using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingWindow : MonoBehaviour
{
    public RectTransform Content;
    public GameObject ButtonPattern;
    public static readonly Type[] BuiltMapObject = new Type[] { typeof(House) ,typeof(Road)};
    // Start is called before the first frame update
    void Start()
    {

        foreach (Type mapObjectType in BuiltMapObject)
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
        MouseAnimator.SetTrigger("Build");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
