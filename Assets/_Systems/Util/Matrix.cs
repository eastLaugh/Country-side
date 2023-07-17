using UnityEngine;
using System.Collections.Generic;


[System.Obsolete("简单矩阵。若要更高级功能请用Matrix4x4（unity api）")]
public static class MartixExtension
{
    //注：Vector[]是“列”向量的集合

    //对一个三维向量左乘矩阵A
    public static Vector3 LeftMultiple(this Vector3 X, Vector3[] A) => X.x * A[0] + X.y * A[1] + X.z * A[2];
    //对一个二维向量左乘矩阵A
    public static Vector2 LeftMultiple(this Vector2 X, Vector2[] A) => LeftMultiple(X, new Vector3[] { A[0], A[1], Vector3.zero });

    public static readonly Vector3[] IdentityMartix3 = new Vector3[] { Vector3.right, Vector3.up, Vector3.forward };

    public static readonly Vector2[] IdentityMartix2 = new Vector2[] { Vector2.right, Vector2.up };
}


public static class SystemRandomExtension
{
    public static Vector3 NextVector3(this System.Random random, float min, float max)
    {
        return new Vector3(random.NextFloat(min, max), random.NextFloat(min, max), random.NextFloat(min, max));
    }
    public static Vector3 NextVector3(this System.Random random, Vector3 min, Vector3 max)
    {
        return new Vector3(random.NextFloat(min.x, max.x), random.NextFloat(min.y, max.y), random.NextFloat(min.z, max.z));
    }
    public static float NextFloat(this System.Random random, float min, float max)
    {
        return (float)random.NextDouble() * (max - min) + min;
    }
    public static float NextFloat(this System.Random random, Vector2 min, Vector2 max)
    {
        return (float)random.NextDouble() * (max - min).magnitude + min.magnitude;
    }
    public static Vector2 NextVector2(this System.Random random, float min, float max)
    {
        return new Vector2(random.NextFloat(min, max), random.NextFloat(min, max));
    }
    public static Vector2 NextVector2(this System.Random random, Vector2 min, Vector2 max)
    {
        return new Vector2(random.NextFloat(min.x, max.x), random.NextFloat(min.y, max.y));
    }
    public static Vector2 NextVector2(this System.Random random, Vector2 max)
    {
        return new Vector2(random.NextFloat(0, max.x), random.NextFloat(0, max.y));
    }
    public static Vector2 NextVector2(this System.Random random, float max)
    {
        return new Vector2(random.NextFloat(0, max), random.NextFloat(0, max));
    }
    public static Vector3 NextVector3(this System.Random random, Vector3 max)
    {
        return new Vector3(random.NextFloat(0, max.x), random.NextFloat(0, max.y), random.NextFloat(0, max.z));
    }
    public static Vector3 NextVector3(this System.Random random, float max)
    {
        return new Vector3(random.NextFloat(0, max), random.NextFloat(0, max), random.NextFloat(0, max));
    }
    public static Vector3 NextVector3(this System.Random random)
    {
        return new Vector3(random.NextFloat(0, 1), random.NextFloat(0, 1), random.NextFloat(0, 1));
    }
}