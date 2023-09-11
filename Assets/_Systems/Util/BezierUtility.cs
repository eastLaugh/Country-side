
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BezierUtility
{
    public static Vector3 Bezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return (1 - t) * (1 - t) * p0 + 2 * t * (1 - t) * p1 + t * t * p2;
    }

    public static Vector3 CubicBezier(Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3, float t)
    {
        return (1 - t) * (1 - t) * (1 - t) * P0 +
        3 * t * (1 - t) * (1 - t) * P1 +
        3 * t * t * (1 - t) * P2 +
        t * t * t * P3;
    }

    public static Vector3 CubicBezier(Vector3 P0, Vector3 P3, float t)
    {
        Vector3 P1 = P0 + (P3 - P0).Dot2(new Vector3(-0.3f, 0, 0.8f));
        Vector3 P2 = P0 + (P3 - P0).Dot2(new Vector3(0.1f, 0, 1.4f));
        return CubicBezier(P0, P1, P2, P3, t);
    }

    public static IEnumerable<Vector3> CubicBezier(Vector3 P0, Vector3 P3, int num)
    {
        for (float t = 0; t <= 1f; t += 1 / (float)num)
        {
            yield return CubicBezier(P0, P3, t);
        }
    }

    public static Vector3 Dot2(this Vector3 self, Vector3 other)
    {
        return new Vector3(self.x * other.x, self.y * other.y, self.z * other.z);
    }
}
