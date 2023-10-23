using Barmetler.RoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pedestrian_Pathfinding : MonoBehaviour
{
    public NavMeshAgent nav;
    public Transform goal;

    // Start is called before the first frame update
    void Start()
    {
        nav.SetDestination(goal.position);
    }

    // Update is called once per frame
    void Update()
    {
                
    }
}
