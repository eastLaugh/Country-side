using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
public class ArrowRender : MonoBehaviour
{
    public Transform father;
    static ArrowRender Ovary = null;
    private void Awake()
    {
        if (Ovary == null)
        {
            Ovary = this;
        }
    }
    public static ArrowRender NewArrow(Vector3 from, Vector3 target)
    {
        ArrowRender arrowRender = Instantiate(Ovary.gameObject, Ovary.father).GetComponent<ArrowRender>();
        arrowRender.Point(from, target);
        return arrowRender;
    }
    LineRenderer Point(Vector3 from, Vector3 target)
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        var bezierPoints = BezierUtility.CubicBezier(from, target, 50).ToArray();
        lineRenderer.positionCount = bezierPoints.Length;
        lineRenderer.SetPositions(bezierPoints);
        return lineRenderer;
    }
}