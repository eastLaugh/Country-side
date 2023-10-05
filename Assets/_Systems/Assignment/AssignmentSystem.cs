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
    public static List<BasicAssignment> assignmentList = new List<BasicAssignment>();
    public static List<BasicAssignment> displayList = new List<BasicAssignment>();
    public bool isMapLoaded = false;
    private void OnEnable()
    {
    
        GameManager.OnMapLoaded += OnMapLoaded;
        GameManager.OnMapUnloaded += OnMapUnloaded;
    }
    private void OnMapUnloaded()
    {
        displayList.Clear();
        assignmentList.Clear();     
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
        EventHandler.CallInitSoundEffect(SoundName.MissionFinish); 
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
        for(int i = 0;i<assignmentList.Count;i++)
        {
            if (assignmentList[i].unlock && !assignmentList[i].finished && !displayList.Contains(assignmentList[i]))
            {
                displayList.Add(assignmentList[i]);
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
        var A4_5 = new BasicAssignment("幸福度达到100", "",
            "",
            () =>
            {
                if (map.MainData.Happiness == 100)
                {
                    return true;
                }
                else { return false; }
            },
            () =>
            {
                //TODO
            });
        var A4_4 = new BasicAssignment("用绿色能源供给所有建筑", "",
            "金钱+100万",
            () =>
            {
                //TODO
                return false;
            },
            () =>
            {
                map.MainData.Happiness += 8;
                map.MainData.Money += 100;
            });
        var A4_3 = new BasicAssignment("建造2个新能源充电桩", "",
            "幸福度+8",
            () =>
            {
                //TODO
                return false;
            },
            () =>
            {
                map.MainData.Happiness += 8;
            });
        var A4_2 = new BasicAssignment("建造特色景区", "",
            "幸福度+5",
            () =>
            {
                //TODO
                return false;
            },
            () =>
            {
                map.MainData.Happiness += 5;
            });
        var A4_1 = new BasicAssignment("建造5个民宿", "",
            "无",
            () =>
            {
                if (map.GetBuildingNum(typeof(MapObjects.Homestay)) >= 5)
                {
                    return true;
                }
                else { return false; }
            },
            () =>
            {
            });
        var A3_5 = new BasicAssignment("日产值大于20万", "",
            "无",
            () =>
            {
                if (map.FarmProfitTotal.currentValue.m_value >= 20)
                {
                    return true;
                }
                else { return false; }
            },
            () =>
            {
            });
        var A3_4 = new BasicAssignment("建造5个智慧大棚", "",
            "金钱+120万",
            () =>
            {
                if (map.GetBuildingNum(typeof(MapObjects.intelGreenHouse)) >= 5)
                {
                    return true;
                }
                else { return false; }
            },
            () =>
            {
                map.MainData.Money += 120;
            });
        var A3_3 = new BasicAssignment("使共享单车覆盖所有居民楼", "",
            "幸福度+8",
            () =>
            {
                //TODO
                return false;
            },
            () =>
            {
                
            });
        var A3_2 = new BasicAssignment("建造电商服务中心", "",
            "无",
            () =>
            {
                if (map.GetBuildingNum(typeof(MapObjects.ECommerceServiceCenter)) >= 1)
                {
                    return true;
                }
                else { return false; }
            },
            () =>
            {
            });
        var A3_1 = new BasicAssignment("建造农业数字中心", "",
            "金钱+70万",
            () =>
            {
                if (map.GetBuildingNum(typeof(MapObjects.DigitalCenter)) >= 1)
                {
                    return true;
                }
                else { return false; }
            },
            () =>
            {
                map.MainData.Money += 70;
            });
        var A2_5 = new BasicAssignment("使5G覆盖整个村庄", "",
            "幸福度+15",
            () =>
            {
                for (int i = 0; i < map.Houses.Count; i++)
                {
                    var FiveGArea = map.Houses[i].slot.GetMapObject<MapObjects.FiveGArea>();
                    if (FiveGArea == null)
                    {
                        return false;
                    }
                }
                return true;
            },
            () =>
            {
                map.MainData.Happiness += 10;
            }).AddTolist();
        var A2_4 = new BasicAssignment("日产值大于5万", "",
             "金钱+200万",
             () =>
             {
                 if (map.FarmProfitTotal.currentValue.m_value >= 5)
                 {
                     return true;
                 }
                 else { return false; }
             },
             () =>
             {
                 map.MainData.Money += 200;
             }).AddTolist();
        var A2_3 = new BasicAssignment("建造30条沥青路", "",
             "幸福度+10",
             () =>
             {
                 if (map.GetBuildingNum(typeof(MapObjects.PitchRoad)) >= 30)
                 {
                     return true;
                 }
                 else { return false; }
             },
             () =>
             {
                 map.MainData.Happiness += 10;
             }).AddTolist();
        var A2_2 = new BasicAssignment("建造1个秸秆处理厂", "",
             "金钱10万，幸福度+8",
             () =>
             {
                 //TODO
                 return false;
             },
             () =>
             {
                 map.MainData.Happiness += 8;
                 map.MainData.Money += 30;
                  
             }).AddTolist();
        var A2_1 = new BasicAssignment("建造5个温室大棚", "",
            "金钱30万",
            () =>
            {
                if (map.GetBuildingNum(typeof(MapObjects.GreenHouse)) >= 5)
                {
                    return true;
                }
                else { return false; }
            },
            () =>
            {
                map.MainData.Money += 30;
               
            }).AddTolist();
        var A1_4 = new BasicAssignment("建造1个水稻加工厂", "",
            "金钱10万,幸福度+3",
            () =>
            {
                //TODO
                for (int i = 0;i<map.Houses.Count; i++)
                {
                    var TubeArea = map.Houses[i].slot.GetMapObject<MapObjects.FiveGArea>();
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
                
            });
        var A1_3 = new BasicAssignment("建造3个光伏（住宅）","",
            "幸福度+3",
            () =>
            {
                if (map.GetBuildingNum(typeof(MapObjects.PV)) >= 3)
                {
                    return true;
                }
                else { return false; }
            },
            () =>
            {
                UnlockAssignment(A2_1);
                UnlockAssignment(A2_2);
                UnlockAssignment(A2_3);
                map.MainData.Happiness += 3;
                map.Phase = 2;
                EventHandler.CallPhaseUpdate(2);
            }).AddTolist();
       
        var A1_2 = new BasicAssignment("建造3个水泥房",
            "在建造栏点击土坯房，并在村庄网格中建造就行了","无",
            () =>
            {
                //Debug.Log(map.GetBuildingNum("AdobeHouse"));
                if(map.GetBuildingNum(typeof(MapObjects.CementHouse)) >= 3)
                {
                    return true;
                }
                else { return false; } 
            },
            () =>
            {
                
                UnlockAssignment(A1_3);
                
            }).AddTolist();
        var A1_1 = new BasicAssignment("建10条水泥路",
           "在建造栏点击水井，并在村庄网格中建造就行了", "无",
           () =>
           {
               //Debug.Log(map.GetBuildingNum("AdobeHouse"));
               if (map.GetBuildingNum(typeof(MapObjects.CementRoad)) >= 10)
               {
                   return true;
               }
               else { return false; }
           },
           () =>
           {
               UnlockAssignment(A1_2);
           }).AddTolist();

        #endregion
        //tutorial1.unlock = true;
        //assignmentLists.Add(tutorial1);
        //assignmentLists.Add(tutorial2);
        //assignmentLists.Add(Assignment1);
        //assignmentLists.Add(Assignment2);
        //assignmentLists.Add(Assignment3);
        //assignmentLists.Add(Assignment4);
        //assignmentLists.Add(Assignment5);
        UnlockAssignment(A1_1);
        for(int i = 0;i<map.UnlockedAssignments.Count;i++)
        {
            var unlockAssignment = assignmentList.Find((assignment) => { return assignment.name == map.UnlockedAssignments[i]; });   
            unlockAssignment.unlock = true;
        }
        for (int i = 0; i < map.FinishedAssignments.Count; i++)
        {
            var unlockAssignment = assignmentList.Find((assignment) => { return assignment.name == map.FinishedAssignments[i]; });
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
public static class AssignmentExtensions
{
    public static BasicAssignment AddTolist(this BasicAssignment assignment)
    {
        AssignmentSystem.assignmentList.Add(assignment);
        return assignment;
    }
}
