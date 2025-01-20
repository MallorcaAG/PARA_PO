using Codice.Client.Common.GameUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum npc
{
    pedestrians, vehicles
};

public class Spawner : MonoBehaviour
{
    public GameObject pedestrianPrefab;
    public int pedestriansToSpawn;
    private npc  type= npc.pedestrians;

    // Start is called before the first frame update
    void Start()
    {
        if(type == npc.pedestrians)
        {
            StartCoroutine(Spawn());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Spawn()
    {
        if(transform.childCount == 0)
        {
            yield break;
        }

        int count = 0;
        if(type == npc.pedestrians)
        { 
            while (count < pedestriansToSpawn)
            {
                Transform child = transform.GetChild(Random.Range(0, transform.childCount));
            
                if(child.childCount < 1)
                {
                    GameObject spawner = Instantiate(pedestrianPrefab, child);
                    SpawnManager spawnmanager = spawner.GetComponent<SpawnManager>();
                    spawnmanager.setMyWaypoint(child.GetComponent<Waypoint>());
                    //spawnmanager.setMaxNPC(pedestriansToSpawn);
                /*GameObject obj = spawner.GetComponent<SpawnManager>().getPrefab();
                obj.GetComponent<PedestrianAINavigator>().setCurrentWaypoint(child.GetComponent<Waypoint>());
                obj.transform.position = child.position;*/
                    yield return new WaitForFixedUpdate();

                }

                count++;

            }
        }
    }
}
