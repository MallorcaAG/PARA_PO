using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianSpawnManager : SpawnManager
{


    public void Spawn()
    {
        /*if (transform.childCount == 0)
        {*/
        if (prefabCollection == null)
        {
            return;
        }
        GameObject obj = Instantiate(prefabCollection[Random.Range(0,prefabCollection.Length)]);

        npcs.addPedestrianNPC();

        obj.transform.position = transform.position;
        obj.GetComponent<PedestrianAINavigator>().setCurrentWaypoint(myWaypoint);

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
                if (!npcs.maxPedestrianCountReached())
                {
                    Spawn();
                }
            }
        }
    }

}
