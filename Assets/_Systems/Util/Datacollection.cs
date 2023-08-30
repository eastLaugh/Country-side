
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class BuildingDetails
{
    public int ID;
    public string name;
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
    public GlobalData()
    {
     //unlockIlluBookName.Add(string.Empty);
    }
}