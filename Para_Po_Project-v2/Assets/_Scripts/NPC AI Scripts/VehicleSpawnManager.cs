using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnType
{
    CONSTANT, ONEATATIME, ONSTART
}

public class VehicleSpawnManager : SpawnManager
{
    [SerializeField] private SpawnType spawnType = SpawnType.CONSTANT;
    [SerializeField] private VehicleWaypoint newWaypoint;
    [SerializeField] private bool forceSpawn = false;
    [SerializeField] private float obstructionTimer = 15f;

    public void Spawn()
    {
        /*if (transform.childCount == 0)
        {*/
        if(prefabCollection == null)
        {
            return;
        }
        GameObject obj = Instantiate(prefabCollection[Random.Range(0, prefabCollection.Length)], gameObject.transform);
        Transform objTF = obj.transform;
        VehicleAINavigator objNav = obj.GetComponent<VehicleAINavigator>();

        npcs.addVehicleNPC();

        objTF.position = transform.position;
        objNav.setCurrentWaypoint(newWaypoint);
        objNav.setObstructionDespawnTimer(obstructionTimer);
        obj.GetComponent<NPCDistanceToPlayer>().setNPCCount(npcs);

        mySpawnedObj = obj;

        objTF.parent = null;

        /*}*/
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name == "SpawningDespawning Influence")
        {
            if (mySpawnedObj == null)
            {
                if (!npcs.maxVehicleCountReached())
                {
                    Spawn();

                    if (spawnType == SpawnType.CONSTANT)
                        StartCoroutine(Wait());
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "SpawningDespawning Influence")
        {
            if (npcs.maxVehicleCountReached() && !forceSpawn)
            {
                return;
            }

            if (spawnType == SpawnType.ONSTART)
            {
                if (mySpawnedObj == null)
                {
                    Spawn();

                    gameObject.SetActive(false);
                }
            }
        }
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(6f);

        mySpawnedObj = null;
    }
}
