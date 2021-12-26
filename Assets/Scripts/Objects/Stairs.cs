using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class Stairs : MonoBehaviour
{

    // Properties
    public float depth = 0.25f;
    public float width = 3;
    public float height = 0.25f;
    public Vector3 targetPosition = new Vector3(3, 1, 0);

    public void Generate()
    {
        Validate();
        GenerateMesh();
    }

    public void Validate()
    {
        // Validate properties
        depth = Mathf.Clamp(depth, 0.15f, 3f);
        width = Mathf.Clamp(width, 1f, 30f);
        height = Mathf.Clamp(height, 0.15f, 1f);

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

        int segments = (int)(Vector3Extensions.XZDistance(Vector3.zero, targetPosition) / depth);
        AddStairSide(ref vertices, ref triangles, ref uvs, segments, Vector3.left, 0); // Left side
        AddStairSide(ref vertices, ref triangles, ref uvs, segments, Vector3.right, segments * 3 + 1); // Right side
        MeshTools.ConnectToNextIteration(ref triangles, 0, 1, vertices.Count / 2); // Create top/bottom faces
        MeshTools.CreateMesh(gameObject, vertices, triangles, uvs);
    }

    private void AddStairSide(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uvs, int segments, Vector3 left, int offset)
    {
        // Top part
        for (int i = 0; i < segments; i++)
        {
            // Add start vertex .
            if (i == 0)
            {
                vertices.Add(left * width / 2);
            }

            // Add top vertices :*
            vertices.Add(Vector3.forward * depth * i + Vector3.up * height * (i + 1) + left * width / 2); // Top left
            vertices.Add(Vector3.forward * depth * (i + 1) + Vector3.up * height * (i + 1) + left * width / 2); // Top right

            // Add uvs
            // Add start vertex
            if (i == 0)
            {
                uvs.Add(Vector2.zero);
            }

            uvs.Add(Vector2.zero);
            uvs.Add(Vector2.zero);
            uvs.Add(Vector2.zero);
        }

        // Bottom part
        for (int i = segments - 1; i >= 0; i--)
        {
            vertices.Add(Vector3.forward * depth * (i + 1) + Vector3.up * height * i + left * width / 2); // Bottom right vertex ::

            if (offset == 0)
            {
                // Normal in left direction
                MeshTools.AddSquare(ref triangles, offset + i * 2, offset + i * 2 + 1, vertices.Count - 2, vertices.Count - 1);
            }
            else
            {
                // Normal in right direction
                MeshTools.AddSquare(ref triangles, vertices.Count - 1, vertices.Count - 2, offset + i * 2 + 1, offset + i * 2);
            }
        }
    }
}