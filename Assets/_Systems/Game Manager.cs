using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
using Cinemachine;
using Unity.AI.Navigation;
using NaughtyAttributes;
using System.Collections;
using Unity.VisualScripting;





#if UNITY_EDITOR
using UnityEditor;
#endif
public class GameManager : MonoBehaviour
{
    public static event Action<Map> OnMapLoaded;
    public static event Action<Map> BeforeMapLoaded;
    public static GameManager current;
    public Grid grid;

    public static bool DebugMode { get; private set; } = true;
    public CinemachineVirtualCamera CinemachineVirtualCamera;
    public Transform PlaneIndicator;
    public static GlobalData globalData { get; private set; }


    public Vector2Int size;
    [Header("存储")]

    [NaughtyAttributes.ReadOnly]
    public static string SaveDirectory;

    [Header("数据库")]
    public MapObjectDatabase MapObjectDatabase;
    public SlotDatabase SlotDatabase;
    public illuBookData_SO illuBookData;

    #region  For Json.Net
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

    #endregion

    private void Start()
    {

    }

    #region 调试 debug
    SlotRender debugSlotRender = null;
    string debugSlotInfo = "此处显示你点击方块的调试信息。";
    void OnAnySlotClickedInAllMode(SlotRender slotRender)
    {
        if (debugSlotRender != null)
            debugSlotRender.slot.OnSlotUpdate -= UpdateDebugInfo;
        debugSlotRender = slotRender;
        debugSlotRender.slot.OnSlotUpdate += UpdateDebugInfo;
        UpdateDebugInfo();
        void UpdateDebugInfo()
        {
            debugSlotInfo = debugSlotRender.slot.GetInfo(true);
        }
    }

    #endregion

    private void OnEnable()
    {
        SlotRender.OnAnySlotClicked += OnAnySlotClickedInAllMode;
        SlotRender.OnAnySlotClickedInBuildMode += OnAnySlotClickedInAllMode;
        SlotRender.OnAnySlotEnter += OnAnySlotClickedInAllMode;  //触摸时实时更新调试信息
    }

    private void OnDisable()
    {
        SlotRender.OnAnySlotClicked -= OnAnySlotClickedInAllMode;
        SlotRender.OnAnySlotClickedInBuildMode -= OnAnySlotClickedInAllMode;
        SlotRender.OnAnySlotEnter -= OnAnySlotClickedInAllMode;

    }
    private void Update()
    {

    }

    private void Awake()
    {
        current = this;
        SaveDirectory = Path.Combine(Application.persistentDataPath, "beta");

        DG.Tweening.DOTween.Init();
        DG.Tweening.DOTween.SetTweensCapacity(size.x * size.y * 10, 50);

        LoadGlobalData();
    }
    private void LoadGlobalData()
    {
        if (File.Exists(Path.Combine(SaveDirectory, globalFileName)))
        {
            string jsonText = File.ReadAllText(Path.Combine(SaveDirectory, globalFileName));
            globalData = JsonConvert.DeserializeObject<GlobalData>(jsonText, SerializeSettings);
        }
        else
        {
            globalData = new GlobalData();
            SaveGlobalData();
        }
        Settings.GetSettings();

    }

    public static void SaveGlobalData()
    {
        Settings.ExportSettings();
        Directory.CreateDirectory(SaveDirectory);
        // if (File.Exists(Path.Combine(SaveDirectory, globalFileName)))
        //     File.Delete(Path.Combine(SaveDirectory, globalFileName));
        File.WriteAllText(Path.Combine(SaveDirectory, globalFileName), JsonConvert.SerializeObject(globalData, SerializeSettings));
    }

