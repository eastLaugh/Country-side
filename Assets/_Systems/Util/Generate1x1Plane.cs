#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class Generate1x1Plane : MonoBehaviour
{
    //public Vector3Int size=new Vector3Int(1,0,1);

    [NaughtyAttributes.Button]
    private void CreateAndSaveMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-0.5f, 0f, -0.5f),
            new Vector3(0.5f, 0f, -0.5f),
            new Vector3(-0.5f, 0f, 0.5f),
            new Vector3(0.5f, 0f, 0.5f)
        };

        int[] triangles = new int[6] { 0, 2, 1, 2, 3, 1 };

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f)
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        //Selection.activeObject = mesh;
        AssetDatabase.CreateAsset(mesh, "Assets/1x1Plane.asset");
        AssetDatabase.SaveAssets();

    }
}
#endif