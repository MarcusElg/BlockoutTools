using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Wall : MonoBehaviour
{

    // Properties
    public float height = 5;
    public float thickness = 1;
    public float uvScaling = 1;

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

        float currentDistance = 0.02f; // Not zero as that creates flat texture on cap

        for (int i = 0; i < spline.KnotCount; i++)
        {
            Vector3 left = (Quaternion)knots[i].Rotation * Vector3Extensions.GetInwardsFromTangents(((Vector3)knots[i].TangentIn).normalized, ((Vector3)knots[i].TangentOut).normalized);
            int nextIndex = (i + 1) % spline.KnotCount;

            if (spline.Closed)
            {
                nextIndex = (i + 1) % (spline.KnotCount + 1); // Extra vertex at end
            }

            // Add vertices ::
            Vector3 leftPosition = (Vector3)knots[i].Position + left * thickness;
            Vector3 rightPosition = (Vector3)knots[i].Position - left * thickness;
            vertices.Add(leftPosition); // Lower left
            vertices.Add(rightPosition); // Lower right
            vertices.Add(rightPosition + Vector3.up * height); // Upper right
            vertices.Add(leftPosition + Vector3.up * height); // Upper left

            // Add triangles =
            if (i < spline.KnotCount - 1 || spline.Closed)
            {
                MeshTools.ConnectToNextIteration(ref triangles, i, nextIndex, 4);
            }

            // Add cap triangles
            if (!spline.Closed && (i == 0 || i == spline.KnotCount - 1))
            {
                if (i == 0)
                {
                    MeshTools.AddFollowingSquare(ref triangles, i * 4);
                }
                else
                {
                    MeshTools.AddPreceedingSquare(ref triangles, i * 4 + 3);
                }
            }

            // Add uvs
            uvs.Add(new Vector2(currentDistance, 2 * thickness) / uvScaling);
            uvs.Add(new Vector2(currentDistance, 0) / uvScaling);
            uvs.Add(new Vector2(currentDistance, height) / uvScaling);
            uvs.Add(new Vector2(currentDistance, height + 2 * thickness) / uvScaling);

            // Update for next iteration
            currentDistance += Vector3.Distance(knots[i].Position, knots[(i + 1) % spline.KnotCount].Position);
        }

        if (spline.Closed)
        {
            // Add extra vertices to prevent uv stretching
            vertices.AddRange(vertices.GetRange(0, 4));

            // Add extra uvs
            uvs.Add(new Vector2(currentDistance, 2 * thickness) / uvScaling);
            uvs.Add(new Vector2(currentDistance, 0) / uvScaling);
            uvs.Add(new Vector2(currentDistance, height) / uvScaling);
            uvs.Add(new Vector2(currentDistance, height + 2 * thickness) / uvScaling);
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
