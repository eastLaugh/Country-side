using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Mouse : MonoBehaviour
{
    public static event Action<IClickable> OnSelectionChange;
    public GameObject mouseIndicator;
    public GameManager gameManager;

    HashSet<IClickable> Selections = new();
    void Update()
    {
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            mouseIndicator.transform.position = new Vector3(hitPoint.x, 0.3f, hitPoint.z);
            ;
        }


        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            (GameObject @object, bool err) = MouseRayCast();
            if (err)
            {
                foreach(var clickable in Selections){
                    clickable.UnClick();
                }
                Selections = new();
                OnSelectionChange?.Invoke(null);
            }
            else
            {
                //Debug.Log($"点选{@object}");
                IClickable clickable = @object.GetComponent<IClickable>();
                if (clickable != null && !Selections.Contains(clickable))
                {
                    clickable.OnClick();
                    Selections.Add(clickable);
                    
                    OnSelectionChange?.Invoke(clickable);
                }

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

    private void Start()
    {
        OnSelectionChange?.Invoke(null);
    }
}
