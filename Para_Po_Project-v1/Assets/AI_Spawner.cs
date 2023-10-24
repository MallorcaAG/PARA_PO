using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AI_Spawner : MonoBehaviour
{
    [SerializeField] private float spawnTime = 5f;

    public GameObject obj;

    public POI_SOdef poi;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        GameObject pedestrian = Instantiate(obj, transform.position, Quaternion.identity);
        Transform point = poi.POIs[Random.Range(0, poi.POIs.Length)];
        pedestrian.GetComponent<Pedestrian_Pathfinding>().goal = point;


        yield return new WaitForSeconds(spawnTime);

        StartCoroutine(Start());
    }
}
