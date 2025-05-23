using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnType
{
    CONSTANT, ONETIME
}

public class VehicleSpawnManager : SpawnManager
{
    [SerializeField] private SpawnType spawnType = SpawnType.CONSTANT;
    [SerializeField] private VehicleWaypoint newWaypoint;

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
            if (mySpawnedObj == null)
            {
                if (!npcs.maxVehicleCountReached())
                {
                    Spawn();

                    if(spawnType == SpawnType.CONSTANT)
                        StartCoroutine(Wait());
                }
            }
        }
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(5f);

        mySpawnedObj = null;
    }
}
