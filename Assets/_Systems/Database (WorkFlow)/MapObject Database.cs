using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;


[CreateAssetMenu(fileName = "MapObject Database", menuName = "Country side/Database/MapObject", order = 0)]
public class MapObjectDatabase : ScriptableObject
{
    public static MapObjectDatabase main;
    
    public void OnEnable()
    {
        main = this;
    }

    [SerializeField]
    [SerializedDictionary("Map Object", "Config")]
    SerializedDictionary<Element, Config> Dict;

    [System.Serializable]
    public struct Element
    {
        [TypeToString(typeof(Slot.MapObject))]
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

    public Config this[Type type]{
        get{
            return Dict[new Element{_=type.Name}];
        }
    }
}