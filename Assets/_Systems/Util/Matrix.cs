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