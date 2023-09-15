using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewGameMenu : MonoBehaviour
{
    
    [SerializeField] Button startBtn;
    [SerializeField] Button randomBtn;
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] TMP_InputField saveNameInput;
    [SerializeField] GameObject Overlay;
    void Start()
    {
        startBtn.onClick.AddListener(() =>
        {
            if(saveNameInput.text != "" && nameInput.text != "")
            {
                Overlay.SetActive(true);
                StartCoroutine(Creat(saveNameInput.text));
            }        
        });
        randomBtn.onClick.AddListener(() =>
        {
            string[] adjectives = { "深度的", "聪慧的", "大胆的", "史诗的", "无畏的", "辉煌的", "英雄的", "不可思议之", "强大的", "强力的", "有效的", "突破性的" };
            string[] nouns = { "革新", "探索", "追求", "旅程", "发现", "远征", "事业", "投机", "事迹" };
            saveNameInput.text = adjectives[Random.Range(0, adjectives.Length)] + nouns[Random.Range(0, nouns.Length)];
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator Creat(string fileName)
    {
        yield return null;
        //var fileName = Path.GetFileName(filePath);
        GameManager.current.NewGame(fileName);
        gameObject.SetActive(false);
        Overlay.SetActive(false);
        AudioSystem.current.SwitchMusic(SoundName.GameMusic);
        yield break;
    }
}
