using static Slot;
using UnityEngine;
using DG.Tweening;
using Newtonsoft.Json;

public class Tree : MapObject /* , IReject<House>, IReject<Road> */
{
    public Tree(int AppearanceSeed) : base(AppearanceSeed)
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
            trees[i] = MonoBehaviour.Instantiate(prefabs[random.Next(0, prefabs.Length)], slotRender.transform.position + offset, Quaternion.identity, father);
            trees[i].transform.DOScale(Vector3.zero, Settings.建筑时物体缓动持续时间).From().SetEase(Ease.OutBack);
        }

        iconPattern = IconPattern.Create(father, Vector3.zero);
        return trees;
    }

    IconPattern iconPattern;
    protected override void OnClick()
    {
        base.OnClick();

        iconPattern.New("Chopping Icon");
    }
}