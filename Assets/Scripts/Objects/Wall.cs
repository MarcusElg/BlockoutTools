using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BlockoutTools
{
    [RequireComponent(typeof(SplineContainer), typeof(MeshRenderer), typeof(MeshFilter))]
    public class Wall : MonoBehaviour
    {

        // Properties
        public float height = 2;
        public float thickness = 0.25f;

        // Internal
        public List<Vector3> wallCenterPositions = new List<Vector3>();
        public List<Vector3> wallCenterLefts = new List<Vector3>();

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
            height = Mathf.Clamp(height, 0.5f, 10);
            thickness = Mathf.Clamp(thickness, 0.1f, 2.5f);

            // Validate spline
            SplineContainer splineContainer = GetComponent<SplineContainer>();
            splineContainer.Spline.EditType = SplineType.Linear;

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
                wallCenterPositions.Clear();
                wallCenterLefts.Clear();
                return;
            }

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
                uvs.Add(Vector2.zero);
                uvs.Add(Vector2.zero);
                uvs.Add(Vector2.zero);
                uvs.Add(Vector2.zero);
            }

            if (shouldClose)
            {
                // Add extra vertices to prevent uv stretching
                vertices.AddRange(vertices.GetRange(0, 4));

                // Add extra uvs
                uvs.Add(Vector2.zero);
                uvs.Add(Vector2.zero);
                uvs.Add(Vector2.zero);
                uvs.Add(Vector2.zero);
            }

            MeshTools.CreateMesh(gameObject, vertices, triangles, uvs);
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
    }
}