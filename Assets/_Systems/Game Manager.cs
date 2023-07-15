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

    private void Start()
    {
        InfoWindow.Create("这是一个窗口，点击右下角关闭");
    }

    private void Awake()
    {
        one = this;
        SaveDirectory = Path.Combine(Application.persistentDataPath, "beta");
        FileName = Path.Combine(SaveDirectory, "GameSave1.zmq");
    }
    private void OnGUI()
    {
        if (GUILayout.Button("生成新地图"))
        {
            map = GenerateMap(size);
        }
        if (GUILayout.Button("生成新地图并保存"))
        {
            map = GenerateMap(size);
            SaveLocalFile();
        }
        if (GUILayout.Button("加载已保存地图"))
        {
            LoadFile();
        }
        if (GUILayout.Button("保存本地地图"))
        {
            SaveLocalFile();
        }
        if (GUILayout.Button("关闭地图"))
        {
            UnLoad();
        }

        if (map == null)
        {
            GUILayout.Label("地图未存在");
        }

    }
    public Map map { get; private set; }
    void SaveLocalFile()
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
        File.WriteAllText(FileName, gameData);
        UnityEditor.EditorUtility.OpenWithDefaultApp(FileName);
    }

    void UnLoad()
    {

        for (int i = grid.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(grid.transform.GetChild(i).gameObject);
        }
        map = null;
    }
    Map GenerateMap(Vector2Int size)
    {
        UnLoad();
        var slots = new Slot[100];
        var map = new Map(size, slots);
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                slots[i * size.y + j] = new Plain(map, new Vector2(i, j),new());
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
            UnLoad();
            string jsonText = File.ReadAllText(FileName);
            Map map = JsonConvert.DeserializeObject<Map>(jsonText, SerializeSettings);
            LoadMap(map);
            this.map = map;
        }
    }

    public static event Action<Map> OnLoadMap;
    void LoadMap(Map map)
    {
        OnLoadMap?.Invoke(map);

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

