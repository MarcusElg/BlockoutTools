using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class Stairs : MonoBehaviour
{

    public enum Type
    {
        Straight,
        Spiral
    }

    // Properties
    public Type type;
    public float depth = 0.25f;
    public float innerRadius = 0.1f;
    public bool rotateClockwise = true;
    public float width = 3;
    public float height = 0.25f;
    public Vector3 targetPosition = new Vector3(3, 1, 0);
    public float targetRotation = 0f;

    public void Generate()
    {
        Validate();
        GenerateMesh();
    }

    public void Validate()
    {
        // Validate properties
        depth = Mathf.Clamp(depth, 0.15f, 3f);
        innerRadius = Mathf.Clamp(innerRadius, 0f, 1f);
        width = Mathf.Clamp(width, 1f, 30f);
        height = Mathf.Clamp(height, 0.15f, 1f);
        targetRotation = Mathf.Clamp(targetRotation, 0, 1800f); // 5 turns

        // Prevent scaling
        transform.localScale = Vector3.one;

        // Rotate object towards target
        if (type == Type.Straight)
        {
            transform.rotation = Quaternion.LookRotation(Vector3Extensions.ToXZ(targetPosition));
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
    }

    private void GenerateMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        int segments = (int)(Vector3Extensions.XZDistance(Vector3.zero, targetPosition) / depth);
        float actualDepth = Vector3Extensions.XZDistance(Vector3.zero, targetPosition) / segments;
        AddStairSide(ref vertices, ref triangles, ref uvs, segments, actualDepth, Vector3.left, 0); // Left side
        AddStairSide(ref vertices, ref triangles, ref uvs, segments, actualDepth, Vector3.right, segments * 3 + 1); // Right side
        MeshTools.ConnectToNextIteration(ref triangles, 0, 1, vertices.Count / 2); // Create top/bottom faces
        MeshTools.CreateMesh(gameObject, vertices, triangles, uvs);
    }

    private void AddStairSide(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uvs, int segments, float actualDepth, Vector3 left, int offset)
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
            vertices.Add(Vector3.forward * actualDepth * i + Vector3.up * height * (i + 1) + left * width / 2); // Top left
            vertices.Add(Vector3.forward * actualDepth * (i + 1) + Vector3.up * height * (i + 1) + left * width / 2); // Top right

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
            vertices.Add(Vector3.forward * actualDepth * (i + 1) + Vector3.up * height * i + left * width / 2); // Bottom right vertex ::

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
