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
    }

    public void OnSceneGUI()
    {
        Draw();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate"))
        {
            wall.Generate();
        }
    }

    private void Draw()
    {
        
    }
}
