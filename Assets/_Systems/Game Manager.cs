using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Runtime.Serialization;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.InputSystem.XR.Haptics;
#if UNITY_EDITOR
using UnityEditor;
#endif
using NaughtyAttributes;
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

    private GlobalData globalData;
    private TimeSystem timeSystem;
    private illuBookSystem illuBookSystem;
    public enum GameState
    {
        Unload,NewGame,Loading,Playing
    }
    FSM<GameState> fsm = new FSM<GameState>();

    public Vector2Int size;
    [Header("存储")]

    [NaughtyAttributes.ReadOnly]
    public string SaveDirectory;

    [NaughtyAttributes.Foldout("数据库")]
    public MapObjectDatabase MapObjectDatabase;
    public SlotDatabase SlotDatabase;
    public illuBookData_SO illuBookData;

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
        FSMInit();
        var map = Map.Generate(size, seed);
        LoadMap(map);
        //InfoWindow.Create("这是一个窗口，点击右下角关闭");

        // var map = GenerateMap(size);
        // LoadMap(map);
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
    private void Update()
    {
        fsm.update();
    }

    private void Awake()
    {
        DG.Tweening.DOTween.Init();
        DG.Tweening.DOTween.SetTweensCapacity(size.x * size.y, 50);
        current = this;
        SaveDirectory = Path.Combine(Application.persistentDataPath, "beta");
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
            Debug.Log(Path.Combine(SaveDirectory, globalFileName));
        }
        else
        {
            globalData = new GlobalData();
            SaveGlobalData(globalData);
        }
    }
    private void SaveGlobalData(GlobalData globalData)
    {
        string GlobalData = JsonConvert.SerializeObject(globalData, SerializeSettings);
        Directory.CreateDirectory(SaveDirectory);
        if (File.Exists(Path.Combine(SaveDirectory, globalFileName)))
            File.Delete(Path.Combine(SaveDirectory, globalFileName));
        File.WriteAllText(Path.Combine(SaveDirectory, globalFileName), GlobalData);
    }
    #region SystemInit

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
        uiManager.Initialize(timeSystem,illuBookSystem);
    }
    
    #endregion

    int seed = -1;  // -1 : 由程序随机生成种子
    string globalFileName = "GlobalSave.dat";
    string autoFileName = "AutoSave.dat";
    string fileName = "Save0.dat";
    private void OnGUI()
    {
        GUI.skin.textField.fontSize = 25;
        GUI.skin.button.fontSize = 25;
        GUILayoutOption[] layout1 = new GUILayoutOption[] { GUILayout.Width(400), GUILayout.Height(50) };
        GUILayoutOption[] layout2 = new GUILayoutOption[] { GUILayout.Width(150), GUILayout.Height(50) };
        CurrentSate = fsm.CurrentState.ToString();
        fileName = GUILayout.TextField(fileName, layout1);

        if (int.TryParse(GUILayout.TextField(seed.ToString(),layout1), out int newSeed))
        {
            seed = newSeed;
        }
        else
        {
            seed = -1;
        }
        if (seed != -1)
        {
            if (GUILayout.Button("新地图", layout2))
            {
                fsm.ChangeState(GameState.Loading);
                seed = -1;
                UnLoad();
                var map = Map.Generate(size, seed);
                LoadMap(map);
                fsm.ChangeState(GameState.Playing);
            }
        }
        //if (GUILayout.Button("创建", layout2))
        //{
        //    UnLoad();
        //    var map = Map.Generate(size, seed);
        //    LoadMap(map);
        //
        //}
        if (GUILayout.Button("自动加载", layout2))
        {
            fsm.ChangeState(GameState.Loading);
            GenerateFromLocalFile(Path.Combine(SaveDirectory, autoFileName));
            fsm.ChangeState(GameState.Playing);
        }
        if (GUILayout.Button("加载", layout2))
        {
            fsm.ChangeState(GameState.Loading);
            GenerateFromLocalFile(Path.Combine(SaveDirectory, fileName));
            fsm.ChangeState(GameState.Playing);
        }

        if (map == null)
        {
            GUILayout.Label("地图未存在");
        }
        else
        {
            if (GUILayout.Button("保存", layout2))
            {
                bool saved = false;                
                while(!saved)
                {
                    string fileName = "Save" + saveIndex + ".dat";
                    if (File.Exists(Path.Combine(SaveDirectory, fileName)))
                    {
                        saveIndex = saveIndex + 1;
                    }
                    else
                    {
                        SaveCurrentMap(Path.Combine(SaveDirectory, fileName));
                        SaveGlobalData(globalData);
                        saved = true;
                    }
                }
                
            }
            if (GUILayout.Button("[调试|查看地图信息]", layout1))
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
    void AutoSave()
    {
        if (File.Exists(Path.Combine(SaveDirectory, autoFileName)))
            File.Delete(Path.Combine(SaveDirectory, autoFileName));
        SaveCurrentMap(Path.Combine(SaveDirectory, autoFileName));
    }
    public Map map { get; private set; }
    void SaveCurrentMap(string filePath)
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
        File.WriteAllText(filePath, gameData);
#if UNITY_EDITOR
        UnityEditor.EditorUtility.OpenWithDefaultApp(filePath);
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


