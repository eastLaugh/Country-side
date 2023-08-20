using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InfoWindow.Create("点击左上角开始游戏");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 100), "Start"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Map Scene");
        }
    }
}
