using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventUISystem : MonoBehaviour
{
    Dictionary<string, Action<int>> OptEvents = new Dictionary<string, Action<int>>();
    /// <summary>
    /// csv��ʽ
    /// </summary>
    public List<TextAsset> EventDataFiles;
    public TextMeshProUGUI mainContent;
    public GameObject EventUI;
    [SerializeField] TimeController timeController;
    public int optIndex;
    public string[] optRows;
    public GameObject optButtonTemplate;
    public RectTransform optRoot;
    /// <summary>
    /// TODO
    /// </summary>
    Dictionary<string, Sprite> imageDic = new Dictionary<string, Sprite>();
    private void Start()
    {
        enabled = false;
    }
    private void OnEnable()
    {
        GameManager.OnMapLoaded += OnMapLoaded;
        GameManager.OnMapUnloaded += OnMapUnloaded;
    }
    public void OnMapLoaded(Map _)
    {
        enabled = true;
        InitOptEvents();
        EventHandler.DayPass += RandomEvent;
    }
    public void OnMapUnloaded()
    {
        enabled = false;
    }
    private void RandomEvent()
    {
        int random = UnityEngine.Random.Range(0, 100);
        if (random == 1)
        {
            GenerateEvent();
        }
    }
    private void GenerateEvent()
    {
        ReadText(EventDataFiles[UnityEngine.Random.Range(0, EventDataFiles.Count)]);
        EventUI.SetActive(true);
        timeController.Pause();
    }
    public void ReadText(TextAsset textAsset)
    {
        optRows = textAsset.text.Split('\n');
        optIndex = 1;
        ShowContent();
    }
    public void ShowContent()
    {
        for (int i = 1; i < optRows.Length; i++)
        {
            string[] cell = optRows[i].Split(',');
            if (!int.TryParse(cell[1], out int index)) continue;
            if (index != optIndex) continue;
            if (cell[0] == "#")
            {
                //Display
                mainContent.text = cell[2];
                //
                optIndex = int.Parse(cell[3]);//TODO
                var continueBtn = Instantiate(optButtonTemplate, optRoot);
                continueBtn.GetComponentInChildren<TMP_Text>().text = "����";
                continueBtn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    for (int i = 0; i < optRoot.childCount; i++)
                    {
                        Destroy(optRoot.GetChild(i).gameObject);
                    }
                    ShowContent();
                });
                continueBtn.SetActive(true);
                if (cell[4] != null)
                {
                    CheckEffect(cell[4]);
                }
                break;
            }
            else if (cell[0] == "&")
            {
                mainContent.text = cell[2];
                GenerateOpt(i + 1);
                break;
            }
            else if (cell[0] == "End")
            {
                mainContent.text = cell[2];
                var endBtn = Instantiate(optButtonTemplate, optRoot);
                endBtn.GetComponentInChildren<TMP_Text>().text = "����";
                endBtn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    for (int i = 0; i < optRoot.childCount; i++)
                    {
                        Destroy(optRoot.GetChild(i).gameObject);
                    }
                    EventUI.SetActive(false);
                    timeController.Continue();
                });
                endBtn.SetActive(true);
                if (cell[4] != null)
                {
                    CheckEffect(cell[4]);
                }

                break;
            }

        }

    }
    public void GenerateSimpleBtn(string content, bool isEnd)
    {

    }
    void GenerateOpt(int index)
    {
        string[] cells = optRows[index].Split(',');
        if (cells[0] == "&")
        {
            var optButton = Instantiate(optButtonTemplate, optRoot);
            optButton.GetComponentInChildren<TMP_Text>().text = cells[2];//TODO
            optButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnOptionClick(int.Parse(cells[3]));//TODO
            });
            optButton.SetActive(true);
            GenerateOpt(index + 1);
        }
    }
    void InitOptEvents()
    {
        OptEvents.Add("AddMoney", (value) =>
        {
            GameManager.current.map.economyWrapper.AddMiddleware(new SolidMiddleware<EconomyVector>(new EconomyVector(0, value, 0), null));
        });
        OptEvents.Add("AddPopulation", (value) =>
        {
            GameManager.current.map.economyWrapper.AddMiddleware(new SolidMiddleware<EconomyVector>(new EconomyVector(value, 0, 0), null));
        });

    }
    void CheckEffect(string effectText)
    {
        string[] cells = effectText.Split("|");
        foreach (string cell in cells)
        {
            string[] paraments = cell.Split(' ');
            if (paraments.Length == 2)
            {
                if (OptEvents.ContainsKey(paraments[0]))
                {
                    OptEvents[paraments[0]].Invoke(int.Parse(paraments[1]));
                }
                else
                {
                    Debug.LogError("û�����ֽ�" + paraments[0] + "��Ч��");
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void OnOptionClick(int index)
    {
        optIndex = index;

        for (int i = 0; i < optRoot.childCount; i++)
        {
            Destroy(optRoot.GetChild(i).gameObject);
        }
        ShowContent();
    }
}
