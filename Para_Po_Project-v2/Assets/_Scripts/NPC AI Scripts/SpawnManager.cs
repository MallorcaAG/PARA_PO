using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlasticGui.LaunchDiffParameters;

public class SpawnManager : MonoBehaviour
{
    public static int npcCount;

    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject mySpawnedObj;

    Waypoint myWaypoint;
    private int maxNPC;

    public GameObject getPrefab()
    {
        return prefab;
    }
    public void setMyWaypoint(Waypoint waypoint)
    {
        myWaypoint = waypoint;
    }
    public void setMaxNPC(int max)
    {
        maxNPC = max;
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "SpawningDespawning Influence")
        {
            if(mySpawnedObj == null)
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

    public void Initialize()
    {
        /*if (transform.childCount == 0)
        {*/
        GameObject obj = Instantiate(prefab);

        SpawnManager.npcCount++;

        obj.transform.position = transform.position;
        obj.GetComponent<PedestrianAINavigator>().setCurrentWaypoint(myWaypoint);

        mySpawnedObj = obj;

        obj.transform.parent = null;

        /*}*/
    }
}
