using UnityEngine;
using UnityEditor;

namespace BlockoutTools
{
    [CustomEditor(typeof(Primitive))]
    public class PrimitiveEditor : Editor
    {
        Primitive primitive = null;
        SerializedObject settings;

        private void OnEnable()
        {
            primitive = (Primitive)target;
            Tools.current = Tool.None;
        }

        private void OnSceneGUI()
        {
            settings = PrototypingToolSettings.GetUpdatedSettings(settings);

            Draw();
        }

        private void Draw()
        {
            // Dragging handles requires more than just draw event
            // Bounds position handles
            {
                ScaleHandle(primitive.GetComponent<MeshFilter>().sharedMesh.bounds.min);
                ScaleHandle(primitive.GetComponent<MeshFilter>().sharedMesh.bounds.max);
            }
        }

        private void ScaleHandle(Vector3 bounds)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 currentBoundsHandlePosition = primitive.transform.TransformPoint(bounds);
            Vector3 newBoundsHandlePosition = CustomHandles.DrawPositionHandle(false, settings.FindProperty("gizmoSize").floatValue * 5, currentBoundsHandlePosition, Quaternion.LookRotation(primitive.transform.TransformDirection(Vector3.forward)));

            if (EditorGUI.EndChangeCheck())
            {
                Vector3 newLocalPosition = primitive.transform.InverseTransformPoint(newBoundsHandlePosition);
                // Only apply half of the scaling as the movement otherwise will make it scale twice as much in that direction
                float xScale = Mathf.Lerp(1, newLocalPosition.x / bounds.x, 0.5f) * primitive.transform.localScale.x;
                float yScale = Mathf.Lerp(1, newLocalPosition.y / bounds.y, 0.5f) * primitive.transform.localScale.y;
                float zScale = Mathf.Lerp(1, newLocalPosition.z / bounds.z, 0.5f) * primitive.transform.localScale.z;

                Vector3 movement = new Vector3(xScale - primitive.transform.localScale.x, yScale - primitive.transform.localScale.y, zScale - primitive.transform.localScale.z);
                movement.Scale(bounds); // Scale with bounds to work the same regardless of bound size
                primitive.transform.localScale = new Vector3(xScale, yScale, zScale);
                primitive.transform.position += primitive.transform.rotation * movement;
            }
        }
    }
}