using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Landmark : MonoBehaviour
{
    [Header("Game Events")]
    [SerializeField] private GameEvent onPlayerEnterLandmark;
    [SerializeField] private GameEvent onPlayerExitLandmark;
    [SerializeField] private GameEvent onPlayerStopAtLandmark;
    [Header("My Waypoints")]
    [SerializeField] private Waypoint[] waypoints;

    private Rigidbody player;
    bool isMoving;
    bool dataSent = false;

    private void LateUpdate()
    {
        if (player == null)
        {
            return;
        }

        if (player != null)
        {
            if (player.velocity.magnitude > 0.1f)
            {
                isMoving = true;

                if(!dataSent)
                {
                    onPlayerStopAtLandmark.Raise(this, this.gameObject);

                    dataSent = true;
                }
            }
            else
            {
                isMoving = false;
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(player != null)
        {
            return;
        }

        if(other.gameObject.CompareTag("Player"))
        {
            player = other.gameObject.GetComponent<Rigidbody>();

            onPlayerEnterLandmark.Raise(this, this.gameObject);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = null;

            onPlayerExitLandmark.Raise(this, this.gameObject);
        }
    }
}
