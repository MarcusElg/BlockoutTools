using UnityEditor;
using UnityEngine;

namespace BlockoutTools
{
    public static class CustomHandles
    {
        // A handle cap that renders a disc
        public static void DiscCapFunction(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            SerializedObject settings = PrototypingToolSettings.GetUpdatedSettings();

            switch (Event.current.type)
            {
                case EventType.Layout:
                case EventType.MouseMove:
                    {
                        HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(position, size));
                        break;
                    }
                case EventType.Repaint:
                    {
                        Handles.color = settings.FindProperty("gizmoColour").colorValue;

                        // On hover
                        if (HandleUtility.nearestControl == controlID)
                        {
                            size *= 1.5f;
                        }

                        // On selection
                        if (GUIUtility.hotControl == controlID)
                        {
                            Handles.color = settings.FindProperty("selectedGizmoColour").colorValue;
                        }

                        Handles.DrawSolidDisc(position, Camera.current.transform.rotation * Vector3.forward, size);
                        Handles.color = new Color(0, 0, 0, 0); // Hide actual handle cap
                        Handles.DotHandleCap(controlID, position, Quaternion.identity, size, eventType);
                        break;
                    }
            }
        }

        public static Vector3 DrawPositionHandle(bool alwaysScale, float handleSize, Vector3 position, Quaternion rotation)
        {
            handleSize = Mathf.Min(handleSize, HandleUtility.GetHandleSize(position));

            if (alwaysScale)
            {
                handleSize = HandleUtility.GetHandleSize(position);
            }

            Color color = Handles.color;
            Handles.color = Handles.xAxisColor;
            position = Handles.Slider(position, rotation * Vector3.right, handleSize, Handles.ArrowHandleCap, EditorSnapSettings.move.x);
            Handles.color = Handles.yAxisColor;
            position = Handles.Slider(position, rotation * Vector3.up, handleSize, Handles.ArrowHandleCap, EditorSnapSettings.move.y);
            Handles.color = Handles.zAxisColor;
            position = Handles.Slider(position, rotation * Vector3.forward, handleSize, Handles.ArrowHandleCap, EditorSnapSettings.move.z);
            Handles.color = Handles.centerColor;
            position = Handles.FreeMoveHandle(position, handleSize * 0.1f, EditorSnapSettings.move, Handles.RectangleHandleCap);
            Handles.color = color;
            return position;
        }
    }
}