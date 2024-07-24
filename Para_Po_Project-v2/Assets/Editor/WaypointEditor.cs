using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class WaypointEditor
{
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmo(Waypoint waypoint, GizmoType gizmoType)
    {
        // Check if the waypoint is selected
        if ((gizmoType & GizmoType.Selected) != 0) // Corrected spacing issue
        {
            Gizmos.color = Color.yellow;
        }
        else
        {
            Gizmos.color = Color.yellow * 0.5f;
        }

        // Draw the waypoint as a sphere
        Gizmos.DrawSphere(waypoint.transform.position, 0.1f);

        // Draw a line representing the width of the waypoint
        Gizmos.color = Color.white;
        Gizmos.DrawLine(waypoint.transform.position + (waypoint.transform.right * waypoint.width / 2f),
                        waypoint.transform.position - (waypoint.transform.right * waypoint.width * 2f));

        // Draw a line to the previous waypoint if it exists
        if (waypoint.previousWaypoint != null)
        {
            Gizmos.color = Color.red;
            Vector3 offset = waypoint.transform.right * waypoint.width / 2f;
            Vector3 offsetTo = waypoint.previousWaypoint.transform.right * waypoint.previousWaypoint.width / 2f;

            Gizmos.DrawLine(waypoint.transform.position + offset,
                            waypoint.previousWaypoint.transform.position + offsetTo);
        }

        // Draw a line to the next waypoint if it exists
        if (waypoint.nextWaypoint != null)
        {
            Gizmos.color = Color.green;
            Vector3 offset = waypoint.transform.right * -waypoint.width / 2f;
            Vector3 offsetTo = waypoint.nextWaypoint.transform.right * -waypoint.nextWaypoint.width / 2f;

            Gizmos.DrawLine(waypoint.transform.position + offset,
                            waypoint.nextWaypoint.transform.position + offsetTo);
        }
    }
}