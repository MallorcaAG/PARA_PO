using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleWaypoint : Waypoint
{
    [Header("VEHICLE LANE SETTINGS")]
    [SerializeField] private bool correctLane;

    public bool isCorrectLane()
    {
        return correctLane;
    }
}
