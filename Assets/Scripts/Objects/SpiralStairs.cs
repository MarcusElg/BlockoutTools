using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockoutTools
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class SpiralStairs : MonoBehaviour
    {

        // Properties
        public float innerRadius = 0.25f;
        public float width = 3;
        public bool rotateClockwise = true;
        public float height = 0.25f;
        public float targetHeight = 3f;
        public float targetRotation = 180f;

        public void Generate()
        {
            Validate();
            GenerateMesh();
        }

        public void Validate()
        {
            // Validate properties
            innerRadius = Mathf.Clamp(innerRadius, 0f, 15f);
            width = Mathf.Clamp(width, 1f, 10f);
            height = Mathf.Clamp(height, 0.15f, 1f);
            targetHeight = Mathf.Clamp(targetHeight, 1f, 10f);
            targetRotation = Mathf.Clamp(targetRotation, 0, 1080); // 3 turns

            // Prevent scaling and rotation
            transform.localScale = Vector3.one;
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }

        private void GenerateMesh()
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            int segments = Mathf.Max(2, (int)(targetHeight / height));
            float actualHeight = targetHeight / segments;
            float rotation = targetRotation / segments;

            // Flip rotation direction
            if (rotateClockwise)
            {
                AddStairSide(ref vertices, ref triangles, ref uvs, segments, actualHeight, rotation, innerRadius + width, 0); // Outer side
                AddStairSide(ref vertices, ref triangles, ref uvs, segments, actualHeight, rotation, innerRadius, segments * 3 + 1); // Inner side
            }
            else
            {
                AddStairSide(ref vertices, ref triangles, ref uvs, segments, actualHeight, -rotation, innerRadius, 0); // Inner side 
                AddStairSide(ref vertices, ref triangles, ref uvs, segments, actualHeight, -rotation, innerRadius + width, segments * 3 + 1); // Outer side      
            }

            MeshTools.ConnectToNextIteration(ref triangles, 0, 1, vertices.Count / 2); // Create top/bottom faces
            MeshTools.CreateMesh(gameObject, vertices, triangles, uvs);
        }
        private void AddStairSide(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uvs, int segments, float height, float rotation, float width, int offset)
        {
            float currentRotation = 0;
            // Top part
            for (int i = 0; i < segments; i++)
            {
                Vector3 currentPosition = Quaternion.Euler(0, currentRotation, 0) * Vector3.right;
                Vector3 nextPosition = Quaternion.Euler(0, currentRotation + rotation, 0) * Vector3.right;

                // Add start vertex .
                if (i == 0)
                {
                    vertices.Add(Vector3.right * width);
                }

                // Add top vertices :*
                vertices.Add(Vector3.up * height * (i + 1) + currentPosition.normalized * width); // Top left
                vertices.Add(Vector3.up * height * (i + 1) + nextPosition.normalized * width); // Top right

                // Add uvs
                // Add start vertex
                if (i == 0)
                {
                    uvs.Add(Vector2.zero);
                }

                uvs.Add(Vector2.zero);
                uvs.Add(Vector2.zero);
                uvs.Add(Vector2.zero);

                // Change variables for next iteration
                currentRotation += rotation;
            }

            // Bottom part
            for (int i = segments - 1; i >= 0; i--)
            {
                currentRotation -= rotation;
                Vector3 nextPosition = Quaternion.Euler(0, currentRotation + rotation, 0) * Vector3.right;

                vertices.Add(Vector3.up * height * i + nextPosition.normalized * width); // Bottom right vertex ::

                if (offset == 0)
                {
                    // Normal in left direction
                    MeshTools.AddSquare(ref triangles, offset + i * 2, offset + i * 2 + 1, offset + i * 2 + 2, vertices.Count - 1);
                    MeshTools.AddTriangle(ref triangles, offset + i * 2 + 2, vertices.Count - 1, vertices.Count - 2);
                }
                else
                {
                    // Normal in right direction
                    MeshTools.AddSquare(ref triangles, vertices.Count - 1, offset + i * 2 + 2, offset + i * 2 + 1, offset + i * 2);
                    MeshTools.AddTriangle(ref triangles, vertices.Count - 2, vertices.Count - 1, offset + i * 2 + 2);
                }
            }
        }
    }
}