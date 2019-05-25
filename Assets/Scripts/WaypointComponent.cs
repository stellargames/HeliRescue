using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaypointComponent : MonoBehaviour
{
    [SerializeField] private List<Vector3> waypoints = new List<Vector3>();

    public List<Vector3> Waypoints => waypoints;

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
        var position = transform.position;
        foreach (var waypoint in waypoints)
        {
            Gizmos.DrawLine(position, waypoint);
            position = waypoint;
            Gizmos.DrawSphere(position, 0.5f);
        }
    }
}
