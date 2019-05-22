using UnityEngine;

[ExecuteInEditMode]
public class WaypointFollower : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;

#if UNITY_EDITOR
    private void Update()
    {
        var position = transform.position;
        foreach (var waypoint in waypoints)
        {
            Debug.DrawLine(position, waypoint.position, Color.green);
            position = waypoint.position;
        }
    }
#endif
}