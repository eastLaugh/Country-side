using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

public partial class Person
{
    public class PersonSystem
    {

        [JsonProperty]
        public readonly List<Person> persons = new();
        public event Action<Person> OnPersonBirth;
        public PersonSystem()
        {

        }

        public void GiveBirthTo(Person p)
        {
            persons.Add(p);
            OnPersonBirth?.Invoke(p);
        }

        [OnDeserialized]
        void OnDeserializedMethod(System.Runtime.Serialization.StreamingContext context)
        {
            //反序列化完成回调
            GameManager.OnMapLoaded += OnMapLoaded;
        }

        private void OnMapLoaded(Map map)
        {
            foreach (var p in persons)
            {
                OnPersonBirth?.Invoke(p);
            }
            GameManager.OnMapLoaded -= OnMapLoaded;
        }
    }
}