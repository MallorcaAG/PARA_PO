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
    [Header("Spawn Points")]
    [SerializeField] private GameObject[] spawnPoints;
    

    private Rigidbody player;
    bool isMoving;
    bool dataSent = false;
    bool playerPassed = false;

    public GameObject[] getSpawnpoints()
    {
        return spawnPoints;
    }
    public bool PlayerPassedByBefore()
    {
        return playerPassed; 
    }

    private void Awake()
    {
        
    }

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
                //Debug.Log("Player is moving: "+ player.velocity.magnitude);

                if (dataSent)
                {
                    dataSent = false;
                }
            }
            else
            {
                isMoving = false;
                //Debug.Log("Player is still: " + player.velocity.magnitude);

                if (!dataSent)
                {
                    onPlayerStopAtLandmark.Raise(this, this.gameObject);
                    //Debug.Log("Data sent");
                    dataSent = true;
                }
            }

            
            

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject);

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

            playerPassed = true;
        }
    }
}
