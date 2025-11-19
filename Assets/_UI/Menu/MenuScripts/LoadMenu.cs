using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadMenu : MonoBehaviour
{
    public Transform RuleItemRoot;
    public LoadBar LoadBarPrefab;
   
    private void OnEnable()
    {
        RefreshUI();
        EventHandler.SavefileDeleted += RefreshUI;
    }
    private void OnDisable()
    {
        EventHandler.SavefileDeleted -= RefreshUI;
    }
    private void RefreshUI()
    {
        for (int i = 0; i < RuleItemRoot.childCount; i++)
        {
            Destroy(RuleItemRoot.GetChild(i).gameObject);
        }
        foreach (var filePath in GameManager.globalData.GameSaveFiles)
        {
            if (File.Exists(filePath))
            {
                var bar = Instantiate(LoadBarPrefab, RuleItemRoot);
                bar.UpdateBar(filePath);
                bar.gameObject.SetActive(true);
            }
        }
    }
}
