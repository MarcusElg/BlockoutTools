using UnityEngine;
using UnityEditor;
using UnityEngine.Splines;

[CustomEditor(typeof(Wall))]
public class WallEditor : Editor
{

    Wall wall = null;

    private void OnEnable()
    {
        wall = (Wall)target;
        wall.Setup();

        wall.GetComponent<SplineContainer>().Spline.changed += wall.Generate;
    }

    private void OnDisable()
    {
        wall.GetComponent<SplineContainer>().Spline.changed -= wall.Generate;
    }

    public void OnSceneGUI()
    {
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
