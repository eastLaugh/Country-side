using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Runtime.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif
using NaughtyAttributes;
public class GameManager : MonoBehaviour
{
    public static event Action<Map> OnMapLoaded;
    public static GameManager current;

    public Grid grid;


    public Vector2Int size;
    [Header("存储")]

    [NaughtyAttributes.ReadOnly]
    public string SaveDirectory;

    [NaughtyAttributes.Foldout("数据库")]
    public MapObjectDatabase MapObjectDatabase;
    public SlotDatabase SlotDatabase;

    public static JsonSerializerSettings SerializeSettings = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        TypeNameHandling = TypeNameHandling.Auto
    };

    private void Start()
    {
        UnLoad();
        //InfoWindow.Create("这是一个窗口，点击右下角关闭");

        // var map = GenerateMap(size);
        // LoadMap(map);
    }

    private void Awake()
    {
        DG.Tweening.DOTween.Init();
        DG.Tweening.DOTween.SetTweensCapacity(size.x * size.y, 50);

        current = this;
        SaveDirectory = Path.Combine(Application.persistentDataPath, "beta");
    }
    int seed = -1;
    private void OnGUI()
    {
        // if (GUILayout.Button("-1将自动生成"))
        // {
        //     seed = -1;
        // }
        if (int.TryParse(GUILayout.TextField(seed.ToString()), out int newSeed))
        {
            seed = newSeed;
        }
        else
        {
            seed = -1;
        }
        if (seed != -1)
        {
            if (GUILayout.Button("重新创建"))
            {
                seed = -1;
                var map = GenerateMap(size);
                LoadMap(map);
            }
        }
        if (GUILayout.Button("创建"))
        {
            var map = GenerateMap(size);
            LoadMap(map);
        }
        if (GUILayout.Button("加载"))
        {
            var map = GenerateFromLocalFile(Path.Combine(SaveDirectory, "GameSave1.zmq"));
            LoadMap(map);
        }


        if (map == null)
        {
            GUILayout.Label("地图未存在");
        }
        else
        {
            if (GUILayout.Button("保存"))
            {
                SaveCurrentMap(Path.Combine(SaveDirectory, "GameSave1.zmq"));
            }
            if (GUILayout.Button("[调试]"))
            {
                SaveCurrentMap(Path.Combine(SaveDirectory, "调试.zmq"));
            }
        }

    }
    public Map map { get; private set; }
    void SaveCurrentMap(string fileName)
    {
        if (map == null)
        {
            InfoWindow.Create("请确保地图已加载");
            return;
        }
        //序列化
        string gameData = JsonConvert.SerializeObject(map, SerializeSettings);

        //本地化
        Directory.CreateDirectory(SaveDirectory);
        File.WriteAllText(fileName, gameData);
#if UNITY_EDITOR
        UnityEditor.EditorUtility.OpenWithDefaultApp(fileName);
#endif
    }

    void UnLoad()
    {
        grid.transform.DestroyAllChild();
        map = null;
    }
    Map GenerateMap(Vector2Int size)
    {
        UnLoad();
        return Map.Generate(size, seed);
    }

    Map GenerateFromLocalFile(string FileName)
    {
        if (!File.Exists(FileName))
        {
            throw new FileNotFoundException(FileName + " not exist .");
        }
        else
        {
            UnLoad();
            string jsonText = File.ReadAllText(FileName);
            Map map = JsonConvert.DeserializeObject<Map>(jsonText, SerializeSettings);
            return map;
        }
    }

    public void LoadMap(Map map)
    {
        this.map = map;
        OnMapLoaded?.Invoke(map);
        seed = map.MainRandomSeed;
        grid.transform.position = new Vector3(-map.size.x * grid.cellSize.x / 2f, 0, /*-map.size.y * grid.cellSize.z / 2f*/0);

    }

}


