using System.Collections;           //该命名空间包含C#中许多常用的数据结构和算法
using System.Collections.Generic;   //同上
using UnityEngine;


//自定义命名空间，该类用于管理玩家背包中的物品，供其他模块系统引用
    //继承Singleton<InventoryManager>，便于其他类引用这两个类的内容
public class illuBookSystem 
{
    /*在背包中，每种物品都是一个InventoryBuilding对象；
    每个物品包含两个字段：BuildingID表示物品的ID，amount表示物品的数量；
    背包中所有物品的信息都存储在playerBag_SO对象中*/
    private List<BuildingDetails> illuBookList => GameManager.current.illuBookData.illuBookList;
    //定义玩家背包内的全物品属性
    /// <summary>
    /// 找到物品信息
    /// </summary>
    /// <param name="ID">物品ID</param>
    /// <returns>物品信息</returns>
    private GlobalData globalData;
    public BuildingDetails GetBuildingDetails(string name)       //根据指定的背包物品的ID，获取背包内物品的详细信息
    {
        return illuBookList.Find(i => i.name == name);
    }

    public illuBookSystem(GlobalData globalData)
    {
        this.globalData = globalData;
        Init();
    }
    private void Init()
    {
        //if (globalData == null) { return; }
        Debug.Log(globalData.unlockIlluBookName);
        for (int i = 0;i<illuBookList.Count;i++)
        {
            if (globalData.unlockIlluBookName.Contains(illuBookList[i].name))
                illuBookList[i].unclock = true;
            else
                illuBookList[i].unclock = false;
        }
        //EventHandler.illuBookUnlocked += Unlock;
    }

    public void Unlock(string name)
    {
        globalData.unlockIlluBookName.Add(name);
        var detail = GetBuildingDetails(name);
        detail.unclock = true;
    }

    

    
    /// <summary>
    /// 把物品放入背包（入口函数）
    /// </summary>
    /// <param name="ID">物品ID</param>
    /// <param name="num">物品数量</param>
    /// <returns>是否能放入背包</returns>
    //public bool AddBuilding(int ID, int num)             //将指定数量的物品添加到背包中（作为入口函数），用bool量检测添加过程（ID和num）
    //{
    //    int index = GetBuildingIndexInBag(ID);
    //    if (!CheckBagCapacity() && index == -1)
    //    {
    //        return false;
    //    }
    //    AddBuildingAtIndex(ID, index, num);
    //    
    //    return true;
    //}
    //public bool ReduceBuilding(int ID,int num)          //将指定数量的物品移除背包（作为出口函数），用bool量检测添加过程（ID和num）
    //{
    //    for (int i = 0; i < playerBag_SO.BuildingList.Count; i++)
    //    {
    //        if (playerBag_SO.BuildingList[i].BuildingID == ID && playerBag_SO.BuildingList[i].amount >= num)
    //        {
    //            playerBag_SO.BuildingList[i].amount -= num;
    //            return true;
    //        }
    //    }
    //    return false;
    //
    //}
    ////在背包中查找是否存在指定数量的物品，存在则返回true，不存在返回false
    //public bool SearchBuilding(int ID, int num)
    //{
    //    for (int i = 0; i < playerBag_SO.BuildingList.Count; i++)
    //    {
    //        if (playerBag_SO.BuildingList[i].BuildingID == ID && playerBag_SO.BuildingList[i].amount >= num)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}
    //
    ////通过设置物品属性，清空背包中的所有物品
    //public void EmptyBuildings()
    //{
    //    for (int i = 0; i < playerBag_SO.BuildingList.Count; i++)
    //    {
    //        playerBag_SO.BuildingList[i].amount = 0;
    //        playerBag_SO.BuildingList[i].BuildingID = 0;
    //    }
    //   
    //}
    //
    ///// <summary>
    ///// 得到物品在背包的索引值
    ///// </summary>
    ///// <param name="ID">物品ID</param>
    ///// <returns>物品在背包的索引值</returns>
    //private int GetBuildingIndexInBag(int ID)           //通过传入指定物品的ID来获取该物品在背包中的索引值ID
    //{
    //    for (int i = 0; i < playerBag_SO.BuildingList.Count; i++)
    //    {
    //        if (playerBag_SO.BuildingList[i].BuildingID == ID)
    //        {
    //            return i;
    //        }
    //    }
    //    return -1;
    //}
    //
    ///// <summary>
    ///// 检查背包是否有空位
    ///// </summary>
    ///// <param name="ID">物品ID</param>
    ///// <returns>是否有空位</returns>
    //private bool CheckBagCapacity()                 //遍历背包容量来检查背包是否有空位
    //{
    //    for (int i = 0; i < playerBag_SO.BuildingList.Count; i++)
    //    {
    //        if (playerBag_SO.BuildingList[i].BuildingID == 0)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}
    //
    ///*将指定数量的物品添加到背包的指定位置，需依次传入ID、位置、数量；
    //该方法会自动检测背包是否有空位，若背包中没有空位则不会添加物品*/
    //private void AddBuildingAtIndex(int ID, int index, int amount)
    //{
    //    if (index == -1)
    //    {
    //        var Building = new InventoryBuilding { BuildingID = ID, amount = amount };
    //        for (int i = 0; i < playerBag_SO.BuildingList.Count; i++)
    //        {
    //            if (playerBag_SO.BuildingList[i].BuildingID == 0)
    //            {
    //                playerBag_SO.BuildingList[i] = Building;
    //                break;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        int currentAmount = playerBag_SO.BuildingList[index].amount + amount;
    //        var Building = new InventoryBuilding { BuildingID = ID, amount = currentAmount };
    //        playerBag_SO.BuildingList[index] = Building;
    //    }
    //}

    //程序开始时会调用，用于更新背包的UI界面
    void Start()
    {
        
    }
    
    //每一帧都会调用，实现实时更新
    void Update()
    {
        //可继续开发加入多样的更新时候的功能
    }
}

