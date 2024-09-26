using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleWaypointHolder : MonoBehaviour
{
    [SerializeField] private Waypoint myWaypoint;

    #region Getter/Setter Functions
    public Waypoint getVehicleWaypoint()
    {
        return myWaypoint;
    }
    #endregion

}
