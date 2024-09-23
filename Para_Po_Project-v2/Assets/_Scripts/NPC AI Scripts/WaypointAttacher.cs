using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointAttacher : MonoBehaviour
{
    
    private List<Waypoint> waypoints = new List<Waypoint>();



    private void OnTriggerEnter(Collider obj)
    {
        if (!obj.CompareTag("Waypoint"))
            return;

        waypoints.Add(obj.GetComponent<Waypoint>());
    }

    private void OnTriggerExit(Collider obj)
    {
        if (!obj.CompareTag("Waypoint"))
            return;

        waypoints.Remove(obj.GetComponent<Waypoint>());
    }
}
