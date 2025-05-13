using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Waypoint previousWaypoint;
    public Waypoint nextWaypoint;

    [Range(0f, 5f)]
    public float width = 1f;

    public List<Waypoint> branches;

    [Range(0f, 1f)]
    public float branchRatio = 0.5f;

    public Vector3 GetPosition()
    {
        // Calculate the minimum and maximum bounds based on the width
        Vector3 minBound = transform.position - transform.right * width / 2f;
        Vector3 maxBound = transform.position + transform.right * width / 2f;

        // Return a random position between the min and max bounds
        return Vector3.Lerp(minBound, maxBound, Random.Range(0f, 1f));
    }

    void Update()
    {
        SnapToGround();
    }

    private void SnapToGround()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 10f;
        float rayDistance = 100f;

        // Only snap to objects on the "Driveable" layer
        int driveableMask = LayerMask.GetMask("Driveable");

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, rayDistance, driveableMask))
        {
            transform.position = hit.point;
        }
    }
}