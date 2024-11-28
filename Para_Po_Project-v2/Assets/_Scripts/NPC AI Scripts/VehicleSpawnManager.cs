using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSpawnManager : SpawnManager
{
    [SerializeField] private VehicleWaypoint newWaypoint;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        /*if (transform.childCount == 0)
        {*/
        if(prefabCollection == null)
        {
            return;
        }
        GameObject obj = Instantiate(prefabCollection[Random.Range(0, prefabCollection.Length)]);

        SpawnManager.npcCount++;

        obj.transform.position = transform.position;
        obj.GetComponent<VehicleAINavigator>().setCurrentWaypoint(newWaypoint);

        mySpawnedObj = obj;

        obj.transform.parent = null;

        /*}*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "SpawningDespawning Influence")
        {
            if (mySpawnedObj == null)
            {
                if (npcCount < maxNPC)
                {
                    Initialize();
                }
            }
            if (!mySpawnedObj.activeInHierarchy)
            {
                mySpawnedObj.SetActive(false);
            }
            if (mySpawnedObj.GetComponent<NPCDistanceToPlayer>().primeDestruction == true)
            {
                mySpawnedObj.GetComponent<NPCDistanceToPlayer>().primeDestruction = false;
            }
        }
    }
}
