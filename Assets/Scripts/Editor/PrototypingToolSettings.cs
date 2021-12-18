using System.IO;
using UnityEditor;
using UnityEngine;

public class PrototypingToolSettings : ScriptableObject
{
    public static float gizmoSize = 0.2f;
    public static Color gizmoColour = Color.blue;
    public static Color selectedGizmoColour = Color.red;

    private static PrototypingToolSettings GetOrCreateSettings()
    {
        string path = "Assets/Editor/PrototypingToolSettings.asset";
        PrototypingToolSettings settings = AssetDatabase.LoadAssetAtPath<PrototypingToolSettings>(path);

        if (settings == null)
        {
            if (Directory.Exists("Assets/Editor") == false)
            {
                Directory.CreateDirectory("Assets/Editor");
            }

            settings = ScriptableObject.CreateInstance<PrototypingToolSettings>();
            AssetDatabase.CreateAsset(settings, path);
            AssetDatabase.SaveAssets();
        }

        return settings;
    }

    private static void Validate()
    {
        gizmoSize = Mathf.Clamp(gizmoSize, 0.05f, 0.5f);
    }

    [SettingsProvider]
    public static SettingsProvider CreateSettingsProvider()
    {
        SettingsProvider settingsProvider = new SettingsProvider("Project/PrototypingTools", SettingsScope.Project)
        {
            label = "Prototyping Tools",

            guiHandler = (searchContext) =>
            {
                GUILayout.Label("Gizmos", EditorStyles.boldLabel);
                gizmoSize = EditorGUILayout.FloatField("Gizmo Size", gizmoSize);

                GUILayout.Space(20);
                GUILayout.Label("Colours", EditorStyles.boldLabel);
                gizmoColour = EditorGUILayout.ColorField("Standard Gizmo Colour", gizmoColour);
                selectedGizmoColour = EditorGUILayout.ColorField("Selected Gizmo Colour", selectedGizmoColour);

                Validate();
            }
        };

        return settingsProvider;
    }

}
