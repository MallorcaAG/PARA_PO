using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDistanceToPlayer : MonoBehaviour
{
    [SerializeField] private float range;
    public bool primeDestruction;

    private Transform player;
    private float distance;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        distance = Vector3.Distance(player.position, transform.position);

        if(primeDestruction)
        {
            if(distance >= range)
            {
                Destroy(gameObject);
            }
        }
    }
}
