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
    [NaughtyAttributes.ReadOnly]
    public string FileName;

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

        var map = GenerateMap(size);
        LoadMap(map);
    }

    private void Awake()
    {
        DG.Tweening.DOTween.Init();
        DG.Tweening.DOTween.SetTweensCapacity(size.x * size.y, 50);

        current = this;
        SaveDirectory = Path.Combine(Application.persistentDataPath, "beta");
        FileName = Path.Combine(SaveDirectory, "GameSave1.zmq");
    }
    int seed = -1;
    private void OnGUI()
    {
        if (GUILayout.Button("-1将自动生成"))
        {
            seed = -1;
        }
        if (int.TryParse(GUILayout.TextField(seed.ToString()), out int newSeed))
        {
            seed = newSeed;
        }
        else
        {
            seed = -1;
        }
        if (GUILayout.Button("创建"))
        {
            var map = GenerateMap(size);
            LoadMap(map);
        }
        // if (GUILayout.Button("生成新地图并保存"))
        // {
        //     var map = GenerateMap(size);
        //     LoadMap(map);
        //     SaveCurrentMap();
        // }
        if (GUILayout.Button("加载"))
        {
            var map = GenerateFromLocalFile();
            LoadMap(map);
        }
        if (GUILayout.Button("保存"))
        {
            SaveCurrentMap();
        }
        // if (GUILayout.Button("关闭"))
        // {
        //     UnLoad();
        // }

        if (map == null)
        {
            GUILayout.Label("地图未存在");
        }

    }
    public Map map { get; private set; }
    void SaveCurrentMap()
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
#if UNITY_EDITOR
        UnityEditor.EditorUtility.OpenWithDefaultApp(FileName);
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
        var slots = new Slot[size.x * size.y];
        if (seed == -1)
        {
            seed = UnityEngine.Random.Range(0, int.MaxValue);
        }
        var map = new Map(size, slots, seed);
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                var newSlot = new Plain(map, new Vector2(i, j), new());

                if (UnityEngine.Random.Range(0, 100) < 10)
                {
                    new Tree(-1).Inject(newSlot);
                }

                slots[i * size.y + j] = newSlot;
            }
        }
        return map;
    }

    Map GenerateFromLocalFile()
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


