using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BlockoutTools
{
    [RequireComponent(typeof(SplineContainer), typeof(MeshRenderer), typeof(MeshFilter))]
    public class Floor : MonoBehaviour
    {

        // Properties
        public float thickness = 0.25f;

        // Internal
        public Vector3 centerPosition;

        public void Generate()
        {
            Validate();
            GenerateMesh();

            // Save changes
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        public void Validate()
        {
            // Validate properties
            thickness = Mathf.Clamp(thickness, 0.1f, 1f);

            // Validate spline
            SplineContainer splineContainer = GetComponent<SplineContainer>();
            splineContainer.Spline.EditType = SplineType.Linear;
            splineContainer.Spline.Closed = false;

            // Prevent scaling
            transform.localScale = Vector3.one;
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
                MeshTools.CreateMesh(gameObject, vertices, triangles, uvs);
                centerPosition = Vector3.zero;
                return;
            }

            CalculateCenterPosition(knots);

            // Add bottom vertices
            for (int i = 0; i < spline.Count; i++)
            {
                vertices.Add(spline[i].Position);
                uvs.Add(Vector2.zero);
            }

            // Add top vertices
            for (int i = 0; i < spline.Count; i++)
            {
                vertices.Add((Vector3)spline[i].Position + Vector3.up * thickness);
                uvs.Add(Vector2.zero);
            }

            MeshTools.ConnectToNextIteration(ref triangles, 0, 1, vertices.Count / 2); // Create sides
            int vertexCount = vertices.Count;
            triangles.AddRange(MeshTools.PolygonTriangulation(vertices.GetRange(0, vertexCount / 2), 0, true)); // Create bottom
            triangles.AddRange(MeshTools.PolygonTriangulation(vertices.GetRange(vertexCount / 2, vertexCount / 2), vertexCount / 2)); // Create top

            MeshTools.CreateMesh(gameObject, vertices, triangles, uvs);
        }

        private void CalculateCenterPosition(BezierKnot[] knots)
        {
            Vector3 totalPosition = Vector3.zero;

            for (int i = 0; i < knots.Length; i++)
            {
                totalPosition += (Vector3)knots[i].Position;
            }

            centerPosition = transform.TransformPoint(totalPosition / knots.Length);
        }
    }
}