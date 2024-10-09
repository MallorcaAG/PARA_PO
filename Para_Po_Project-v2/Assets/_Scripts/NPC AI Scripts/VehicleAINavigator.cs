using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleAINavigator : WaypointNavigator
{
    private enum NPCState
    {
        WAITING, DRIVING
    };

    [Header("Behavior")]
    [SerializeField] private NPCState state;

    [Header("Game Event")]
    [SerializeField] private GameEvent onImpactWithPlayer;

    bool isStopped = false;
    private VehicleAISensors senses;

    #region Getter/Setter Functions

    #endregion

    #region Runtime Functions
    // Start is called before the first frame update
    void Start()
    {
        senses = GetComponent<VehicleAISensors>();

        if (currentWaypoint != null)
        {
            state = NPCState.DRIVING;
            SetDestination(currentWaypoint.GetPosition());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isStopped)
        {
            Stop();
        }
        else
        {
            Go();
        }
    }
    #endregion

    public void Brakes()
    {
        Stop();
        isStopped = true;
    }
    public void Gas()
    {
        isStopped = false;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if(senses.getSensors() && !fullStop)
            {
                //I'm at fault
                onImpactWithPlayer.Raise(this, 0);
                
            }
            else
            {
                //Not my fault
                onImpactWithPlayer.Raise(this, 1);

            }
            
            
        }
        isStopped = true;
    }

}
