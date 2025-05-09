using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNavigator : MonoBehaviour
{
    [Header("Pathfinding")]
    [SerializeField] protected Waypoint currentWaypoint;
    [SerializeField] protected bool changeDirection = false;
    [SerializeField] protected float baseSpeed = 2f;

    private Waypoint holdCurrentWaypoint;
    protected CharacterNav controller;
    protected int direction;
    public bool fullStop = false;

    #region Getter/Setter Functions
    public Waypoint getCurrentWaypoint() => currentWaypoint;
    public void setCurrentWaypoint(Waypoint w) => currentWaypoint = w;
    public CharacterNav getController() => controller;
    #endregion

    #region Runtime Functions
    private void Awake()
    {
        direction = Mathf.RoundToInt(Random.Range(0f, 1f));
        controller = GetComponent<CharacterNav>();

        if (currentWaypoint != null)
        {
            SetDestination(currentWaypoint.GetPosition());
        }
    }

    void Update()
    {
        NPCTraversal();
    }
    #endregion

    protected void NPCTraversal()
    {
        if (currentWaypoint == null)
            return;

        if (controller.destinationInfo.reachedDestination)
        {
            bool shouldBranch = false;

            if (currentWaypoint.branches != null && currentWaypoint.branches.Count > 0)
            {
                shouldBranch = Random.Range(0f, 1f) <= currentWaypoint.branchRatio;
            }

            if (shouldBranch)
            {
                currentWaypoint = currentWaypoint.branches[Random.Range(0, currentWaypoint.branches.Count)];
            }
            else
            {
                if (direction == 0)
                {
                    if (currentWaypoint.nextWaypoint != null)
                    {
                        currentWaypoint = currentWaypoint.nextWaypoint;
                    }
                    else
                    {
                        currentWaypoint = currentWaypoint.previousWaypoint;
                        direction = 1;
                    }
                }
                else if (direction == 1)
                {
                    if (currentWaypoint.previousWaypoint != null)
                    {
                        currentWaypoint = currentWaypoint.previousWaypoint;
                    }
                    else
                    {
                        currentWaypoint = currentWaypoint.nextWaypoint;
                        direction = 0;
                    }
                }
            }

            SetDestination(currentWaypoint.GetPosition());
        }
    }

    protected void SetDestination(Vector3 destination)
    {
        destination = SnapToGround(destination);
        controller.SetDestination(destination);
    }

    protected Vector3 SnapToGround(Vector3 inputPosition)
    {
        RaycastHit hit;

        // Raise the ray origin 2 meters above the input position
        Vector3 rayOrigin = inputPosition + Vector3.up * 2f;
        float rayDistance = 10f;
        int driveableMask = LayerMask.GetMask("Driveable"); // Only raycast against Driveable layer

        // Visual debug line to see the ray in the scene view
        Debug.DrawRay(rayOrigin, Vector3.down * rayDistance, Color.red, 1f);

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, rayDistance, driveableMask))
        {
            return hit.point; // Ground hit point
        }

        // Optional: return position slightly lower if no hit is found
        return inputPosition + Vector3.down * 0.2f;
    }

    public void Stop()

    //Old implementation, dont remove cause idk if i may need this later 
    /*if(currentWaypoint != null)
    {
        holdCurrentWaypoint = currentWaypoint;
        currentWaypoint = null;
    }
    SetDestination(transform.position);*/


    {
        if (controller.movementSpeed > 0.1f)
        {
            controller.movementSpeed -= 0.25f;
        }
        else if (controller.movementSpeed <= 0.1f)
        {
            controller.movementSpeed = 0f;
            fullStop = true;
        }
    }

    public void FULLSTOPNOW()
    {
        controller.movementSpeed = 0f;
        fullStop = true;
    }

    public void Go()

    //Old implementation, dont remove cause idk if i may need this later
    /*if (holdCurrentWaypoint != null)
    {
        currentWaypoint = holdCurrentWaypoint;
        holdCurrentWaypoint = null;
        controller.SetDestination(currentWaypoint.GetPosition());


    }*/


    {
        NPCTraversal();

        if (controller.movementSpeed >= baseSpeed)
        {
            controller.movementSpeed = baseSpeed;
        }
        else
        {
            controller.movementSpeed += 0.1f;
            fullStop = false;
        }
    }
}
