using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [SerializeField]
    Button BtnExit;
    // Update is called once per frame
    private void OnEnable()
    {
        BtnExit.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
    private void OnDisable()
    {
        BtnExit?.onClick.RemoveListener(() =>
        {
            Application.Quit();
        });
    }
}
