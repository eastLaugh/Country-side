using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    public static void DestroyAllChildren(this Transform transform)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            MonoBehaviour.Destroy(transform.GetChild(i).gameObject);
        }
    }
    public static string Phase(int phase)
    {
        switch (phase)
        {
            case 1:
                return "新手指引";
            case 2:
                return "基础设施建设";
            case 3:
                return "数字化建设";
            case 4:
                return "生态文化建设";
            default:
                return "";
        }
    }
    public static IEnumerator Wait(float sec,Action action)
    {
        yield return new WaitForSeconds(sec);
        action.Invoke();
    }
}
