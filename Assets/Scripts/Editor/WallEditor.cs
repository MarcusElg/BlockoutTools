using UnityEngine;
using UnityEngine.Splines;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.Splines;

[CustomEditor(typeof(Wall))]
public class WallEditor : Editor
{

    Wall wall = null;
    bool setupCompleted = false;

    private void OnEnable()
    {
        wall = (Wall)target;
        wall.Setup();

        wall.GetComponent<SplineContainer>().Spline.changed += wall.Generate;
    }

    private void OnDisable()
    {
        wall.GetComponent<SplineContainer>().Spline.changed -= wall.Generate;
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

        Draw();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck() || GUILayout.Button("Generate"))
        {
            wall.Generate();
        }
    }

    private void Draw()
    {

    }
}
