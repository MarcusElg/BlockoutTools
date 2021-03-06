using UnityEngine;
using UnityEngine.Splines;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.Splines;
using System.Linq;

namespace BlockoutTools
{
    [CustomEditor(typeof(Wall))]
    public class WallEditor : Editor
    {

        Wall wall = null;
        bool setupCompleted = false;
        SerializedObject settings;

        private void OnEnable()
        {
            wall = (Wall)target;

            // Regenerate on changes
            wall.GetComponent<SplineContainer>().Spline.changed += wall.Generate;
            Undo.undoRedoPerformed += wall.Generate;
        }

        private void OnDisable()
        {
            if (wall == null)
            {
                return;
            }

            wall.GetComponent<SplineContainer>().Spline.changed -= wall.Generate;
            Undo.undoRedoPerformed -= wall.Generate;
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

            Event currentEvent = Event.current;
            Draw(currentEvent);

            if (wall.transform.hasChanged)
            {
                wall.Generate();
                wall.transform.hasChanged = false;
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            wall.height = EditorGUILayout.FloatField("Height", wall.height);
            wall.thickness = EditorGUILayout.FloatField("Thickness", wall.thickness);

            if (EditorGUI.EndChangeCheck() || GUILayout.Button("Generate"))
            {
                wall.Generate();
            }
        }

        private void Draw(Event currentEvent)
        {
            // Prevent handles when knot placement tool is selected
            if (wall.GetComponent<SplineContainer>().Spline.Count < 2 || Tools.current == Tool.Custom)
            {
                return;
            }

            // Dragging handles requires more than just draw event
            // Get nearest center position
            int nearestPositionIndex = wall.wallCenterPositions.Select((pos, index) => (pos, index)).OrderBy(position =>
                Vector2.Distance(currentEvent.mousePosition, HandleUtility.WorldToGUIPoint(position.pos + Vector3.up * wall.height))).First().index;
            Vector3 nearestPosition = wall.wallCenterPositions[nearestPositionIndex];
            Vector3 nearestLeft = wall.wallCenterLefts[nearestPositionIndex];

            // Height handle
            {
                EditorGUI.BeginChangeCheck();
                Vector3 currentHeightHandlePosition = nearestPosition + wall.transform.TransformDirection(Vector3.up) * wall.height;
                Vector3 newHeightHandlePosition = Handles.Slider(currentHeightHandlePosition, wall.transform.TransformDirection(Vector3.up), settings.FindProperty("gizmoSize").floatValue, CustomHandles.DiscCapFunction, EditorSnapSettings.move.y);

                if (EditorGUI.EndChangeCheck())
                {
                    wall.height += (wall.transform.InverseTransformPoint(newHeightHandlePosition) - wall.transform.InverseTransformPoint(currentHeightHandlePosition)).y;
                    wall.Generate();
                }
            }

            // Thickness handle
            {
                EditorGUI.BeginChangeCheck();
                Vector3 thicknessHandlePosition = nearestPosition + nearestLeft * wall.thickness;
                thicknessHandlePosition = Handles.Slider(thicknessHandlePosition, nearestLeft, settings.FindProperty("gizmoSize").floatValue, CustomHandles.DiscCapFunction, EditorSnapSettings.move.x);

                if (EditorGUI.EndChangeCheck())
                {
                    wall.thickness = Vector3.Distance(thicknessHandlePosition, nearestPosition);
                    wall.Generate();
                }
            }
        }
    }
}