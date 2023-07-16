using static Slot;
using UnityEngine;
public class Tree : MapObject /* , IReject<House>, IReject<Road> */
{
    protected override void OnSlot()
    {

    }

    protected override void Render(GameObject prefab, Transform parent, Map map)
    {
        //base.Render(prefab, parent);
        Vector3 halfCellSize = GameManager.current.grid.cellSize / 2f;
        for (int i = 0; i < Random.Range(1, 5); i++)
        {
            Vector3 offset = new Vector3(Random.Range(-halfCellSize.x, halfCellSize.x), 0, Random.Range(-halfCellSize.z, halfCellSize.z));
            MonoBehaviour.Instantiate(prefab, parent.position + offset, Quaternion.identity, parent);
        }
        
    }
}