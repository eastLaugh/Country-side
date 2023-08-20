using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
partial class MapObjects
{
    public class Lake : Slot.MapObject, MustNotExist<Lake>, IInfoProvider
    {
        public override bool CanBeUnjected => false; 

        [JsonProperty]
        LakeEcosystem lakeEcosystem;

        [JsonProperty]
        HashSet<Vector2> CoastlineDirections;
        //地图被创建时才会被加载。读取本地存档时不会加载OnCreated();
        protected override void OnCreated()
        {
            if (lakeEcosystem == null)
            {

                LakeEcosystem lakeEcosystem = new(map);

                DFS(this);

                //用DFS查找湖泊的连通图
                void DFS(Lake node)
                {
                    lakeEcosystem.Inject(node);

                    HashSet<Vector2> CoastlineDirections = new();
                    List<Lake> targetLakes = new();

                    for (int i = 0; i < Slot.AllDirections.Length; i++)
                    {
                        var dir = Slot.AllDirections[i];
                        Slot target = map[node.slot.position + dir];
                        if (target != null)
                        {
                            Lake targetLake = target.GetMapObject<Lake>();
                            if (targetLake != null)
                            {
                                if (targetLake.lakeEcosystem == null)
                                {
                                    targetLakes.Add(targetLake);
                                }
                            }
                            else
                            {
                                CoastlineDirections.Add(dir);
                            }
                        }
                        else
                        {
                            CoastlineDirections.Add(dir);
                        }

                    }
                    node.CoastlineDirections = CoastlineDirections;

                    //
                    foreach (var targetLake in targetLakes)
                    {
                        DFS(targetLake);
                    }
                }
            }

        }
        protected override void OnDisable()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnEnable()
        {

        }

        public void ProvideInfo(System.Action<string> provide)
        {
            provide(lakeEcosystem.name);
            if (CoastlineDirections.Count > 0)
            {
                provide(" 岸线方向：");
                foreach (var dir in CoastlineDirections)
                {
                    provide(dir.ToString());
                }
            }

        }

        public class LakeEcosystem
        {

            static readonly string[] LakeEcosystemNameDataBase = {
"洞庭湖", "太湖", "汾湖", "珠江", "贝加尔湖", "密西西比河",
"巴尔喀什湖", "维多利亚湖", "死海", "马尔地夫湖", "尼亚萨湖",
"鄂博湖", "鄱阳湖", "图尔恰湖", "切德湖", "咸海", "阿拉尔湖",
"苏必利尔湖", "胡德逊湾", "巴基尔湖", "尼罗河", "贾纳湖", "北冰洋",
"加利福尼亚湾", "马拉维湖", "马来西亚湾", "萨格勒布湖", "昆仑湖",
"维尔纽斯湖", "芬兰湾", "阿尔卑斯湖", "查尔斯河", "大熊湖", "维尔德湖",
"巴拉顿湖", "奥克拉荷马湖", "曼彻斯特湖", "班戈湖", "布拉干萨河",
"湄公河", "尼日尔河", "冈比亚河", "黑尔格达河", "塞纳河", "尼加拉瓜湖",
"贝尔湖", "博伊拉湖", "萨马拉湖", "安曼湖", "巴都湖", "比亚湖",
"博尔诺湖", "彼得湖", "阿尔瓦湖", "埃德湖", "科罗曼德尔湖", "阿亚湖",
"奥哈瓦利湖", "雷西亚湖", "巴尔喀什湖", "博多湖", "拉东加湖", "埃比湖"
// 继续添加更多的湖泊名称...
};
            public string name;
            [JsonProperty]
            public Map map { get; private set; }

            [JsonProperty]
            public HashSet<Lake> lakes { get; private set; } = new();

            public List<Vector2> vertices = new();

            public void Inject(Lake lake)
            {
                if (lake.map == map)
                {
                    lakes.Add(lake);
                    lake.lakeEcosystem = this;
                }
            }

            public LakeEcosystem(Map map)
            {
                Debug.Log("LakeEcosystem有参构造函数");
                this.map = map;
                map.lakeEcosystems.Add(this);
                name = LakeEcosystemNameDataBase[Random.Range(0, LakeEcosystemNameDataBase.Length)];
            }

            [JsonConstructor]
            public LakeEcosystem()
            {
                Debug.Log("LakeEcosystem无参构造函数");
            }


        }


    }
}