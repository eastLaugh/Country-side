using UnityEngine;
using static Slot;

public static partial class MapObjects
{
    public class House : MapObject, MustNotExist<House>
    {

        protected override GameObject[] Render(GameObject prefab, GameObject[] prefabs, SlotRender slotRender)
        {
            IconPattern iconPattern = IconPattern.Create(father, Vector3.zero);
            iconPattern.New("Building Icon");

            return base.Render(prefab, prefabs, slotRender);
        }

        protected override void Awake()
        {
        }

        protected override void OnDisable()
        {

        }

        protected override void OnCreated()
        {
        }

        public override bool CanBeUnjected => true;
    }

    public class Road : MapObject
    {
        public override bool CanBeUnjected => true;

        protected override void Awake()
        {
        }

        protected override void OnDisable()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnCreated()
        {
            throw new System.NotImplementedException();
        }
    }

}