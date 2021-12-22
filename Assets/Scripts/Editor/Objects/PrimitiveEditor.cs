using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Primitive))]
public class PrimitiveEditor : Editor
{
    Primitive primitive = null;
    SerializedObject settings;

    private void OnEnable()
    {
        primitive = (Primitive)target;
    }

    private void OnSceneGUI()
    {
        settings = PrototypingToolSettings.GetUpdatedSettings(settings);

        Draw();
    }

    private void Draw()
    {
        // Dragging handles requires more than just draw event
        // Bounds position handle
        {
            EditorGUI.BeginChangeCheck();
            Vector3 bounds = primitive.GetComponent<MeshFilter>().sharedMesh.bounds.max;
            Vector3 currentBoundsHandlePosition = primitive.transform.TransformPoint(bounds);
            Vector3 newBoundsHandlePosition = CustomHandles.DrawPositionHandle(false, settings.FindProperty("gizmoSize").floatValue * 5, currentBoundsHandlePosition, Quaternion.LookRotation(primitive.transform.TransformDirection(Vector3.forward)));

            if (EditorGUI.EndChangeCheck())
            {
                Vector3 newLocalPosition = primitive.transform.InverseTransformPoint(newBoundsHandlePosition);
                float xScale = newLocalPosition.x / bounds.x * primitive.transform.localScale.x;
                float yScale = newLocalPosition.y / bounds.y * primitive.transform.localScale.y;
                float zScale = newLocalPosition.z / bounds.z * primitive.transform.localScale.z;

                Vector3 movement = new Vector3(xScale - primitive.transform.localScale.x, yScale - primitive.transform.localScale.y, zScale - primitive.transform.localScale.z) / 2;
                primitive.transform.localScale = new Vector3(xScale, yScale, zScale);
                primitive.transform.position += primitive.transform.rotation * movement;
            }
        }
    }
}
