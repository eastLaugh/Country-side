using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;


[CreateAssetMenu(fileName = "MapObjectDatabase", menuName = "Country side/Database/MapObject", order = 0)]
public class MapObjectDatabase : ScriptableObject
{
    public static MapObjectDatabase main;
    
    public void OnEnable()
    {
        main = this;
    }


    [SerializedDictionary("Map Object", "Config")]
    public SerializedDictionary<Element, Config> ElementDescriptions;

    [System.Serializable]
    public struct Element
    {
        [MapObject]
        public string _;
    }
    [System.Serializable]
    public struct Config
    {
        [NaughtyAttributes.ShowAssetPreview]
        public GameObject Prefab;

        [NaughtyAttributes.ResizableTextArea]
        public string 备注;
    }
}