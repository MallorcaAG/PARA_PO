using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianSpawner : MonoBehaviour
{
    public GameObject pedestrianPrefab;
    public int pedestriansToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
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

        while (count < pedestriansToSpawn)
        {
            GameObject obj = Instantiate(pedestrianPrefab);
            Transform child = transform.GetChild(Random.Range(0, transform.childCount));
            obj.GetComponent<PedestrianAINavigator>().setCurrentWaypoint(child.GetComponent<Waypoint>());
            obj.transform.position = child.position;

            yield return new WaitForEndOfFrame();

            count++;
            
        }
    }
}
