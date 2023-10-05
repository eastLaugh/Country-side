using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{

    public const float 建筑时物体缓动持续时间 = 0.25f;
    public const float 相机初始高度 = 10f;

    public static readonly bool 开启浮动效果;

    public static float MasterVolume = 45;
    public static float MusicVolume = 35;
    public static float EffectVolume = 35;
    static Settings()
    {
        开启浮动效果 = false;
    }

    public static bool GridOn;
    public static void GetSettings()
    {
        MasterVolume = GameManager.globalData.MasterVolume;
        MusicVolume = GameManager.globalData.MusicVolume;
        EffectVolume = GameManager.globalData.EffectVolume;
        GridOn = GameManager.globalData.GridOn;
    }
    public static void ExportSettings()
    {
        GameManager.globalData.MasterVolume = MasterVolume;
        GameManager.globalData.MusicVolume = MusicVolume;
        GameManager.globalData.EffectVolume = EffectVolume;
    }


}