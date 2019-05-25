using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaypointComponent))]
public class WaypointsEditor : Editor
{
    private bool _inAddingMode;
    private SerializedProperty _waypoints;

    private void OnEnable()
    {
        _waypoints = serializedObject.FindProperty("waypoints");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUILayout.Label("Waypoints: " + _waypoints.arraySize);
        DrawDefaultInspector();
        DrawWaypointAddingButton();
        DrawWaypointRemoveButton();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawWaypointAddingButton()
    {
        if (GUILayout.Button("Add new Waypoints"))
        {
            if (_waypoints.arraySize == 0)
            {
                _waypoints.InsertArrayElementAtIndex(0);
                _waypoints.GetArrayElementAtIndex(0).vector3Value =
                    ((Component) target).transform.position;
            }

            _inAddingMode = true;
        }
    }

    private void DrawWaypointRemoveButton()
    {
        if (_waypoints.arraySize > 0 && GUILayout.Button("Remove last Waypoint"))
            _waypoints.arraySize--;
    }

    private void OnSceneGUI()
    {
        var waypoints = ((WaypointComponent) target).Waypoints;
        if (waypoints == null) return;

        if (_inAddingMode)
            HandleAddingMode(waypoints);
        else
            HandleMoveHandles(waypoints);
    }

    private void HandleAddingMode(ICollection<Vector3> waypoints)
    {
        var controlId = GUIUtility.GetControlID(FocusType.Passive);

        var eventType = Event.current.GetTypeForControl(controlId);
        switch (eventType)
        {
            case EventType.MouseUp:
            {
                if (Event.current.button == 0)
                {
                    waypoints.Add(GetMousePosition());
                    Event.current.Use();
                }

                break;
            }

            case EventType.MouseDown:
                if (Event.current.button == 0)
                    // This prevents the component losing focus on a left-click.
                    // We don't use hotcontrol because we want to keep middle-click
                    // panning functionality.
                    GUIUtility.hotControl = 0;
                else if (Event.current.button == 1) _inAddingMode = false;

                break;

            case EventType.MouseLeaveWindow:
                _inAddingMode = false;
                break;

            case EventType.Repaint:
                var position = waypoints.Last();
                Handles.DrawDottedLine(position, GetMousePosition(), 2f);
                break;
        }
    }

    private void HandleMoveHandles(IList<Vector3> waypoints)
    {
        for (var i = 0; i < waypoints.Count; i++)
        {
            var position = Handles.PositionHandle(waypoints[i], Quaternion.identity);
            if (position == waypoints[i]) continue;

            GUI.changed = true;
            Undo.RecordObject(target, "Move waypoint");
            waypoints[i] = position;
        }
    }

    private static Vector2 GetMousePosition()
    {
        var guiPoint = Event.current.mousePosition;
        var screenPosition = HandleUtility.GUIPointToScreenPixelCoordinate(guiPoint);
        var mousePosition = Camera.current.ScreenToWorldPoint(screenPosition);
        return mousePosition;
    }
}