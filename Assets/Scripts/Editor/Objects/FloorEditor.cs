using UnityEngine;
using UnityEngine.Splines;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.Splines;
using System.Linq;

[CustomEditor(typeof(Floor))]
public class FloorEditor : Editor
{

    Floor floor = null;
    bool setupCompleted = false;
    SerializedObject settings;

    private void OnEnable()
    {
        floor = (Floor)target;

        // Regenerate on changes
        floor.GetComponent<SplineContainer>().Spline.changed += floor.Generate;
        Undo.undoRedoPerformed += floor.Generate;
    }

    private void OnDisable()
    {
        floor.GetComponent<SplineContainer>().Spline.changed -= floor.Generate;
        Undo.undoRedoPerformed -= floor.Generate;
        setupCompleted = false;
    }

    public void OnSceneGUI()
    {
        // Calling it in OnEnable is too early as spline has not loaded yet
        if (!setupCompleted)
        {
            ToolManager.SetActiveContext(typeof(SplineToolContext));
            setupCompleted = true;
        }

        settings = PrototypingToolSettings.GetUpdatedSettings(settings);

        Draw();

        if (floor.transform.hasChanged)
        {
            floor.Generate();
            floor.transform.hasChanged = false;
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        floor.thickness = EditorGUILayout.FloatField("Thickness", floor.thickness);

        if (EditorGUI.EndChangeCheck() || GUILayout.Button("Generate"))
        {
            floor.Generate();
        }
    }

    private void Draw()
    {
        // Prevent handles when knot placement tool is selected
        if (floor.GetComponent<SplineContainer>().Spline.Count < 2 || Tools.current == Tool.Custom)
        {
            return;
        }

        // Thickness handle
        {
            EditorGUI.BeginChangeCheck();
            Vector3 currentThicknessHandlePosition = floor.centerPosition + floor.transform.TransformDirection(Vector3.up) * floor.thickness;
            Vector3 newThicknessHandlePosition = Handles.Slider(currentThicknessHandlePosition, floor.transform.TransformDirection(Vector3.up), settings.FindProperty("gizmoSize").floatValue, CustomHandles.DiscCapFunction, EditorSnapSettings.move.x);

            if (EditorGUI.EndChangeCheck())
            {
                floor.thickness += (floor.transform.InverseTransformPoint(newThicknessHandlePosition) - floor.transform.InverseTransformPoint(currentThicknessHandlePosition)).y;
                floor.Generate();
            }
        }
    }
}
