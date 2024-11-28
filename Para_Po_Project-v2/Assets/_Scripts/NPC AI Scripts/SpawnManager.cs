using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlasticGui.LaunchDiffParameters;

public class SpawnManager : MonoBehaviour
{
    public static int npcCount;

    [SerializeField] protected GameObject[] prefabCollection;
    [SerializeField] protected GameObject mySpawnedObj;

    protected Waypoint myWaypoint;
    protected int maxNPC;

    public GameObject[] getPrefab()
    {
        return prefabCollection;
    }
    public void setMyWaypoint(Waypoint waypoint)
    {
        myWaypoint = waypoint;
    }
    public void setMaxNPC(int max)
    {
        maxNPC = max;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "SpawningDespawning Influence")
        {
            if (!mySpawnedObj.activeInHierarchy)
            {
                mySpawnedObj.SetActive(false);
            }
            if(mySpawnedObj.GetComponent<NPCDistanceToPlayer>().primeDestruction == true)
            {
                mySpawnedObj.GetComponent<NPCDistanceToPlayer>().primeDestruction = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "SpawningDespawning Influence")
        {
            if (mySpawnedObj != null)
            {
                mySpawnedObj.GetComponent<NPCDistanceToPlayer>().primeDestruction = true;
            }
        }
    }

    private void OnDestroy()
    {
        SpawnManager.npcCount--;
    }

}
