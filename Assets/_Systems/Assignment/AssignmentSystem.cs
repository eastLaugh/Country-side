using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AssignmentSystem : MonoBehaviour
{

    public static AssignmentSystem Instance;
    Map map;
    [SerializeField] Transform RuleItemRoot;
    [SerializeField] AssignmentBarUI assignmentBarPrefab;
    public List<BasicAssignment> assignmentLists = new List<BasicAssignment>();
    public List<BasicAssignment> displayList = new List<BasicAssignment>();
    public bool isMapLoaded = false;
    private void OnEnable()
    {
    
        Instance = this;
        GameManager.OnMapLoaded += OnMapLoaded;
        GameManager.OnMapUnloaded += OnMapUnloaded;
    }
    private void OnMapUnloaded()
    {
        displayList.Clear();
        assignmentLists.Clear();     
        isMapLoaded = false;
    }
    private void OnMapLoaded(Map map)
    {
        this.map = map;
        AssignmentInit();
        isMapLoaded=true;
    }
    private void Update()
    {
        if (isMapLoaded) 
        {
            CheckAssignments();
        }
        
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
        map.FinishedAssignments.Add(basicAssignment.name);
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
            //Debug.Log(displayList[i].name);
            Bar.UpdateBar(displayList[i].name, displayList[i].prizeText);
            Bar.gameObject.SetActive(true);

        }
    }
    public void AssignmentInit()
    {
        #region 任务初始化 
        var Assignment5 = new BasicAssignment("建造30条水泥路", "",
             "幸福度+10",
             () =>
             {
                 if (map.GetBuildingNum("CementRoad") >= 30)
                 {
                     return true;
                 }
                 else { return false; }
             },
             () =>
             {
                 map.MainData.Happiness += 8;
                 map.MainData.Money += 30;
             });
        var Assignment4 = new BasicAssignment("建造5个砖瓦房", "",
             "金钱10万，幸福度+8",
             () =>
             {
                 if (map.GetBuildingNum("TileHouse") >= 30)
                 {
                     return true;
                 }
                 else { return false; }
             },
             () =>
             {
                 map.MainData.Happiness += 8;
                 map.MainData.Money += 30;
                 UnlockAssignment(Assignment5);
             });
        var Assignment3 = new BasicAssignment("为所有农田供水", "",
            "金钱30万",
            () =>
            {
                for (int i = 0; i < map.Farms.Count; i++)
                {
                    var TubeArea = map.Farms[i].slot.GetMapObject<MapObjects.WaterArea>();
                    if (TubeArea == null)
                    {
                        return false;
                    }
                }
                return true;
            },
            () =>
            {
                map.MainData.Money += 30;
                UnlockAssignment(Assignment4);
            });
        var Assignment2 = new BasicAssignment("为所有住宅供水", "",
            "金钱10万,幸福度+3",
            () =>
            {
                for (int i = 0;i<map.Houses.Count; i++)
                {
                    var TubeArea = map.Houses[i].slot.GetMapObject<MapObjects.WaterArea>();
                    if (TubeArea==null)
                    {
                        return false;
                    }                   
                }
                return true;
            },
            () =>
            {
                map.MainData.Money += 10;
                map.MainData.Happiness += 3;
                UnlockAssignment(Assignment3);
            });
        var Assignment1 = new BasicAssignment("建造20块农田","",
            "金钱10万",
            () =>
            {
                //Debug.Log(map.GetBuildingNum("AdobeHouse"));
                if (map.Farms.Count >= 20)
                {
                    return true;
                }
                else { return false; }
            },
            () =>
            {
                UnlockAssignment(Assignment2);
                map.MainData.Money += 10;
            });
        var tutorial2 = new BasicAssignment("建一个水井",
            "在建造栏点击水井，并在村庄网格中建造就行了","无",
            () =>
            {
                //Debug.Log(map.GetBuildingNum("AdobeHouse"));
                if (map.GetBuildingNum("Well") >= 1)
                {
                    return true;
                }
                else { return false; }
            },
            () =>
            {
                UnlockAssignment(Assignment1);
            });
        var tutorial1 = new BasicAssignment("建三个土坯房",
            "在建造栏点击土坯房，并在村庄网格中建造就行了","无",
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
                
                UnlockAssignment(tutorial2);
                
            });
        

        #endregion
        tutorial1.unlock = true;
        assignmentLists.Add(tutorial1);
        assignmentLists.Add(tutorial2);
        assignmentLists.Add(Assignment1);
        assignmentLists.Add(Assignment2);
        assignmentLists.Add(Assignment3);
        assignmentLists.Add(Assignment4);
        assignmentLists.Add(Assignment5);
        for(int i = 0;i<map.UnlockedAssignments.Count;i++)
        {
            var unlockAssignment = assignmentLists.Find((assignment) => { return assignment.name == map.UnlockedAssignments[i]; });   
            unlockAssignment.unlock = true;
        }
        for (int i = 0; i < map.FinishedAssignments.Count; i++)
        {
            var unlockAssignment = assignmentLists.Find((assignment) => { return assignment.name == map.FinishedAssignments[i]; });
            unlockAssignment.finished = true;
        }

        UIRefresh();
        //Debug.Log("added");

    }
    void UnlockAssignment(BasicAssignment assignment)
    {
        assignment.unlock = true;
        if(!map.UnlockedAssignments.Contains(assignment.name))
        {
            map.UnlockedAssignments.Add(assignment.name);
        }       
    }
    
}
