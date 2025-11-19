
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class BuildingDetails
{
    public string name;
    public string chineseName;
    public Sprite icon;
    public string description;
    public string function;
    public string requirement;
    public BuildingType buildingType;
    public int price;
    public bool unclock = false;
}

public class GlobalData
{
    [JsonProperty]
    public List<string> unlockIlluBookName = new List<string>();

    //记录存档列表
    [JsonProperty]
    public HashSet<string> GameSaveFiles = new();
    [JsonProperty]
    public float MasterVolume = 45;
    [JsonProperty]
    public float MusicVolume = 35;
    [JsonProperty]
    public float EffectVolume = 35;
    [JsonProperty]
    public bool GridOn = true;

    public GlobalData()
    {
        //unlockIlluBookName.Add(string.Empty);
    }
}