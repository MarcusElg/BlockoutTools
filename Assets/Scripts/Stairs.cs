using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class Stairs : MonoBehaviour
{

    // Properties
    public float depth = 1;
    public float width = 1;
    public float height = 1;
    public float uvScaling = 1;
    public Vector3 targetPosition = new Vector3(0, 1, 1);

    public void Generate()
    {
        Validate();
        GenerateMesh();
    }

    public void Validate()
    {
        // Validate properties
        depth = Mathf.Clamp(depth, 0.5f, 10);
        width = Mathf.Clamp(width, 0.1f, 30f);
        height = Mathf.Clamp(height, 0.1f, 2.5f);
        uvScaling = Mathf.Clamp(uvScaling, 0.1f, 10f);

        // Prevent scaling
        transform.localScale = Vector3.one;

        // Rotate object towards target
        transform.rotation = Quaternion.LookRotation(Vector3Extensions.ToXZ(targetPosition));
    }

    private void GenerateMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        int segments = (int)(Vector3Extensions.XZDistance(Vector3.zero, targetPosition));

        for (int i = 0; i < segments; i++)
        {
            // Add start vertices
            if (i == 0)
            {
                vertices.Add(Vector3.left * width / 2);
                vertices.Add(Vector3.right * width / 2);
            }

            // Add vertices *: (left side)
            vertices.Add(Vector3.forward * depth * i + Vector3.up * height * (i + 1) + Vector3.left * width / 2); // Top left
            vertices.Add(Vector3.forward * depth * (i + 1) + Vector3.up * height * (i + 1) + Vector3.left * width / 2); // Top right
            vertices.Add(Vector3.forward * depth * (i + 1) + Vector3.up * height * i + Vector3.left * width / 2); // Bottom right

            // Add vetices *: (right side)
            vertices.Add(Vector3.forward * depth * i + Vector3.up * height * (i + 1) + Vector3.right * width / 2); // Top left
            vertices.Add(Vector3.forward * depth * (i + 1) + Vector3.up * height * (i + 1) + Vector3.right * width / 2); // Top right
            vertices.Add(Vector3.forward * depth * (i + 1) + Vector3.up * height * i + Vector3.right * width / 2); // Bottom right

            // Add triangles
            MeshTools.AddSquare(ref triangles, i == 0 ? vertices.Count - 8 : vertices.Count - 10, vertices.Count - 6, vertices.Count - 5, vertices.Count - 4); // Left side
            MeshTools.AddSquare(ref triangles, vertices.Count - 1, vertices.Count - 2, vertices.Count - 3, vertices.Count - 7); // Right side
            MeshTools.AddSquare(ref triangles, vertices.Count - 5, vertices.Count - 6, vertices.Count - 3, vertices.Count - 2); // Top face
            MeshTools.AddSquare(ref triangles, vertices.Count - 7, vertices.Count - 3, vertices.Count - 6, i == 0 ? 0 : vertices.Count - 10); // Front face

            if (i > 0)
            {
                MeshTools.AddSquare(ref triangles, vertices.Count - 7, vertices.Count - 10, vertices.Count - 4, vertices.Count - 1); // Bottom face
            }

            // Add top face
            if (i == segments - 1)
            {
                MeshTools.AddSquare(ref triangles, vertices.Count - 4, vertices.Count - 5, vertices.Count - 2, vertices.Count - 1);
            }

            // Add uvs
            // Add start vertex
            if (i == 0)
            {
                uvs.Add(Vector2.zero);
                uvs.Add(Vector2.zero);
            }

            uvs.Add(Vector2.zero);
            uvs.Add(Vector2.zero);
            uvs.Add(Vector2.zero);

            uvs.Add(Vector2.zero);
            uvs.Add(Vector2.zero);
            uvs.Add(Vector2.zero);
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
