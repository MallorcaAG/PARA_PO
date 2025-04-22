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
    [SerializeField] private float deathFromImpactWithPlayerTimer = 5f;

    [Header("Game Event")]
    [SerializeField] private GameEvent onImpactWithPlayer;

    bool isStopped = false, playerHit = false;
    private AISensors senses;
    private NPCDistanceToPlayer destroyer;
    private float holdBaseSpd;


    #region Getter/Setter Functions
    public void setBaseSpeed(float n)
    {
        baseSpeed = n;
    }
    public float getMyBaseSpeed()
    {
        return holdBaseSpd;
    }
    #endregion

    #region Runtime Functions
    // Start is called before the first frame update
    void Start()
    {
        senses = GetComponent<AISensors>();
        destroyer = GetComponent<NPCDistanceToPlayer>();

        if (currentWaypoint != null)
        {
            state = NPCState.DRIVING;
            SetDestination(currentWaypoint.GetPosition());
        }

        holdBaseSpd = baseSpeed;
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

        if(playerHit)
        {
            FULLSTOPNOW();
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
            playerHit = true;
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
            FULLSTOPNOW();
            StartCoroutine(kys());
            
        }
        isStopped = true;
    }

    private IEnumerator kys()
    {
        yield return new WaitForSeconds(deathFromImpactWithPlayerTimer);

        destroyer.kys();
    }

}
