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
        public PersonSystem()
        {

        }

        public void GiveBirthTo(Person p)
        {
            persons.Add(p);
            p.OnCreated();
            EnablePerson(p);
        }

        void EnablePerson(Person p)
        {
            PersonManager.current.OnPersonBirth(p);
            p.OnEnable();
        }

        [OnDeserialized]
        void OnDeserializedMethod(System.Runtime.Serialization.StreamingContext context)
        {
            //反序列化完成回调
            foreach (var p in persons)
            {
                EnablePerson(p);
            }
        }
    }
}