using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
using Cinemachine;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class GameManager : MonoBehaviour
{
    public static event Action<Map> OnMapLoaded;
    public static event Action<Map> AfterMapLoaded;
    public static event Action OnGameUpdate;
    public static event Action OnGameExit;
    public static GameManager current;
    public static int saveIndex = 0;

    public Grid grid;

    [SerializeField]
    public string CurrentSate;
    [SerializeField] GameObject Overlay;
    [SerializeField] UIManager uiManager;

    public CinemachineVirtualCamera CinemachineVirtualCamera;
    public RoadRenderer roadRenderer;
    private GlobalData globalData;
    private TimeSystem timeSystem;
    private illuBookSystem illuBookSystem;


    [Obsolete]
    public enum GameState
    {
        Unload, NewGame, Loading, Playing
    }

    [Obsolete]
    FSM<GameState> fsm = new FSM<GameState>();

    public Vector2Int size;
    [Header("存储")]

    [NaughtyAttributes.ReadOnly]
    public string SaveDirectory;

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
        FSMInit();
    }
    void FSMInit()
    {
        fsm.State(GameState.Loading)
        .OnEnter(() =>
        {
            Overlay.SetActive(true);
        }).OnExit(() =>
        {
            Overlay.SetActive(false);
        });
        fsm.State(GameState.Playing)
        .OnUpdate(() =>
        {
            OnGameUpdate?.Invoke();
        });
        fsm.State(GameState.Unload);
        fsm.ChangeState(GameState.Unload);
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
        void UpdateDebugInfo()
        {
            debugSlotInfo = debugSlotRender.slot.GetInfo(true);
        }
    }

    EconomyVector currentEconomyVector;

    private void OnEconomyDataUpdated(EconomyVector _new)
    {
        currentEconomyVector = _new;
    }

    #endregion

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
    private void Update()
    {
        fsm.update();
    }

    private void Awake()
    {
        current = this;
        SaveDirectory = Path.Combine(Application.persistentDataPath, "beta");

        DG.Tweening.DOTween.Init();
        DG.Tweening.DOTween.SetTweensCapacity(size.x * size.y, 50);

        LoadGlobalData();
        TimeInit();
        IlluBookInit();
        UIInit();

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
    }
    private void SaveGlobalData()
    {
        Directory.CreateDirectory(SaveDirectory);
        // if (File.Exists(Path.Combine(SaveDirectory, globalFileName)))
        //     File.Delete(Path.Combine(SaveDirectory, globalFileName));
        File.WriteAllText(Path.Combine(SaveDirectory, globalFileName), JsonConvert.SerializeObject(globalData, SerializeSettings));
    }
    #region SystemInit

    [Obsolete]
    private void TimeInit()
    {
        timeSystem = new TimeSystem();
    }
    private void IlluBookInit()
    {
        //Debug.Log(globalData.unlockIlluBookName);
        illuBookSystem = new illuBookSystem(globalData);

    }
    private void UIInit()
    {
        uiManager.Initialize(timeSystem, illuBookSystem);
    }

    #endregion

    int seed = -1;  // -1 : 由程序随机生成种子
    const string globalFileName = "GlobalSave.dat";
    // string autoFileName = "AutoSave.dat";
    string fileName = DefaultSaveName;
    const string DefaultSaveName = "[默认存档].dat";
    readonly GUILayoutOption[] textFieldLayout = new GUILayoutOption[] { GUILayout.Width(400), GUILayout.Height(50) };
    readonly GUILayoutOption[] buttonLayout = new GUILayoutOption[] { GUILayout.Height(50) };
    private void OnGUI()
    {

        GUI.skin.textField.fontSize = 25;
        GUI.skin.button.fontSize = 25;
        GUI.skin.label.fontSize = 25;

        CurrentSate = fsm.CurrentState.ToString();

        GUILayout.BeginHorizontal();
        fileName = GUILayout.TextField(fileName, textFieldLayout);
        if (GUILayout.Button("R", buttonLayout))
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
        if (GUILayout.Button("-1", buttonLayout))
        {
            seed = -1;
        }
        GUILayout.EndHorizontal();
        // if (seed != -1)
        // {
        //     if (GUILayout.Button("新地图", layout2))
        //     {
        //         fsm.ChangeState(GameState.Loading);
        //         seed = -1;
        //         UnLoad();
        //         var map = Map.Generate(size, seed);
        //         LoadMap(map);
        //         fsm.ChangeState(GameState.Playing);
        //     }
        // }
        if (GUILayout.Button("创建", buttonLayout))
        {
            UnLoad();
            var map = Map.Generate(size, seed);
            LoadMap(map);
            SaveCurrentMap(Path.Combine(SaveDirectory, fileName));
        }
        // if (GUILayout.Button("自动加载", layout2))
        // {
        //     fsm.ChangeState(GameState.Loading);
        //     GenerateFromLocalFile(Path.Combine(SaveDirectory, autoFileName));
        //     fsm.ChangeState(GameState.Playing);
        // }

        // if (GUILayout.Button("加载", buttonLayout))
        // {
        //     //fsm.ChangeState(GameState.Loading);
        //     LoadFromLocalFile(Path.Combine(SaveDirectory, fileName));
        //     //fsm.ChangeState(GameState.Playing);
        // }

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
            GUILayout.Label("地图未创建。");
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
    // void AutoSave()
    // {
    //     if (File.Exists(Path.Combine(SaveDirectory, autoFileName)))
    //         File.Delete(Path.Combine(SaveDirectory, autoFileName));
    //     SaveCurrentMap(Path.Combine(SaveDirectory, autoFileName));
    // }
    public Map map { get; private set; }
    void SaveCurrentMap(string filePath, bool temp = false)
    {
        if (map == null)
        {
            InfoWindow.Create("请确保地图已加载");
            return;
        }

        //创建目录
        Directory.CreateDirectory(SaveDirectory);

        if (!temp && File.Exists(filePath) && Path.GetFileName(filePath) != DefaultSaveName)
        {
            SaveCurrentMap(filePath + ".dat"); //添加2个.dat表示重名存档
            return;
        }

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

    void UnLoad()
    {
        if (map != null)
            map.economyWrapper.OnDataUpdated -= OnEconomyDataUpdated;
        grid.transform.DestroyAllChild();
        map = null;
    }


    void LoadFromLocalFile(string FileName)
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
        OnMapLoaded?.Invoke(map);

        this.map = map;
        seed = map.MainRandomSeed;

        //grid.transform.position = new Vector3(-map.size.x * grid.cellSize.x / 2f, 0, /*-map.size.y * grid.cellSize.z / 2f*/0); //对齐到左下角
        CinemachineVirtualCamera.transform.position = new Vector3(map.size.x * grid.cellSize.x / 2f, CinemachineVirtualCamera.transform.position.y, map.size.y * grid.cellSize.z / 2f);

        map.economyWrapper.OnDataUpdated += OnEconomyDataUpdated;
        OnEconomyDataUpdated(map.economyWrapper.GetValue());

        AfterMapLoaded?.Invoke(map);

    }

}


