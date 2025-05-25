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

    public void Spawn()
    {
        /*if (transform.childCount == 0)
        {*/
        if(prefabCollection == null)
        {
            return;
        }
        GameObject obj = Instantiate(prefabCollection[Random.Range(0, prefabCollection.Length)], gameObject.transform);

        npcs.addVehicleNPC();

        obj.transform.position = transform.position;
        obj.GetComponent<VehicleAINavigator>().setCurrentWaypoint(newWaypoint);

        mySpawnedObj = obj;

        obj.transform.parent = null;

        /*}*/
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name == "SpawningDespawning Influence")
        {
            if (npcs.maxVehicleCountReached())
            {
                return;
            }

            if (mySpawnedObj == null)
            {
                Spawn();

                if (spawnType == SpawnType.CONSTANT)
                    StartCoroutine(Wait());

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
