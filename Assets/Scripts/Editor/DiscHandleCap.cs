using UnityEditor;
using UnityEngine;

public static class DiscHandleCap
{
    // A handle cap that renders a disc
    public static void CapFunction(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
    {
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
                    Handles.color = PrototypingToolSettings.gizmoColour;

                    // On hover
                    if (HandleUtility.nearestControl == controlID)
                    {
                        size *= 1.5f;
                    }

                    // On selection
                    if (GUIUtility.hotControl == controlID)
                    {
                        Handles.color = PrototypingToolSettings.selectedGizmoColour;
                    }

                    Handles.DrawSolidDisc(position, Camera.current.transform.rotation * Vector3.forward, size);
                    Handles.color = new Color(0, 0, 0, 0); // Hide actual handle cap
                    Handles.DotHandleCap(controlID, position, Quaternion.identity, size, eventType);
                    break;
                }
        }
    }
}
