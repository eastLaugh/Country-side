using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameViewBtn : MonoBehaviour
{
    [SerializeField] Button settingInGameBtn;
    [SerializeField] Button homeBtn;
    [SerializeField] TimeController timeController;
    [SerializeField] GameObject returnWindow;
    [SerializeField] GameObject settingWindow;
    void Start()
    {
        settingInGameBtn.onClick.AddListener(() =>
        {
            settingWindow.SetActive(true);
            timeController.Pause();
        });
        homeBtn.onClick.AddListener(() =>
        {
            returnWindow.SetActive(true);
            timeController.Pause();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
