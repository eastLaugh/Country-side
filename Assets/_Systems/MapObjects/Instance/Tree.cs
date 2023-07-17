using static Slot;
using UnityEngine;
using DG.Tweening;
using Newtonsoft.Json;

public class Tree : MapObject /* , IReject<House>, IReject<Road> */
{
    [JsonConstructor]
    public Tree(int AppearanceSeed) : base(AppearanceSeed)
    {
    }

    public Tree() : base(-1)
    {
    }
    protected override void OnSlot()
    {

    }

    protected override GameObject[] Render(GameObject prefab, GameObject[] prefabs, SlotRender slotRender)
    {
        // base.Render(prefab, prefabs, slotRender);
        Vector3 halfCellSize = GameManager.current.grid.cellSize / 2f;
        var trees = new GameObject[random.Next(1, 5)];
        for (int i = 0; i < trees.Length; i++)
        {
            Vector3 offset = new Vector3(random.NextFloat(-halfCellSize.x, halfCellSize.x), 0, random.NextFloat(-halfCellSize.z, halfCellSize.z));
            trees[i] = MonoBehaviour.Instantiate(prefabs[random.Next(0, prefabs.Length)], slotRender.transform.position + offset, Quaternion.identity, slotRender.transform);
            trees[i].transform.DOScale(Vector3.zero, Enums.建筑时物体缓动持续时间).From().SetEase(Ease.OutBack);
        }
        return trees;
    }
}