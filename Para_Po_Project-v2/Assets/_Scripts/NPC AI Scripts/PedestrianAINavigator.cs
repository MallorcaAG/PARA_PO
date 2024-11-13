using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianAINavigator : WaypointNavigator
{
    private enum NPCState
    {
        WAITING, WALKING, RIDING, INGRESS, EGRESS
    };

    [Header("Behavior")]
    [SerializeField] private NPCState state;
    [SerializeField] private GameObject myLandmark;
    [SerializeField] private GameObject desiredLandmark;

    [Header("Game Event")]
    [SerializeField] private GameEvent onPedestrianIngress;
    [SerializeField] private GameEvent onPedestrianEgress;
    [SerializeField] private GameEvent onImpactWithPlayer;
    [SerializeField] private GameEvent onSuccessfulEgress;

    private Waypoint playersWaypoint;
    private PedestrianAISensors senses;

    bool canBeViolated = true;
    bool isRiding = false;

    #region Getter/Setter funcs
    public void setMyLandmark(GameObject newLandmark)
    {
        myLandmark = newLandmark;
    }

    public void setDesiredLandmark(GameObject newLandmark)
    {
        desiredLandmark = newLandmark;
    }

    public void setPlayersWaypointRef(Component component, object data)
    {
        GameObject obj = (GameObject)data;
        playersWaypoint = obj.GetComponent<Waypoint>();
    }

    
    #endregion

    #region Runtime Functions
    // Start is called before the first frame update
    void Start()
    {
        senses = GetComponent<PedestrianAISensors>();

        if (currentWaypoint == null)
        {
            state = NPCState.WAITING;
            SetDestination(transform.position);
        }
        else
        {
            state = NPCState.WALKING;
            SetDestination(currentWaypoint.GetPosition());
        }

        baseSpeed = Random.Range(1f,3f);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case NPCState.WAITING:
                break;

            case NPCState.WALKING:
                NPCWalking();
                break;

            case NPCState.INGRESS:
                if (controller.destinationInfo.reachedDestination)
                {
                    state = NPCState.RIDING;
                    isRiding = true;
                    onPedestrianIngress.Raise(this, gameObject);
                }
                break;

            case NPCState.RIDING:
                break;

            case NPCState.EGRESS:
                if (controller.destinationInfo.reachedDestination)
                {
                    state = NPCState.WALKING;
                    senses.enabled = true;
                    canBeViolated = true;
                    myLandmark = null;
                    desiredLandmark = null;
                    playersWaypoint = null;
                    onSuccessfulEgress.Raise(this, gameObject);
                }
                break;
        }
    }
    #endregion

    private void NPCWalking()
    {
        NPCTraversal();
    }

    public void pivot(Vector3 sense, float maxDistance)
    {
        Vector3 pivotPos = transform.TransformPoint(sense * maxDistance);

        SetDestination(pivotPos);
    }

    #region Pedestrian Specific Functions
    public void GetOnVehicle(Component component, object landmarkPlayerIsIn)
    {
        if ((GameObject)landmarkPlayerIsIn != myLandmark)
        {
            return;
        }

        senses.enabled = false;
        state = NPCState.INGRESS;
        SetDestination(playersWaypoint.GetPosition());
        canBeViolated = false;
    }

    public void LandmarkReached(Component sender, object landmarkPlayerIsIn)
    {
        if ((GameObject)landmarkPlayerIsIn != desiredLandmark)
        {
            return;
        }

        if(isRiding)
        {
            state = NPCState.EGRESS;
            Debug.Log("<b>PARA PO!</b>");
        }
        else
        {
            Destroy(gameObject);
        }

        
    }

    public void GetOffVehicle(Component sender, object landmarkPlayerIsIn)
    {
        if (state != NPCState.EGRESS)
        {
            return;
        }
        
        
        onPedestrianEgress.Raise(this, gameObject);
        
        currentWaypoint = playersWaypoint ?? currentWaypoint; 

        if (!changeDirection)
        {
            if (currentWaypoint.nextWaypoint != null)
            {
                currentWaypoint = currentWaypoint.nextWaypoint; 
            }
            else
            {
                Destroy(gameObject); 
            }
        }
        else
        {
            if (currentWaypoint.previousWaypoint != null)
            {
                currentWaypoint = currentWaypoint.previousWaypoint;
            }
            else
            {
                Destroy(gameObject); 
            }
        }
        isRiding = false;
        SetDestination(currentWaypoint.GetPosition());
    }
    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Player") && canBeViolated)
        {
            onImpactWithPlayer.Raise(this, gameObject);
        }
    }
}