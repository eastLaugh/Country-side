using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [SerializeField]
    Button BtnExit;
    // Update is called once per frame
    private static bool firstStart = false;
    [SerializeField]
    GameObject PoleCat;
    private void OnEnable()
    {
        if (!firstStart)
        {
            PoleCat.SetActive(true);
            firstStart = true;
        }      
        BtnExit.onClick.AddListener(() =>
        {
            GameManager.SaveGlobalData();
            Application.Quit();
        });
    }
    private void OnDisable()
    {
        BtnExit?.onClick.RemoveListener(() =>
        {
            GameManager.SaveGlobalData();
            Application.Quit();
        });
    }
}