    int seed = -1;  // -1 : 由程序随机生成种子
    const string globalFileName = "GlobalSave.dat";
    // string autoFileName = "AutoSave.dat";
    string fileName = DefaultSaveName;
    const string DefaultSaveName = "[默认存档]";
    readonly GUILayoutOption[] textFieldLayout = new GUILayoutOption[] { GUILayout.Height(50) };
    readonly GUILayoutOption[] buttonLayout = new GUILayoutOption[] { GUILayout.Height(50) };
    private void OnGUI()
    {

        GUI.skin.textField.fontSize = 25;
        GUI.skin.button.fontSize = 25;
        GUI.skin.label.fontSize = 25;
        GUI.skin.toggle.fontSize = 25;

        DebugMode = GUILayout.Toggle(DebugMode, "开发者模式");
        if (!DebugMode) return;

        GUILayout.BeginHorizontal();
        fileName = GUILayout.TextField(fileName, textFieldLayout);
        if (GUILayout.Button("R", GUILayout.Width(40f)))
        {
            string[] adjectives = { "勇敢的", "聪慧的", "大胆的", "史诗的", "无畏的", "辉煌的", "英雄传的", "不可思议之", "强大的", "强力的" };
            string[] nouns = { "冒险", "探索", "追求", "旅程", "发现", "远征", "希腊史", "事业", "投机" };

            fileName = adjectives[UnityEngine.Random.Range(0, adjectives.Length)] + nouns[UnityEngine.Random.Range(0, nouns.Length)] + ".dat";
            seed = -1;
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (int.TryParse(GUILayout.TextField(seed.ToString(), textFieldLayout), out int newSeed))
        {
            seed = newSeed;
        }
        else
        {
            seed = -1;
        }
        if (GUILayout.Button("-1", GUILayout.Width(40f)))
        {
            seed = -1;
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("创建", buttonLayout))
        {
            UnLoad();
            var map = Map.Generate(size, seed);
            LoadMap(map);
            SaveCurrentMap(Path.Combine(SaveDirectory, fileName + ".dat"));
        }

        if (globalData?.GameSaveFiles != null)
        {
            GUILayout.BeginVertical("Box");
            foreach (var filePath in globalData.GameSaveFiles)
            {
                if (File.Exists(filePath))
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(Path.GetFileName(filePath), buttonLayout))
                    {
                        fileName = Path.GetFileName(filePath);
                        LoadFromLocalFile(filePath);
                    }
                    if (GUILayout.Button("x", GUILayout.Width(40f)))
                    {
                        globalData.GameSaveFiles.Remove(filePath);
                        //EventHandler.CallSavefileDeleted();
                        SaveGlobalData();
                        //以防万一暂时不真的删除源文件
                        break;

                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }
        if (GUILayout.Button("打开存储目录", buttonLayout))
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.OpenWithDefaultApp(SaveDirectory);
#endif
        }

        if (map == null)
        {
            GUILayout.Label("地图未创建。点击R自动填入随机地图名称和种子。点击-1讲种子设置为-1.", GUILayout.Width(200f));
        }
        else
        {
            if (GUILayout.Button("保存", buttonLayout))
            {
                SaveCurrentMap(Path.Combine(SaveDirectory, fileName));
            }
            if (GUILayout.Button("[调试|查看地图信息]", textFieldLayout))
            {
                SaveCurrentMap(Path.Combine(SaveDirectory, "临时.dat"), true);
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

        if (GUILayout.Button("卸载", buttonLayout))
        {
            UnLoad();
        }

        // if (map != null)
        // {
        //     //显示一些经济参数
        //     GUILayout.BeginArea(new Rect(Screen.width - 200, 0, 200, Screen.height), GUI.skin.box);
        //     {
        //         GUILayout.Label("参数");
        //         GUILayout.Label(JsonConvert.SerializeObject(currentEconomyVector, Formatting.Indented));

        //     }
        //     GUILayout.EndArea();
        // }
    }
    public Map map { get; private set; }
    public void SaveCurrentMap(string filePath, bool temp = false)
    {
        if (map == null)
        {
            InfoWindow.Create("请确保地图已加载");
            return;
        }

        //创建目录
        Directory.CreateDirectory(SaveDirectory);

        //序列化
        string mapData = JsonConvert.SerializeObject(map, SerializeSettings);
        File.WriteAllText(filePath, mapData);
#if UNITY_EDITOR
        UnityEditor.EditorUtility.OpenWithDefaultApp(filePath);
#endif

        if (!temp)
        {
            //记录在GlobalData中
            globalData.GameSaveFiles.Add(filePath);
            SaveGlobalData();
        }
    }
    public static event Action OnMapUnloaded;
    void UnLoad()
    {
        //if (map != null)
        //    map.economyWrapper.OnMiddlewareUpdated -= OnEconomyDataUpdated;
        grid.transform.DestroyAllChildren();
        map = null;
        OnMapUnloaded?.Invoke();

    }


    public void LoadFromLocalFile(string FileName)
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

    private void LoadMap(Map map)
    {

        this.map = map;
        seed = map.MainRandomSeed;

        //对齐到左下角
        CinemachineVirtualCamera.transform.position = new Vector3(map.size.x * grid.cellSize.x / 2f, CinemachineVirtualCamera.transform.position.y, map.size.y * grid.cellSize.z / 2f);

        PlaneIndicator.transform.position = new Vector3(map.size.x * grid.cellSize.x / 2f, PlaneIndicator.transform.position.y, map.size.y * grid.cellSize.z / 2f);
        //
        PlaneIndicator.transform.localScale = new Vector3(map.size.x / 10f, 1, map.size.y / 10f);

        OnMapLoaded?.Invoke(map);

        RefreshNavMesh();
    }

    Coroutine RefreshNavMeshCoroutine;
    [Button]
    public void RefreshNavMesh()
    {
        if (RefreshNavMeshCoroutine == null)
        {
            RefreshNavMeshCoroutine = StartCoroutine(WaitOneTick());
            IEnumerator WaitOneTick()
            {
                yield return null;
                NavMeshSurface navMeshSurface = PlaneIndicator.GetComponent<NavMeshSurface>();
                if (navMeshSurface)
                {
                    navMeshSurface.RemoveData();
                    navMeshSurface.BuildNavMesh();
                }

                RefreshNavMeshCoroutine = null;
            }
        }
    }

    public void NewGame(string fileName)
    {
        UnLoad();
        var File = Resources.Load<TextAsset>("Template");
        Map map = JsonConvert.DeserializeObject<Map>(File.text, SerializeSettings);
        LoadMap(map);
        SaveCurrentMap(Path.Combine(SaveDirectory, fileName + ".dat"));

    }
    public void AutoSave()
    {
        SaveCurrentMap(Path.Combine(SaveDirectory, fileName + ".dat"));
        UnLoad();

    }
}


