using System.Collections.Generic;

public static class MeshTools
{
    public static void AddTriangle(ref List<int> triangles, int indexOne, int indexTwo, int indexThree)
    {
        triangles.Add(indexOne);
        triangles.Add(indexTwo);
        triangles.Add(indexThree);
    }

    public static void AddSquare(ref List<int> triangles, int indexOne, int indexTwo, int indexThree, int indexFour)
    {
        AddTriangle(ref triangles, indexOne, indexTwo, indexThree);
        AddTriangle(ref triangles, indexTwo, indexFour, indexThree);
    }
}
