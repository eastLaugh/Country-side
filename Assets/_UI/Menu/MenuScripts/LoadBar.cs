using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadBar : MonoBehaviour
{
    [SerializeField]
    Button LoadBtn, DeleteBtn;
    [SerializeField]
    TextMeshProUGUI nameText;
    [SerializeField]
    LoadMenu loadMenu;
    [SerializeField]
    GameObject Overlay;
    GameManager gameManager => GameManager.current;

    public void UpdateBar(string filePath)
    {
        nameText.text = Path.GetFileName(filePath).Split('.')[0];
        LoadBtn.onClick.AddListener(() =>
        {
            EventHandler.CallInitSoundEffect(SoundName.BtnClick1);
            Overlay.SetActive(true);
            StartCoroutine(Load(filePath));
        });
        DeleteBtn.onClick.AddListener(() =>
        {
            EventHandler.CallInitSoundEffect(SoundName.BtnClick1);
            GameManager.globalData.GameSaveFiles.Remove(filePath);
            GameManager.SaveGlobalData();
            EventHandler.CallSavefileDeleted();
        });
    }

    private IEnumerator Load(string filePath)
    {
        yield return null;
        //var fileName = Path.GetFileName(filePath);
        gameManager.LoadFromLocalFile(filePath);
        
        Overlay.SetActive(false);
        loadMenu.gameObject.SetActive(false);
        AudioSystem.current.SwitchMusic(SoundName.GameMusic);
        yield break;
    }
}
