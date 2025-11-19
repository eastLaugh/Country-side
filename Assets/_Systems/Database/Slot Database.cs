using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;


[CreateAssetMenu(fileName = "Slot Database", menuName = "Country side/Database/Slot", order = 0)]
public class SlotDatabase : ScriptableObject
{
    public static SlotDatabase main;
    
    public void OnEnable()
    {
        main = this;
    }

    [SerializeField]
    [SerializedDictionary("Slot", "Config")]
    SerializedDictionary<Element, Config> Dict;

    [System.Serializable]
    public struct Element
    {
        [TypeToString(typeof(Slots))]
        public string _;
    }
    [System.Serializable]
    public struct Config
    {
        [NaughtyAttributes.ShowAssetPreview]
        public GameObject Prefab;

        public string name;
    }

    public Config this[Type type]{
        get{
            return Dict[new Element{_=type.Name}];
        }
    }
}