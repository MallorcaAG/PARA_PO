using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POI_Despawner : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.GetComponent<Pedestrian_Pathfinding>())
        {
            Destroy(col.gameObject,2f);
        }
    }
}
