using System;
using System.Collections;
using System.Collections.Generic;
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
    }
}