using UnityEngine;

internal class Persons
{
    public class OrdinaryVillager : Person, Person.IPromptProvider
    {
        public OrdinaryVillager(string name, Vector3 worldPosition) : base(name, worldPosition)
        {
        }

        public string GetPrompt()
        {
            return $"本次模拟中，你是一名村民。";

        }

        protected override void OnCreated()
        {
        }

        protected override void OnEnable()
        {
        }
    }

    public class Headman : Person, Person.IPromptProvider
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

        public string GetPrompt()
        {
            return $"本次模拟中，你是村长，名为{name}。";
        }
    }
}

