using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using DG.Tweening;
using AYellowpaper.SerializedCollections;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class IconPattern : MonoBehaviour
{
    static IconPattern Ovary = null;

    [SerializedDictionary("Icon Identify", "Prefab")]
    public SerializedDictionary<string, GameObject> Prefabs;
    private void Awake()
    {
        if (Ovary == null)
        {
            Ovary = this;
        }
    }

    public static IconPattern Create(Transform parent, Vector3 position)
    {
        var obj = Instantiate(Ovary.gameObject, parent);
        obj.transform.localPosition = position;
        IconPattern iconPattern = obj.GetComponent<IconPattern>();
        return iconPattern;

    }

    // Start is called before the first frame update
    void OnEnable()
    {
        InputForCamera.OnCameraInput += OnCameraZoom;
    }


    void OnDisable() {
        InputForCamera.OnCameraInput -= OnCameraZoom;
    }


    private void OnCameraZoom(CinemachineVirtualCamera camera)
    {
        transform.DOLookAt(camera.transform.position, 0.15f).SetDelay(0.25f).SetEase(Ease.OutBack);

    }

    public void New(string key)
    {
        if (Prefabs.TryGetValue(key, out GameObject prefab))
        {
            GameObject instance = Instantiate(prefab, prefab.transform.parent);
            instance.SetActive(true);
        }
        else
        {
            Debug.LogError("Icon Pattern: No such Prefab");
        }

    }

}
