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
    public bool isMapLoaded = false;
    private void OnEnable()
    {
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
        Debug.Log(assignmentLists.Count);
        var Assignment1 = new BasicAssignment("建造20块田地","金钱10万",
            "？？？",
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
                map.MainData.Money += 10;
            });
        var tutorial2 = new BasicAssignment("建一个水井",
            "在建造栏点击水井，并在村庄网格中建造就行了","无",
            () =>
            {
                //Debug.Log(map.GetBuildingNum("AdobeHouse"));
                if (map.GetBuildingNum("Tube") >= 1)
                {
                    return true;
                }
                else { return false; }
            },
            () =>
            {
                map.MainData.Money += 100;
            });
        var tutorial1 = new BasicAssignment("建三个泥土房",
            "在建造栏点击水井，并在村庄网格中建造就行了","无",
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
                UnlockAssignment(Assignment1);
                UnlockAssignment(tutorial2);
                map.MainData.Money += 100;
            });
        

        #endregion
        tutorial1.unlock = true;
        assignmentLists.Add(tutorial1);
        assignmentLists.Add(tutorial2);
        assignmentLists.Add(Assignment1);
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
