using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Stairs))]
public class StairEditor : Editor
{
    Stairs stairs = null;
    SerializedObject settings;

    private void OnEnable()
    {
        stairs = (Stairs)target;

        // Regenerate on changes
        Undo.undoRedoPerformed += stairs.Generate;
    }

    private void OnDisable()
    {
        if (stairs == null)
        {
            return;
        }

        Undo.undoRedoPerformed -= stairs.Generate;
    }

    public void OnSceneGUI()
    {
        settings = PrototypingToolSettings.GetUpdatedSettings(settings);

        // Rotation and scaling tools breaks the system
        if (Tools.current == Tool.Rotate || Tools.current == Tool.Scale)
        {
            Tools.current = Tool.Move;
        }

        Draw();

        if (stairs.transform.hasChanged)
        {
            stairs.Generate();
            stairs.transform.hasChanged = false;
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        stairs.depth = EditorGUILayout.FloatField("Depth", stairs.depth);
        stairs.width = EditorGUILayout.FloatField("Width", stairs.width);
        stairs.height = EditorGUILayout.FloatField("Height", stairs.height);
        stairs.targetPosition = EditorGUILayout.Vector3Field(new GUIContent("Target Position", "Position for the stairs to generate towards in local space"), stairs.targetPosition);

        if (EditorGUI.EndChangeCheck() || GUILayout.Button("Generate"))
        {
            stairs.Generate();
        }
    }

    private void Draw()
    {
        // Dragging handles requires more than just draw event
        // Target position handle
        {
            EditorGUI.BeginChangeCheck();
            Vector3 currentTargetHandlePosition = stairs.transform.position + stairs.targetPosition;
            Vector3 newTargetHandlePosition = CustomHandles.DrawPositionHandle(false, settings.FindProperty("gizmoSize").floatValue * 5, currentTargetHandlePosition, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                stairs.targetPosition = newTargetHandlePosition - stairs.transform.position;
                stairs.Generate();
            }
        }

        // Width handle
        {
            EditorGUI.BeginChangeCheck();
            Vector3 widthHandlePosition = stairs.transform.TransformPoint(Vector3.right * stairs.width / 2 + Vector3.up * stairs.height / 2 + Vector3.forward * stairs.depth / 2); // Convert to global space
            widthHandlePosition = Handles.Slider(widthHandlePosition, stairs.transform.TransformDirection(Vector3.right), settings.FindProperty("gizmoSize").floatValue, CustomHandles.DiscCapFunction, EditorSnapSettings.move.x);

            if (EditorGUI.EndChangeCheck())
            {
                stairs.width = (stairs.transform.InverseTransformPoint(widthHandlePosition) - Vector3.up * stairs.height / 2 - Vector3.forward * stairs.depth / 2).magnitude * 2; // Convert to local space
                stairs.Generate();
            }
        }

        // Depth handle
        {
            EditorGUI.BeginChangeCheck();
            Vector3 depthHandlePosition = stairs.transform.TransformPoint(Vector3.up * stairs.height / 2 + Vector3.forward * stairs.depth); // Convert to global space
            depthHandlePosition = Handles.Slider(depthHandlePosition, stairs.transform.TransformDirection(Vector3.forward), settings.FindProperty("gizmoSize").floatValue, CustomHandles.DiscCapFunction, EditorSnapSettings.move.z);

            if (EditorGUI.EndChangeCheck())
            {
                stairs.depth = (stairs.transform.InverseTransformPoint(depthHandlePosition) - Vector3.up * stairs.height / 2).magnitude; // Convert to local space
                stairs.Generate();
            }
        }

        // Height handle
        {
            EditorGUI.BeginChangeCheck();
            Vector3 heightHandlePosition = stairs.transform.TransformPoint(Vector3.up * stairs.height + Vector3.forward * stairs.depth / 2); // Convert to global space
            heightHandlePosition = Handles.Slider(heightHandlePosition, stairs.transform.TransformDirection(Vector3.up), settings.FindProperty("gizmoSize").floatValue, CustomHandles.DiscCapFunction, EditorSnapSettings.move.y);

            if (EditorGUI.EndChangeCheck())
            {
                stairs.height = (stairs.transform.InverseTransformPoint(heightHandlePosition) - Vector3.forward * stairs.depth / 2).magnitude; // Convert to local space
                stairs.Generate();
            }
        }
    }
}
