using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AssignmentSystem : MonoBehaviour
{
    Map map;
    [SerializeField] Transform RuleItemRoot;
    [SerializeField] AssignmentBarUI assignmentBarPrefab;
    public List<BasicAssignment> assignmentLists = new List<BasicAssignment>();
    public List<BasicAssignment> displayList = new List<BasicAssignment>();

    private void OnEnable()
    {
        GameManager.OnMapLoaded += OnMapLoaded;
        GameManager.OnMapUnloaded += OnMapUnloaded;
    }

    private void OnMapUnloaded()
    {
        enabled = false;      
    }

    private void OnMapLoaded(Map map)
    {
        this.map = map;
        AssignmentInit();
        enabled = true;
    }
    void Start()
    {
        enabled = false;
    }
    private void Update()
    {
        CheckAssignments();
    }
    private void CheckAssignments()
    {
        for(int i = 0; i < displayList.Count; i++)
        {
            if (displayList[i].Check()) 
            {
                AssignmentFinished(displayList[i],i);
            }
        }
    }

    private void AssignmentFinished(BasicAssignment basicAssignment,int index)
    {
        basicAssignment.Finish();
        displayList.RemoveAt(index);
        RuleItemRoot.GetChild(index).GetComponent<AssignmentBarUI>().DestroyBar();
        StartCoroutine(WaitforDestroy());
    }
    IEnumerator WaitforDestroy()
    {
        yield return new WaitForSeconds(2);
        UIRefresh();
    }
    private void UIRefresh()
    {
        for (int i = 0; i < RuleItemRoot.childCount; i++)
        {
            Destroy(RuleItemRoot.GetChild(i).gameObject);
        }
        for(int i = 0;i<assignmentLists.Count;i++)
        {
            if (assignmentLists[i].unlock && !assignmentLists[i].finished && !displayList.Contains(assignmentLists[i]))
            {
                displayList.Add(assignmentLists[i]);
            }
        }
        for (int i = 0; i < displayList.Count; i++)
        {
            //var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
            var Bar = Instantiate(assignmentBarPrefab, RuleItemRoot);
            Debug.Log(displayList[i].name);
            Bar.UpdateBar(displayList[i].name);
            Bar.gameObject.SetActive(true);
        }
    }
    public void AssignmentInit()
    {
        var tempAssignment1 = new BasicAssignment("������������",
            "������Ҫ������ֲ������������ϴ����и��ණ���ԣ����������ȥ׬Ǯ�͸����ˡ���������������ʪ����Ϊ�����أ�Ӧ����ֲС����ˮ����?",
            () =>
            {
                //Debug.Log(map.GetBuildingNum("AdobeHouse"));
                if(map.GetBuildingNum("AdobeHouse") >= 3)
                {
                    return true;
                }
                else { return false; } 
            },
            () =>
            {
                //map.economyWrapper.AddMiddleware(new SolidMiddleware<GameDataVector>(new GameDataVector(0, 1000, 0),this,map.economyWrapper));
            });
        tempAssignment1.unlock = true;
        assignmentLists.Add(tempAssignment1);
        UIRefresh();
        //Debug.Log("added");

    }

    
}
