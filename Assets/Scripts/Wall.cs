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
    public List<Vector3> wallCenterPositions = new List<Vector3>();
    public List<Vector3> wallCenterLefts = new List<Vector3>();

    public void Setup()
    {
        if (!setup)
        {
            GetComponent<MeshFilter>().hideFlags = HideFlags.NotEditable;
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
        // Validate properties
        height = Mathf.Clamp(height, 0.5f, 10);
        thickness = Mathf.Clamp(thickness, 0.1f, 2.5f);
        uvScaling = Mathf.Clamp(uvScaling, 0.1f, 10f);

        // Validate spline
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

        // Don't try generate for less than 2 spline knots
        if (spline.Count < 2)
        {
            CreateMesh(vertices, triangles, uvs);
            wallCenterPositions.Clear();
            wallCenterLefts.Clear();
            return;
        }

        float currentDistance = 0.02f; // Not zero as that creates flat texture on cap
        bool shouldClose = spline.Closed && spline.Count >= 2;

        GenerateWallCenterPositions(knots, shouldClose);

        for (int i = 0; i < spline.Count; i++)
        {
            Vector3 left = (Quaternion)knots[i].Rotation * Vector3Extensions.GetInwardsFromTangents(((Vector3)knots[i].TangentIn).normalized, ((Vector3)knots[i].TangentOut).normalized);
            int nextIndex = (i + 1) % spline.Count;

            if (shouldClose)
            {
                nextIndex = (i + 1) % (spline.Count + 1); // Extra vertex at end
            }

            // Add vertices ::
            Vector3 leftPosition = (Vector3)knots[i].Position + left * thickness;
            Vector3 rightPosition = (Vector3)knots[i].Position - left * thickness;
            vertices.Add(leftPosition); // Lower left
            vertices.Add(rightPosition); // Lower right
            vertices.Add(rightPosition + Vector3.up * height); // Upper right
            vertices.Add(leftPosition + Vector3.up * height); // Upper left

            // Add triangles =
            if (i < spline.Count - 1 || shouldClose)
            {
                MeshTools.ConnectToNextIteration(ref triangles, i, nextIndex, 4);
            }

            // Add cap triangles
            if (!shouldClose && (i == 0 || i == spline.Count - 1))
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
            currentDistance += Vector3.Distance(knots[i].Position, knots[(i + 1) % spline.Count].Position);
        }

        if (shouldClose)
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

    private void GenerateWallCenterPositions(BezierKnot[] knots, bool shouldClose)
    {
        wallCenterPositions.Clear();
        wallCenterLefts.Clear();

        int endIndex = shouldClose ? knots.Length : knots.Length - 1;
        for (int i = 0; i < endIndex; i++)
        {
            Vector3 currentKnot = transform.TransformPoint(knots[i].Position);
            Vector3 nextKnot = transform.TransformPoint(knots[(i + 1) % (knots.Length)].Position);
            wallCenterPositions.Add((currentKnot + nextKnot) / 2);

            Vector3 left = Vector3Extensions.LeftFromForward(nextKnot - currentKnot);
            wallCenterLefts.Add(left);
        }
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
