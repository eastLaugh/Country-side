using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "illuBook Database", menuName = "Country side/Database/illuBook", order = 0)]
public class illuBookData_SO : ScriptableObject
{
    public List<BuildingDetails> illuBookList = new List<BuildingDetails>();
}
