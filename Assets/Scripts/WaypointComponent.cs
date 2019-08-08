using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaypointComponent : MonoBehaviour
{
    [SerializeField] private List<Vector3> waypoints = new List<Vector3>();

    public List<Vector3> Waypoints => waypoints;

    public WaypointNavigator GetNavigator()
    {
        return new WaypointNavigator(waypoints);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;
        DrawGizmos();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        DrawGizmos();
    }

    private void DrawGizmos()
    {
        if (waypoints.Count == 0) return;
        for (int i = 0; i < waypoints.Count; i++)
        {
            Gizmos.DrawSphere(waypoints[i], 0.5f);
            if (i > 0) Gizmos.DrawLine(waypoints[i], waypoints[i - 1]);
        }
    }
}
