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
                return "����ָ��";
            case 2:
                return "������ʩ����";
            case 3:
                return "���ֻ�����";
            case 4:
                return "��̬�Ļ�����";
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
