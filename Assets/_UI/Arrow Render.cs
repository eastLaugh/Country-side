using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
public class ArrowRender : MonoBehaviour
{
    public GameObject EconomyArrowPrefab;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Point(Vector3 from, Vector3 target)
    {
        LineRenderer lineRenderer = new GameObject("Arrow").AddComponent<LineRenderer>();
        Arrow arrow = lineRenderer.AddComponent<Arrow>();
        lineRenderer.transform.SetParent(transform);

        var bezierPoints = BezierUtility.CubicBezier(from, target, 50).ToArray();
        lineRenderer.positionCount = bezierPoints.Length;
        lineRenderer.SetPositions(bezierPoints);
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        lineRenderer.colorGradient = new Gradient()
        {
            colorKeys = new GradientColorKey[] {
                new GradientColorKey(Color.yellow, 0),
                new GradientColorKey(Color.yellow, 1)
            }
        };

        //lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    [ContextMenu("Test")]
    void Test()
    {
        Point(Vector3.zero, new Vector3(6, 0, 6));
    }
}
public class Arrow : MonoBehaviour
{

}