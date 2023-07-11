using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selection : MonoBehaviour
{
    public static Selection current;
    private void Start()
    {
        current = this;
    }

    private ICanBeClicked[] LastClicked;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            (GameObject @object, bool err) = MouseRayCast();
            if (err)
            {
                // Selection.Select(null, shiftclick: Input.GetKey(KeyCode.LeftShift), rightclick: Input.GetMouseButtonDown(1));
            }
            else
            {
                // Selection.Select(@object, shiftclick: Input.GetKey(KeyCode.LeftShift), rightclick: Input.GetMouseButtonDown(1));
            }
        }

    }

    (GameObject, bool) MouseRayCast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            GameObject hitted = hit.collider.gameObject;
            return (hitted, false);
        }
        else
        {
            return (null, true);
        }
    }
}
