using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LakeGenerator : MonoBehaviour
{

    public static LakeGenerator generator { get; private set; }
    private void Awake()
    {
        generator = this;
    }
    private void OnEnable()
    {
        GameManager.OnMapLoaded += OnMapLoaded;
    }

    private void OnMapLoaded(Map map)
    {
        
    }

    private void OnDisable()
    {
        GameManager.OnMapLoaded -= OnMapLoaded;
    }
}
