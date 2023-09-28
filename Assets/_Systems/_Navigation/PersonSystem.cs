using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class PersonSystem
{

    [JsonProperty]
    Person[] persons;
    
    public PersonSystem()
    {
        Debug.Log("PersonSystem Created");
        persons = new Person[]{
            new Person("张三"),
            new Person("李四"),
            new Person("赵明"),
        };
    }

}
