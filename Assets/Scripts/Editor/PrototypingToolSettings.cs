using System.IO;
using UnityEditor;
using UnityEngine;

namespace BlockoutTools
{
    public class PrototypingToolSettings : ScriptableObject
    {
        public float gizmoSize = 0.2f;
        public Color gizmoColour = Color.blue;
        public Color selectedGizmoColour = Color.red;

        private static string path = "Assets/Editor/PrototypingToolSettings.asset";

        private static PrototypingToolSettings GetOrCreateSettings()
        {
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

        public static SerializedObject GetUpdatedSettings(SerializedObject settings = null)
        {
            if (settings == null || settings.targetObject == null)
            {
                return new SerializedObject(GetOrCreateSettings());
            }

            settings.Update();
            return settings;
        }

        private static void Validate(SerializedObject serializedObject)
        {
            serializedObject.FindProperty("gizmoSize").floatValue = Mathf.Clamp(serializedObject.FindProperty("gizmoSize").floatValue, 0.05f, 0.5f);
        }

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            SettingsProvider settingsProvider = new SettingsProvider("Project/PrototypingTools", SettingsScope.Project)
            {
                label = "Prototyping Tools",

                guiHandler = (searchContext) =>
                {
                    SerializedObject serializedObject = GetUpdatedSettings();

                    EditorGUI.BeginChangeCheck();
                    GUILayout.Label("Gizmos", EditorStyles.boldLabel);
                    serializedObject.FindProperty("gizmoSize").floatValue = EditorGUILayout.FloatField("Gizmo Size", serializedObject.FindProperty("gizmoSize").floatValue);

                    GUILayout.Space(20);
                    GUILayout.Label("Colours", EditorStyles.boldLabel);
                    serializedObject.FindProperty("gizmoColour").colorValue = EditorGUILayout.ColorField("Standard Gizmo Colour", serializedObject.FindProperty("gizmoColour").colorValue);
                    serializedObject.FindProperty("selectedGizmoColour").colorValue = EditorGUILayout.ColorField("Selected Gizmo Colour", serializedObject.FindProperty("selectedGizmoColour").colorValue);

                    GUILayout.Space(20);
                    if (GUILayout.Button("Reset to defaults"))
                    {
                        AssetDatabase.DeleteAsset(path); // Removing setting file resets to default
                        return;
                    }

                    Validate(serializedObject);

                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedPropertiesWithoutUndo();
                    }
                }
            };

            return settingsProvider;
        }

    }
}