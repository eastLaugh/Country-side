using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapObjects;

public class RoadRenderer : MonoBehaviour
{
    public static RoadRenderer Instance { get; private set; }
    public Transform SlotParent;
    private void Awake()
    {
        Instance = this;
    }

    public void Connect(Road road1, Road road2)
    {
        Debug.Log(road1.slot.worldPosition + " " + road2.slot.worldPosition);
        var pos1 = road1.slot.worldPosition;
        var pos2 = road2.slot.worldPosition;
        pos1.y = pos2.y = 0.1f;

        LineRenderer lineRenderer = new GameObject().AddComponent<LineRenderer>();
        lineRenderer.transform.SetParent(transform);
        lineRenderer.SetPositions(new Vector3[] { pos1, pos2 });
        lineRenderer.SetWidth(0.1f,0.1f);
    }
}
