using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Runtime.Serialization;
using Cinemachine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using NaughtyAttributes;
public class GameManager : MonoBehaviour
{
    public static event Action<Map> OnMapLoaded;
    public static event Action<Map> AfterMapLoaded;
    public static GameManager current;

    public Grid grid;


    public Vector2Int size;
    [Header("存储")]

    [NaughtyAttributes.ReadOnly]
    public string SaveDirectory;

    [NaughtyAttributes.Foldout("数据库")]
    public MapObjectDatabase MapObjectDatabase;
    public SlotDatabase SlotDatabase;

    public static readonly JsonSerializerSettings SerializeSettings = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        TypeNameHandling = TypeNameHandling.Auto
    };

    public static readonly JsonSerializerSettings DebugSerializeSettings = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        TypeNameHandling = TypeNameHandling.Auto,
        ContractResolver = new MapPropertyIgnore()
    };

    private void Start()
    {
        UnLoad();
        //InfoWindow.Create("这是一个窗口，点击右下角关闭");

        // var map = GenerateMap(size);
        // LoadMap(map);
    }

    SlotRender debugSlotRender = null;
    string debugSlotInfo = "此处显示你点击方块的调试信息。";
    private void OnEnable()
    {
        SlotRender.OnAnySlotClicked += OnAnySlotClickedInAllMode;
        SlotRender.OnAnySlotClickedInBuildMode += OnAnySlotClickedInAllMode;


    }


    private void OnDisable()
    {
        SlotRender.OnAnySlotClicked -= OnAnySlotClickedInAllMode;
        SlotRender.OnAnySlotClickedInBuildMode -= OnAnySlotClickedInAllMode;


    }
    void OnAnySlotClickedInAllMode(SlotRender slotRender)
    {
        if (debugSlotRender != null)
            debugSlotRender.slot.OnSlotUpdate -= UpdateDebugInfo;
        debugSlotRender = slotRender;
        debugSlotRender.slot.OnSlotUpdate += UpdateDebugInfo;
        void UpdateDebugInfo()
        {
            debugSlotInfo = debugSlotRender.slot.GetInfo(true);
        }
    }


    private void Awake()
    {
        DG.Tweening.DOTween.Init();
        DG.Tweening.DOTween.SetTweensCapacity(size.x * size.y, 50);

        current = this;
        SaveDirectory = Path.Combine(Application.persistentDataPath, "beta");
    }
    int seed = -1;  // -1 : 由程序随机生成种子
    string fileName = "默认存档.zmq";
    private void OnGUI()
    {

        fileName = GUILayout.TextField(fileName);

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
            if (GUILayout.Button("新地图"))
            {
                seed = -1;
                UnLoad();
                var map = Map.Generate(size, seed);
                LoadMap(map);
            }
        }
        if (GUILayout.Button("创建"))
        {
            UnLoad();
            var map = Map.Generate(size, seed);
            LoadMap(map);

        }
        if (GUILayout.Button("加载"))
        {
            GenerateFromLocalFile(Path.Combine(SaveDirectory, fileName));

        }


        if (map == null)
        {
            GUILayout.Label("地图未存在");
        }
        else
        {
            if (GUILayout.Button("保存"))
            {
                SaveCurrentMap(Path.Combine(SaveDirectory, fileName));
            }
            if (GUILayout.Button("[调试|查看地图信息]"))
            {
                SaveCurrentMap(Path.Combine(SaveDirectory, "临时非存档.zmq"));
            }
        }


        GUILayout.Label(/*JsonConvert.SerializeObject(debugSlotRender.slot, DebugSerializeSettings*/ debugSlotInfo, new GUIStyle(GUI.skin.label)
        {
            fontSize = 30,
            normal = new GUIStyleState()
            {
                textColor = Color.black
            }
        });

        if (debugSlotRender != null)
        {

            if (GUILayout.Button("重新渲染"))
            {
                debugSlotRender.Refresh();
            }
        }

        if (map != null)
        {
            //显示一些经济参数
            GUILayout.BeginArea(new Rect(Screen.width - 200, 0, 200, Screen.height), GUI.skin.box);
            {
                GUILayout.Label("经济参数");
                GUILayout.Label(JsonConvert.SerializeObject(currentEconomyVector, Formatting.Indented));

            }
            GUILayout.EndArea();
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
        if (map != null)
            map.economyWrapper.OnDataUpdated -= OnEconomyDataUpdated;
        grid.transform.DestroyAllChild();
        map = null;
    }


    void GenerateFromLocalFile(string FileName)
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
            LoadMap(map);
        }
    }

    public CinemachineVirtualCamera CinemachineVirtualCamera;
    private void LoadMap(Map map)
    {
        OnMapLoaded?.Invoke(map);

        this.map = map;
        seed = map.MainRandomSeed;
        
        //grid.transform.position = new Vector3(-map.size.x * grid.cellSize.x / 2f, 0, /*-map.size.y * grid.cellSize.z / 2f*/0); //对齐到左下角
        CinemachineVirtualCamera.transform.position = new Vector3(map.size.x * grid.cellSize.x / 2f, CinemachineVirtualCamera.transform.position.y, map.size.y * grid.cellSize.z / 2f);

        map.economyWrapper.OnDataUpdated += OnEconomyDataUpdated;
        OnEconomyDataUpdated(map.economyWrapper.GetValue());

        AfterMapLoaded?.Invoke(map);

    }

    EconomyVector currentEconomyVector;
    private void OnEconomyDataUpdated(EconomyVector _new)
    {
        currentEconomyVector = _new;
    }
}


