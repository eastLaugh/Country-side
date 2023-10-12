using static Slot;
using UnityEngine;
using DG.Tweening;
using Newtonsoft.Json;
using System;
using Random = UnityEngine.Random;
using System.Collections;
partial class MapObjects
{

    public class Tree : MapObject /* , IReject<House>, IReject<Road> */ , IInfoProvider
    {



        //实现这个接口（可选），用以显示在Slot Window上
        public void ProvideInfo(Action<string> provide)
        {
            provide("树");
            if (isChopping)
            {
                provide(":正在砍伐");
            }
        }

        #region Model
        //这是每个棵树要保存的模型信息，因为游戏中每一棵树的模型都不是固定的。但是需要被保存下来，留便下次加载使用
        [JsonProperty]
        TreeModel[] TreeModels;
        struct TreeModel
        {
            public Vector3 offset;
            public int prefabIndex;
        }
        #endregion

        protected override void Render(GameObject prefab, GameObject[] prefabs, SlotRender slotRender)
        {
            // base.Render(prefab, prefabs, slotRender); 我们不需要默认的渲染方式，故注释

            father.DestroyAllChildren();  //Render()这是个可能被系统多次调用的API，所以请确保可以被重复调用的健全性，需要删除“上次”的已渲染物体，避免重复

            if (TreeModels == null)
            {
                //说明未存储信息
                Vector3 halfCellSize = GameManager.current.grid.cellSize / 2f;
                TreeModels = new TreeModel[Random.Range(1, 5)];
                for (int i = 0; i < TreeModels.Length; i++)
                {
                    TreeModels[i] = new TreeModel
                    {
                        prefabIndex = Random.Range(0, prefabs.Length),
                        offset = new Vector3(Random.Range(-halfCellSize.x, halfCellSize.x), 0, Random.Range(-halfCellSize.z, halfCellSize.z))
                    };
                }
            }

            //根据已有信息加载模型
            for (int i = 0; i < TreeModels.Length; i++)
            {
                GameObject tree = UnityEngine.Object.Instantiate(prefabs[TreeModels[i].prefabIndex], slotRender.transform.position + TreeModels[i].offset, Quaternion.identity, father);
                // tree.SetActive(false);

                GameManager.current.StartCoroutine(WaitOneTick());
                IEnumerator WaitOneTick()
                {
                    yield return null;
                    if (tree)
                    {
                        tree.SetActive(true);
                        tree.transform.DOScale(Vector3.zero, Settings.建筑时物体缓动持续时间).From().SetEase(Ease.OutBack);
                    }else{
                        //这里我百思不得其解，为啥会有 诡异的树木
                    }
                }
            }

            //创建 图标的“调色盘”
            iconPattern = IconPattern.Create(father, Vector3.zero);

            // RefreshChopping(); 这里不能写这个，因为这里是Render（） API 仅限于渲染相关

        }

        [JsonProperty] //树木是否正在砍伐，这需要记录下来
        public bool isChopping { get; private set; }


        public static event Action<Tree, bool> OnTreeChopped;
        IconPattern iconPattern;
        GameObject ChoppingIcon;
        public override void OnClick()
        {
            //base.OnClick();
            //isChopping = !isChopping;
            //RefreshChoppingState();

        }


        //刷新砍树相关的事件
        void RefreshChoppingState()
        {
            OnTreeChopped?.Invoke(this, isChopping);
            if (isChopping)
            {
                if (ChoppingIcon == null)
                {
                    ChoppingIcon = iconPattern.New("Chopping Icon");//创建图标
                }
            }
            else
            {
                MonoBehaviour.Destroy(ChoppingIcon);//删除图标
            }
        }


        //地图一旦创建好就会立刻执行，且永远只执行一次
        protected override void OnEnable()
        {
            RefreshChoppingState();
        }

        public override bool CanBeUnjected => false; //树木不可被玩家移除，因为需要玩家砍伐

        protected override void OnDisable()
        {

        }

        protected override void OnCreated()
        {

        }
    }
}
