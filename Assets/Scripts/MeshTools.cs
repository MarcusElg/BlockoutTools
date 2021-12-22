using System.Collections.Generic;
using UnityEngine;

public static class MeshTools
{
    public static void AddTriangle(ref List<int> triangles, int indexOne, int indexTwo, int indexThree)
    {
        triangles.Add(indexOne);
        triangles.Add(indexTwo);
        triangles.Add(indexThree);
    }

    // Uses a index and the following two to create a triangle
    public static void AddFollowingTriangle(ref List<int> triangles, int startIndex)
    {
        AddTriangle(ref triangles, startIndex, startIndex + 1, startIndex + 2);
    }

    // Uses a index and the following three to create a square
    public static void AddFollowingSquare(ref List<int> triangles, int startIndex)
    {
        AddSquare(ref triangles, startIndex, startIndex + 1, startIndex + 2, startIndex + 3);
    }

    // Uses a index and the preceeding three to create a square
    public static void AddPreceedingSquare(ref List<int> triangles, int startIndex)
    {
        AddSquare(ref triangles, startIndex, startIndex - 1, startIndex - 2, startIndex - 3);
    }

    public static void AddSquare(ref List<int> triangles, int indexOne, int indexTwo, int indexThree, int indexFour)
    {
        AddTriangle(ref triangles, indexOne, indexThree, indexTwo);
        AddTriangle(ref triangles, indexOne, indexFour, indexThree);
    }

    public static void ConnectToNextIteration(ref List<int> triangles, int currentIndex, int nextIndex, int countPerIteration)
    {
        for (int i = 0; i < countPerIteration; i++)
        {
            int nextI = (i + 1) % countPerIteration;
            AddSquare(ref triangles, nextIndex * countPerIteration + i, nextIndex * countPerIteration + nextI, currentIndex * countPerIteration + nextI, currentIndex * countPerIteration + i);
        }
    }

    public static void CreateMesh(GameObject gameObject, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        // Flat shaded triangles
        Vector3[] flatShadedVertices = new Vector3[triangles.Count];
        Vector2[] flatShadedUvs = new Vector2[triangles.Count];

        for (int i = 0; i < triangles.Count; i++)
        {
            flatShadedVertices[i] = vertices[triangles[i]];
            flatShadedUvs[i] = uvs[triangles[i]];

            triangles[i] = i;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = flatShadedVertices;
        mesh.triangles = triangles.ToArray();
        mesh.uv = flatShadedUvs;
        mesh.RecalculateNormals();

        gameObject.GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
