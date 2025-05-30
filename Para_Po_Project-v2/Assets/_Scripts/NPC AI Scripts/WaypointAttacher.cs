using System.Collections.Generic;
using UnityEngine;



public class WaypointAttacher : MonoBehaviour
{
    [SerializeField] private Waypoint myWaypoint;
    private List<Waypoint> waypoints = new List<Waypoint>();

    Waypoint currentClosestWaypoint;
    float dist;

    private void Update()
    {
        //Setup
        if (waypoints.Count == 0)
        {
            myWaypoint.previousWaypoint = null;
            myWaypoint.nextWaypoint = null;
            dist = 999;
            return;
        }
        if (currentClosestWaypoint == null)
        {
            currentClosestWaypoint = waypoints[waypoints.Count - 1];
            dist = Mathf.Abs(Vector3.Distance(myWaypoint.transform.position, currentClosestWaypoint.transform.position));
        }

        //Check if currentClosestWaypoint is still in the List
        foreach (Waypoint waypoint in waypoints)
        {
            //If it is, stay connected
            if (currentClosestWaypoint == waypoint)
            {
                return;
            }
            //If not, disconnect
            else
            {
                myWaypoint.previousWaypoint = null;
                myWaypoint.nextWaypoint = null;
                dist = 999;
            }
        }

        //then change the connection
        for (int i = 0; i < waypoints.Count; i++)
        {
            float newDist = Mathf.Abs(Vector3.Distance(myWaypoint.transform.position, waypoints[i].transform.position));

            if (newDist < dist)
            {
                currentClosestWaypoint = waypoints[i];
                dist = newDist;
            }
        }


        //Update Waypoint Information
        myWaypoint.previousWaypoint = currentClosestWaypoint;
        myWaypoint.nextWaypoint = currentClosestWaypoint;
    }

    private void OnTriggerEnter(Collider obj)
    {

        if (!obj.CompareTag("Waypoint"))
            return;

        if (obj.GetComponents<Waypoint>() == null || obj.GetComponent<VehicleWaypoint>() != null)
            return;

        //Debug.LogWarning("WAYPOINT FOUND: " + obj.name);

        waypoints.Add(obj.GetComponent<Waypoint>());
    }

    private void OnTriggerExit(Collider obj)
    {

        if (!obj.CompareTag("Waypoint"))
            return;

        if (obj.GetComponents<Waypoint>() == null)
            return;

        waypoints.Remove(obj.GetComponent<Waypoint>());
    }
}