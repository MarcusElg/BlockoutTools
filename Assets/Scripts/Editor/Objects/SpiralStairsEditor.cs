using UnityEditor;
using UnityEngine;

namespace BlockoutTools
{
    [CustomEditor(typeof(SpiralStairs))]
    public class SpiralStairEditor : Editor
    {
        SpiralStairs stairs = null;
        SerializedObject settings;

        private void OnEnable()
        {
            stairs = (SpiralStairs)target;

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
            stairs.innerRadius = EditorGUILayout.FloatField("Inner Radius", stairs.innerRadius);
            stairs.width = EditorGUILayout.FloatField("Width", stairs.width);
            stairs.height = EditorGUILayout.FloatField("Height", stairs.height);
            stairs.targetHeight = EditorGUILayout.FloatField("Target Height", stairs.targetHeight);
            stairs.targetRotation = EditorGUILayout.FloatField(new GUIContent("Target Rotation", "Positive value rotates clockwise whilst negative value rotates anti-clockwise"), stairs.targetRotation);

            if (EditorGUI.EndChangeCheck() || GUILayout.Button("Generate"))
            {
                stairs.Generate();
            }
        }

        private void Draw()
        {
            // Dragging handles requires more than just draw event
            // Rotation target handle
            {
                EditorGUI.BeginChangeCheck();
                Quaternion rotation = Handles.Disc(Quaternion.Euler(0, stairs.transform.rotation.eulerAngles.y + stairs.targetRotation, 0), stairs.transform.position, stairs.transform.TransformDirection(Vector3.up), stairs.innerRadius + stairs.width, false, EditorSnapSettings.rotate);

                if (EditorGUI.EndChangeCheck())
                {
                    stairs.targetRotation = rotation.eulerAngles.y - stairs.transform.rotation.eulerAngles.y;
                    stairs.Generate();
                }
            }

            float widthOffset = stairs.innerRadius + stairs.width / 2;
            // Width handle
            {
                EditorGUI.BeginChangeCheck();
                Vector3 widthHandlePosition = stairs.transform.TransformPoint(Vector3.right * (stairs.width + stairs.innerRadius) + Vector3.up * stairs.height / 2); // Convert to global space
                widthHandlePosition = Handles.Slider(widthHandlePosition, stairs.transform.TransformDirection(Vector3.right), settings.FindProperty("gizmoSize").floatValue, CustomHandles.DiscCapFunction, EditorSnapSettings.move.x);

                if (EditorGUI.EndChangeCheck())
                {
                    stairs.width = (stairs.transform.InverseTransformPoint(widthHandlePosition) - Vector3.up * stairs.height / 2 - Vector3.right * stairs.innerRadius).magnitude; // Convert to local space
                    stairs.Generate();
                }
            }

            // Inner radius handle
            {
                EditorGUI.BeginChangeCheck();
                Vector3 radiusHandlePosition = stairs.transform.TransformPoint(Vector3.right * stairs.innerRadius + Vector3.up * stairs.height / 2); // Convert to global space
                radiusHandlePosition = Handles.Slider(radiusHandlePosition, stairs.transform.TransformDirection(Vector3.right), settings.FindProperty("gizmoSize").floatValue, CustomHandles.DiscCapFunction, EditorSnapSettings.move.x);

                if (EditorGUI.EndChangeCheck())
                {
                    stairs.innerRadius = (stairs.transform.InverseTransformPoint(radiusHandlePosition) - Vector3.up * stairs.height / 2).magnitude * 2; // Convert to local space
                    stairs.Generate();
                }
            }

            // Height handle
            {
                EditorGUI.BeginChangeCheck();
                Vector3 heightHandlePosition = stairs.transform.TransformPoint(Vector3.up * stairs.height + Vector3.right * widthOffset); // Convert to global space
                heightHandlePosition = Handles.Slider(heightHandlePosition, Vector3.up, settings.FindProperty("gizmoSize").floatValue, CustomHandles.DiscCapFunction, EditorSnapSettings.move.y);

                if (EditorGUI.EndChangeCheck())
                {
                    stairs.height = (stairs.transform.InverseTransformPoint(heightHandlePosition) - Vector3.right * widthOffset).magnitude; // Convert to local space
                    stairs.Generate();
                }
            }

            // Target height handle
            {
                EditorGUI.BeginChangeCheck();
                Vector3 targetHeightHandlePosition = stairs.transform.TransformPoint(Vector3.up * stairs.targetHeight); // Convert to global space
                targetHeightHandlePosition = Handles.Slider(targetHeightHandlePosition, Vector3.up, settings.FindProperty("gizmoSize").floatValue, CustomHandles.DiscCapFunction, EditorSnapSettings.move.y);

                if (EditorGUI.EndChangeCheck())
                {
                    stairs.targetHeight = (stairs.transform.InverseTransformPoint(targetHeightHandlePosition)).magnitude; // Convert to local space
                    stairs.Generate();
                }
            }
        }
    }
}