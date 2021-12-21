using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Stairs))]
public class StairEditor : Editor
{
    Stairs stairs = null;
    bool setupCompleted = false;
    SerializedObject settings;

    private void OnEnable()
    {
        stairs = (Stairs)target;
        settings = PrototypingToolSettings.GetUpdatedSettings();

        // Regenerate on changes
        Undo.undoRedoPerformed += stairs.Generate;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed += stairs.Generate;
        setupCompleted = false;
    }

    public void OnSceneGUI()
    {
        settings = PrototypingToolSettings.GetUpdatedSettings(settings);

        Event currentEvent = Event.current;
        Draw(currentEvent);

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
        stairs.uvScaling = EditorGUILayout.FloatField("Uv Scale", stairs.uvScaling);
        stairs.targetPosition = EditorGUILayout.Vector3Field(new GUIContent("Target position", "Position for the stairs to generate towards in local space"), stairs.targetPosition);

        if (EditorGUI.EndChangeCheck() || GUILayout.Button("Generate"))
        {
            stairs.Generate();
        }
    }

    private void Draw(Event currentEvent)
    {
        // Dragging handles requires more than just draw event
        // Get nearest center position
        /*int nearestPositionIndex = wall.wallCenterPositions.Select((pos, index) => (pos, index)).OrderBy(position =>
            Vector2.Distance(currentEvent.mousePosition, HandleUtility.WorldToGUIPoint(position.pos + Vector3.up * wall.height))).First().index;
        Vector3 nearestPosition = wall.wallCenterPositions[nearestPositionIndex];
        Vector3 nearestLeft = wall.wallCenterLefts[nearestPositionIndex];

        // Height handle
        {
            EditorGUI.BeginChangeCheck();
            Vector3 currentHeightHandlePosition = nearestPosition + Vector3.up * wall.height;
            Vector3 newHeightHandlePosition = Handles.Slider(currentHeightHandlePosition, Vector3.up, settings.FindProperty("gizmoSize").floatValue, DiscHandleCap.CapFunction, EditorSnapSettings.move.y);

            if (EditorGUI.EndChangeCheck())
            {
                wall.height += (newHeightHandlePosition - currentHeightHandlePosition).y;
                wall.Generate();
            }
        }

        // Thickness handle
        {
            EditorGUI.BeginChangeCheck();
            Vector3 thicknessHandlePosition = nearestPosition + nearestLeft * wall.thickness;
            thicknessHandlePosition = Handles.Slider(thicknessHandlePosition, nearestLeft, settings.FindProperty("gizmoSize").floatValue, DiscHandleCap.CapFunction, EditorSnapSettings.move.x);

            if (EditorGUI.EndChangeCheck())
            {
                wall.thickness = Vector3.Distance(thicknessHandlePosition, nearestPosition);
                wall.Generate();
            }
        }*/
    }
}
