using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{

    [SerializeField] private Transform playerStartPosition;
    [SerializeField] private GameEvent onPlayerRespawn;

    [SerializeField] private VehicleManager vehicleManager;
    private Landmark lastCheckpoint;
    private float targetTime = 2f;

    private void Start()
    {
        if (vehicleManager == null)
            vehicleManager = GetComponent<VehicleManager>();

        if (playerStartPosition == null)
            playerStartPosition = GameObject.Find("PlayerStartingPoint").transform;
    }

    private void Update() 
    {
        GetPlayerInput();
    }

    private void GetPlayerInput()
    {
        if(Timer())
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                targetTime = 2f;
                InitiateRespawn();
            }
        }
        
    }

    private void InitiateRespawn()
    {
        onPlayerRespawn.Raise(this, 0);

        if(vehicleManager.getCheckpoint() == null)
        {
            transform.position = playerStartPosition.transform.position + Vector3.up;
            transform.rotation = playerStartPosition.transform.rotation;
            return;
        }

        lastCheckpoint = vehicleManager.getCheckpoint();
        lastCheckpoint.ClearArea();
        GameObject spawnpoint = lastCheckpoint.getSpawnpoints()[0];
        transform.position = spawnpoint.transform.position + Vector3.up;
        transform.rotation = spawnpoint.transform.rotation;
    }

    private bool Timer()
    {
        targetTime -= Time.deltaTime;

        if (targetTime <= 0.0f)
        {
            return true;
        }

        return false;
    }
}
