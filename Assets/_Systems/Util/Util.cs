using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    public static void DestroyAllChild(this Transform transform)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            MonoBehaviour.Destroy(transform.GetChild(i).gameObject);
        }
    }
}
