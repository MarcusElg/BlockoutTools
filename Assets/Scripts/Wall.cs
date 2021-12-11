using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Wall : MonoBehaviour
{

    // Properties
    public float height = 5;

    // Internal
    bool setup = false;

    public void Setup()
    {
        if (!setup)
        {
            GetComponent<MeshFilter>().hideFlags = HideFlags.NotEditable;
            GetComponent<MeshRenderer>().hideFlags = HideFlags.NotEditable;
            setup = true;
        }
    }

    public void Generate()
    {
        Validate();
        GenerateMesh();
    }

    public void Validate()
    {
        SplineContainer splineContainer = GetComponent<SplineContainer>();
        splineContainer.Spline.EditType = SplineType.Linear;
    }

    private void GenerateMesh()
    {
        Spline spline = GetComponent<SplineContainer>().Spline;
        BezierKnot[] knots = spline.ToArray();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < spline.KnotCount; i++)
        {
            // Add vertices
            vertices.Add(knots[i].Position);
            vertices.Add((Vector3)knots[i].Position + Vector3.up * height);

            // Add triangles
            if (i < spline.KnotCount - 1 || spline.Closed)
            {
                int nextIndex = (i + 1) % spline.KnotCount;
                Debug.Log(nextIndex);
                MeshTools.AddSquare(ref triangles, nextIndex * 2, nextIndex * 2 + 1, i * 2, i * 2 + 1);
            }
        }

        CreateMesh(vertices, triangles, uvs);
    }

    private void CreateMesh(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
