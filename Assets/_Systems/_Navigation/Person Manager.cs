using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using static Person;

public class PersonManager : MonoBehaviour
{

    [SerializedDictionary("Person", "Config")]
    public SerializedDictionary<TypeKey, TypeConfig> PersonDatabase = new();

    [Serializable]
    public struct TypeKey
    {
        [TypeToString(typeof(Persons))]
        public string _;
    }

    [Serializable]
    public struct TypeConfig
    {
        public GameObject Prefab;
    }

    public PersonManager current { get; private set; }
    private void Awake()
    {
        current = this;
    }

    private void OnEnable()
    {
        GameManager.OnMapLoaded += OnMapLoaded;
        GameManager.OnMapUnloaded += OnMapUnloaded;
    }


    private void OnDisable()
    {
        GameManager.OnMapLoaded -= OnMapLoaded;
        GameManager.OnMapUnloaded -= OnMapUnloaded;
    }
    Map map;
    PersonSystem personSystem;
    private void OnMapLoaded(Map map)
    {
        this.map = map;
        personSystem = map.PersonSystem;
        personSystem.OnPersonBirth += OnPersonBirth;
    }

    private void OnMapUnloaded()
    {
        this.map = null;
        personSystem = null;
        Father.DestroyAllChildren();
    }

    public Transform Father;
    private void OnPersonBirth(Person person)
    {
        Debug.Log($"Person {person.name} Birth");
        Type type = person.GetType();
        if (PersonDatabase.ContainsKey(new TypeKey { _ = type.Name }))
        {
            TypeConfig config = PersonDatabase[new TypeKey { _ = type.Name }];
            GameObject go = Instantiate(config.Prefab, person.worldPosition, Quaternion.identity, Father);
            go.GetComponent<PersonBehaviour>().person = person;
            go.SetActive(true);

        }
        else
        {
            Debug.LogError($"PersonDatabase未找到 {type.Name}");
        }
    }

    private void OnGUI()
    {
        if (GameManager.DebugMode && GameManager.current.map != null)
        {
            GUILayout.BeginArea(new Rect(500, 0, 400, 600));
            if (GUILayout.Button("随机生成Person"))
            {
                Type[] nested = typeof(Persons).GetNestedTypes();
                Type type = nested[UnityEngine.Random.Range(0, nested.Length)];

                Person p = Activator.CreateInstance(type, UnityEngine.Random.Range(0, 9999).ToString(), map.GetRandomSlot().worldPosition) as Person;

                personSystem.GiveBirthTo(p);
                //Constructor Tip
                new Persons.OrdinaryVillager("Person", Vector3.zero);
            }
            GUILayout.EndArea();
        }
    }
}

