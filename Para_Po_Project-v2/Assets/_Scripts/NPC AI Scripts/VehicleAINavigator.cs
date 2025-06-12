using System.Collections;
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

    bool isStopped = false, playerHit = false, dying = false, timerSet = false;
    private AISensors senses;
    private NPCDistanceToPlayer destroyer;
    private float holdBaseSpd, targetTime;
    private float obstructionDespawnTimer = 15f;

    #region Getter/Setter Functions
    public void setBaseSpeed(float n)
    {
        baseSpeed = n;
    }
    public float getMyBaseSpeed()
    {
        return holdBaseSpd;
    }
    public void setObstructionDespawnTimer(float n)
    {
        obstructionDespawnTimer = n;
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

        if(fullStop)
        {
            if(!timerSet)
            {
                targetTime = obstructionDespawnTimer;
                timerSet = true;
            }

            if (Timer())
            {
                stopChecker();
            }
            
        }
        else
        {
            timerSet = false;
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
        if (dying) 
            { return; }


        if (collision.transform.CompareTag("Player"))
        {
            Rigidbody playerRB = collision.transform.GetComponent<Rigidbody>();

            playerHit = true;
            if(playerRB.velocity.magnitude <= 0.1f)
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
        else if (collision.transform.CompareTag("Vehicles"))
        {
            FULLSTOPNOW();
            StartCoroutine(kys());
        }
        isStopped = true;
    }

    private void stopChecker()
    {
        if ((controller.movementSpeed <= 1f))
        {
            Debug.LogWarning("Death by obstruction, "+gameObject.name);
            fastKYS();
        }
    }

    private IEnumerator kys()
    {
        yield return new WaitForSeconds(deathFromImpactWithPlayerTimer);
        dying = true;
        destroyer.kys();
    }

    public void fastKYS()
    {
        dying = true;
        destroyer.kys();
    }

    private bool Timer()
    {
        targetTime -= Time.deltaTime;

        if (targetTime <= 0.0f)
        {
            return true;
        }

        return false;
    }

}
