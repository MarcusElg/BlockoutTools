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

    public static List<int> PolygonTriangulation(List<Vector3> shapeVertices, ref List<Vector3> meshVertices, ref List<Vector2> meshUvs, bool flipTriangles = false)
    {
        List<int> triangles = new List<int>();
        List<int> vertexIndexes = new List<int>();
        int currentIndex = 0;
        int iterations = 0;

        // Duplicate vertices
        for (int i = 0; i < shapeVertices.Count; i++)
        {
            meshVertices.Add(shapeVertices[i]);
            meshUvs.Add(meshUvs[i]);
        }

        // Populate vertexIndexes
        for (int i = 0; i < shapeVertices.Count; i++)
        {
            vertexIndexes.Add(i);
        }

        // Make sure that it doesn't loop forever
        while (iterations < 1000 && vertexIndexes.Count > 2)
        {
            // Calculate indexes
            if (currentIndex > vertexIndexes.Count - 1)
            {
                currentIndex = 0;
            }

            int nextIndex;
            if (currentIndex >= vertexIndexes.Count - 1)
            {
                nextIndex = 0;
            }
            else
            {
                nextIndex = currentIndex + 1;
            }

            int previousIndex;
            if (currentIndex <= 0)
            {
                previousIndex = vertexIndexes.Count - 1;
            }
            else
            {
                previousIndex = currentIndex - 1;
            }

            // Test if it's an ear
            bool ear = true;
            for (int j = 0; j < shapeVertices.Count; j++)
            {
                // Don't test for vertices in triangle
                if (j == vertexIndexes[previousIndex] || j == vertexIndexes[currentIndex] || j == vertexIndexes[nextIndex])
                {
                    continue;
                }

                // Test if the point is inside triangle
                if (PointInTriangle(Vector3Extensions.To2DXZ(shapeVertices[vertexIndexes[currentIndex]]), Vector3Extensions.To2DXZ(shapeVertices[vertexIndexes[previousIndex]]), Vector3Extensions.To2DXZ(shapeVertices[vertexIndexes[nextIndex]]), Vector3Extensions.To2DXZ(shapeVertices[j])))
                {
                    ear = false;
                    break;
                }
            }

            if (ear)
            {
                for (float t = 0; t <= 1; t += 0.25f)
                {
                    if (!PointInPolygon(shapeVertices, Vector3.Lerp(shapeVertices[vertexIndexes[previousIndex]], shapeVertices[vertexIndexes[nextIndex]], 0.5f)))
                    {
                        ear = false;
                        break;
                    }
                }
            }

            if (ear)
            {
                // Add triangle
                if (flipTriangles)
                {
                    MeshTools.AddTriangle(ref triangles, meshVertices.Count - shapeVertices.Count + vertexIndexes[nextIndex],
                        meshVertices.Count - shapeVertices.Count + vertexIndexes[currentIndex], meshVertices.Count - shapeVertices.Count + vertexIndexes[previousIndex]);
                }
                else
                {
                    MeshTools.AddTriangle(ref triangles, meshVertices.Count - shapeVertices.Count + vertexIndexes[previousIndex],
                        meshVertices.Count - shapeVertices.Count + vertexIndexes[currentIndex], meshVertices.Count - shapeVertices.Count + vertexIndexes[nextIndex]);
                }

                // Remove from list
                vertexIndexes.RemoveAt(currentIndex);

                // If this was a triangle then there can't be a triangle centered around next point
                currentIndex++;
            }

            currentIndex++;
            iterations++;
        }

        return triangles;
    }

    // Source: http://alienryderflex.com/polygon/
    public static bool PointInPolygon(List<Vector3> polygons, Vector3 point)
    {
        int j = polygons.Count - 1;
        bool oddNodes = false;

        for (int i = 0; i < polygons.Count; i++)
        {
            if ((polygons[i].z < point.z && polygons[j].z >= point.z
            || polygons[j].z < point.z && polygons[i].z >= point.z)
            && (polygons[i].x <= point.x || polygons[j].x <= point.x))
            {
                oddNodes ^= (polygons[i].x + (point.z - polygons[i].z) / (polygons[j].z - polygons[i].z) * (polygons[j].x - polygons[i].x) < point.x);
            }
            j = i;
        }

        return oddNodes;
    }

    // Source: https://github.com/SebLague/Ear-Clipping-Triangulation/blob/master/Scripts/Maths2D.cs
    public static bool PointInTriangle(Vector2 a, Vector2 b, Vector2 c, Vector2 p)
    {
        float area = 0.5f * (-b.y * c.x + a.y * (-b.x + c.x) + a.x * (b.y - c.y) + b.x * c.y);
        float s = 1 / (2 * area) * (a.y * c.x - a.x * c.y + (c.y - a.y) * p.x + (a.x - c.x) * p.y);
        float t = 1 / (2 * area) * (a.x * b.y - a.y * b.x + (a.y - b.y) * p.x + (b.x - a.x) * p.y);
        return s >= 0 && t >= 0 && (s + t) <= 1;
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
