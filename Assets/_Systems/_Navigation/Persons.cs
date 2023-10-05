using UnityEngine;

internal class Persons
{
    public class OrdinaryVillager : Person
    {
        public OrdinaryVillager(string name, Vector3 worldPosition) : base(name, worldPosition)
        {
        }

        protected override void OnCreated()
        {
        }

        protected override void OnEnable()
        {
        }
    }

    public class Headman : Person
    {
        public static Headman instance { get; private set; }
        public Headman(string name, Vector3 worldPosition) : base(name, worldPosition)
        {
        }

        protected override void OnCreated()
        {
        }

        protected override void OnEnable()
        {
            instance = this;
        }
    }
}