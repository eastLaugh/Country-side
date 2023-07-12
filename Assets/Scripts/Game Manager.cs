using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Runtime.Serialization;

public class GameManager : MonoBehaviour
{
    [Header("引用")]
    public static GameManager one;
    public Grid grid;
    public GameObject PlainPrefab;


    public Vector2Int size;
    [Header("存储")]

    [ReadOnly]
    public string SaveDirectory;
    [ReadOnly]
    public string FileName;

    public static JsonSerializerSettings SerializeSettings = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        TypeNameHandling = TypeNameHandling.Auto
    };

    private void Awake()
    {
        one = this;
        SaveDirectory = Path.Combine(Application.persistentDataPath, "beta");
        FileName = Path.Combine(SaveDirectory, "GameSave1.zmq");
    }
    private void OnGUI()
    {
        if (GUILayout.Button("生成地图并保存在本地"))
        {
            //生成地图
            var map = GenerateMap(size);

            //序列化
            string gameData = JsonConvert.SerializeObject(map, SerializeSettings);

            //本地化
            Directory.CreateDirectory(SaveDirectory);
            File.WriteAllText(FileName, gameData);
            UnityEditor.EditorUtility.OpenWithDefaultApp(FileName);

        }
        if (GUILayout.Button("加载本地地图"))
        {
            UnLoad();
            LoadFile();
        }

    }

    void UnLoad()
    {
        for (int i = grid.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(grid.transform.GetChild(i).gameObject);
        }
    }
    Map GenerateMap(Vector2Int size)
    {
        var slots = new Slot[100];
        var map = new Map(size, slots);
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                slots[i * size.y + j] = new Plain(map, new Vector2(i, j));
            }
        }
        return map;
    }

    void LoadFile()
    {
        if (!File.Exists(FileName))
        {
            Debug.LogError(FileName + " not exist .");
        }
        else
        {
            string jsonText = File.ReadAllText(FileName);
            Map map = JsonConvert.DeserializeObject<Map>(jsonText, SerializeSettings);
            LoadMap(map);
        }
    }

    public static event Action<Map> OnLoadMap;
    public static Map CurrentMap { get; private set; }
    void LoadMap(Map map)
    {
        OnLoadMap?.Invoke(map);
        CurrentMap = map;
    }
}

public class Map
{
    [JsonProperty]
    private Slot[] Slots;
    [JsonProperty]
    public Vector2Int size { get; private set; }
    public Map(Vector2Int size, Slot[] Slots)
    {
        this.size = size;
        this.Slots = Slots;
    }

    public Map()
    {

    }
    public Slot this[Vector2 pos] => this[(int)pos.x, (int)pos.y];
    public Slot this[int x, int y] => Slots[x * size.y + y];
}

