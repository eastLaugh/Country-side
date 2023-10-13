using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AssignmentSystem : MonoBehaviour , Person.IPromptProvider
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

        Persons.Headman.instance.promptProviders.Add(this);
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
        var A4_3 = new BasicAssignment("绿色能源占比达到100%", 
            "我们的村子环境变得越来越好了，下一步的计划是用绿色能源供给所有用电设施，可以通过建设更多光伏发电装置，光伏电厂和秸秆处理厂来实现。",
            "金钱+500万",
            () =>
            {
                if (PowerSystem.greenPowerRatio >= 1)
                    return true;
                else
                    return false;
            },
            () =>
            { 
                map.MainData.Money += 500;
            }).AddTolist();
        var A4_2 = new BasicAssignment("建造2个新能源充电桩", 
            "你发现了吗？好多家庭都拥有了新能源车，也有许多旅客开新能源车过来，但是电量消耗很快，要经常充电。为了方便旅客和村民充电，我们可以在村里建造2个新能源充电桩。",
            "幸福度+8",
            () =>
            {
                if (map.GetBuildingNum(typeof(MapObjects.ChargingStation)) >= 2)
                {
                    return true;
                }
                else { return false; }
            },
            () =>
            {
                map.MainData.Happiness += 8;
            }).AddTolist();
        var A4_1 = new BasicAssignment("建造5个民宿", "为了积极响应国家乡村振兴的号召，我们可以在村里发展旅游业。建造几个民宿，提升我们村庄的知名度和吸引力。",
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
            }).AddTolist();
        var A3_4 = new BasicAssignment("日产值大于20万", "日产值要超过20万，可以考虑升级温室大棚为智慧大棚以及运用无人机在水稻田上。",
            "金钱+500万",
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
                CheckToPhase4();
                map.MainData.Money += 500;
            }).AddTolist();
        var A3_3 = new BasicAssignment("建造5个智慧大棚", 
            "我们的农产品一直以来都备受好评，销售量也在不断上升。现在，我们面临着供不应求的情况。为了满足市场需求，提高农产品的产量和质量，我提议将5个温室大棚升级为智慧大棚。通过采用先进的智能化技术，我们可以更好地控制农作物的生长环境，提高产量，同时还可以降低农业生产成本。",
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
                CheckToPhase4();
                map.MainData.Money += 120;
            }).AddTolist();
        var A3_2 = new BasicAssignment("建造电商服务中心", 
            "我们村子里的农产品越来越丰富，质量也越来越高，但是外界对我们农产品的认知度却还远远不够。为了更好地推广我们的农产品和提升村民收入，建造一个电商服务中心，将我们的优质农产品销往全国各地吧。",
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
                CheckToPhase4();
            }).AddTolist();
        var A3_1 = new BasicAssignment("建造农业数字中心", 
            "我们村庄的规模正在不断扩大，农田也在逐步扩大。为了更好地收集农田信息，加强农田管理，我建议建立一个农业数字中心。这样，我们不仅掌握农田的情况，而且还能为其他数字化建筑提供技术支撑。",
            "金钱+100万",
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
                map.MainData.Money += 100;
                CheckToPhase4();
            }).AddTolist();
        var A2_5 = new BasicAssignment("使5G覆盖整个村庄", "为了提升村民生活质量，加强乡村与外界的联系，希望你能建设多个5G基站，使5G覆盖所有居民楼。",
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
                map.MainData.Happiness += 15;
                CheckToPhase3();
            }).AddTolist();
        var A2_4 = new BasicAssignment("日产值大于5万", "日产值要超过5万，也许升级菜田成为温室大棚是个不错的方法。",
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
                 CheckToPhase3();
             }).AddTolist();
        var A2_3 = new BasicAssignment("建造30条沥青路", "现在村里也逐渐有各种各样的车了呢，为了让汽车能行驶更快速，建造30条沥青路吧。",
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
                 CheckToPhase3();
             }).AddTolist();
        var A2_2 = new BasicAssignment("建造1个氢能发电站", "我们村每年种植都会产生不少秸秆，处理秸秆可真是个难题。不过，听说我们市有将秸秆转化为氢能的工程计划。" +
            "试着建造一座这样的氢能发电站吧。",
             "金钱20万",
             () =>
             {
                 if (map.GetBuildingNum(typeof(MapObjects.BiomassPlant)) >= 1)
                 {
                     return true;
                 }
                 else { return false; }
             },
             () =>
             {
                 map.MainData.Money += 20;
                 CheckToPhase3();

             }).AddTolist();
        var A2_1 = new BasicAssignment("建造5个温室大棚", 
            "目前我们村菜田的产出还是不怎么高，所以在菜田上建几个温室大棚吧，这样不仅能保护作物，还能够在淡季生产，获得高的市场价格。",
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
                CheckToPhase3();
               
            }).AddTolist();
        var A1_3 = new BasicAssignment("建造3个光伏（住宅）","hhhhhhh",
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
                UnlockAssignment(A2_4);
                UnlockAssignment(A2_5);
                //map.MainData.Happiness += 3;
                
            }).AddTolist();
       
        var A1_2 = new BasicAssignment("建造3个水泥房",
            "在建造栏点击土坯房，并在村庄网格中建造就行了","无",
            () =>
            {
                //Debug.Log(map.GetBuildingNum("AdobeHouse"));
                if(map.GetBuildingNum(typeof(MapObjects.CementHouse)) >= 22)
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
           "在建造栏点击水井，并在村庄网格中建造就行了", "幸福度+1",
           () =>
           {
               //Debug.Log(map.GetBuildingNum("AdobeHouse"));
               if (map.GetBuildingNum(typeof(MapObjects.CementRoad)) >= 43)
               {
                   return true;
               }
               else { return false; }
           },
           () =>
           {
               //map.MainData.Happiness += 1;
               UnlockAssignment(A1_2);
           }).AddTolist();

        #endregion
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
        void CheckToPhase3()
        {
            if (displayList.Count == 1) //Finish 在 diplaylist 删掉之前 所以为1
            {
                UnlockAssignment(A3_1);
                UnlockAssignment(A3_2);
                UnlockAssignment(A3_3);
                UnlockAssignment(A3_4);
                map.Phase = 3;
                EventHandler.CallPhaseUpdate(3);
            }
        }
        void CheckToPhase4()
        {
            if (displayList.Count == 1)
            {
                UnlockAssignment(A4_1);
                UnlockAssignment(A4_2);
                UnlockAssignment(A4_3);
                map.Phase = 4;
                EventHandler.CallPhaseUpdate(4);
            }
        }
    }
    void UnlockAssignment(BasicAssignment assignment)
    {
        assignment.unlock = true;
        if(!map.UnlockedAssignments.Contains(assignment.name))
        {
            map.UnlockedAssignments.Add(assignment.name);
        }       
    }

    public string GetPrompt()
    {
        string list = "玩家还有任务没有完成：";
        foreach (var ele in displayList)
        {
            list += ele.name;
        }
        return list+"如果玩家问起，顺便解释一下这些任务的意义。";
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
