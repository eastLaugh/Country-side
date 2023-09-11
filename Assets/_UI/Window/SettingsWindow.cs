using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsWindow : MonoBehaviour
{
    [SerializeField] Button BtnClose;
    [SerializeField] TimeController timeController;
    [SerializeField] GameObject window;
    [SerializeField] Toggle gridOn;
    [SerializeField] GameObject Grid;
    void Start()
    {
        BtnClose.onClick.AddListener(()=>{
            timeController.Continue();
            window.SetActive(false);
        });
        gridOn.isOn = true;
        gridOn.onValueChanged.AddListener((value) =>
        {
            Grid.SetActive(value);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
