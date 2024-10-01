using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNavigator : MonoBehaviour
{
    CharacterNav controller;
    public Waypoint currentWaypoint;
    public bool changeDirection = false;

    private void Awake()
    {
        controller = GetComponent<CharacterNav>();
    }

    // Start is called before the first frame update
    void Start()
    {
        controller.SetDestination(currentWaypoint.GetPosition());
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWaypoint == null)
        {
            return;
        }

        // Access reachedDestination through destinationInfo
        if (controller.destinationInfo.reachedDestination)
        {
            if(!changeDirection)
            {
                currentWaypoint = currentWaypoint.nextWaypoint;
            }
            else
            {
                currentWaypoint = currentWaypoint.previousWaypoint;
            }
            controller.SetDestination(currentWaypoint.GetPosition());
        }
    }


}