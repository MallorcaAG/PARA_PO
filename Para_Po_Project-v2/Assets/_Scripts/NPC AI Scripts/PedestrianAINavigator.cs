using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class PedestrianAINavigator : WaypointNavigator
{
    private enum NPCState
    {
        STOPPED, WAITING, WALKING, RIDING, INGRESS, EGRESS
    };

    [Header("Behavior")]
    [SerializeField] private NPCState state, currentState;
    [SerializeField] private GameObject myLandmark;
    [SerializeField] private GameObject desiredLandmark;
    [SerializeField] private Animator animator;

    [Header("Game Event")]
    [SerializeField] private GameEvent onPedestrianIngress;
    [SerializeField] private GameEvent onPedestrianEgress;
    [SerializeField] private GameEvent onImpactWithPlayer;
    [SerializeField] private GameEvent onSuccessfulEgress;

    private static readonly int Idle0 = Animator.StringToHash("Idle0");
    private static readonly int Idle1 = Animator.StringToHash("Idle1");
    private static readonly int Idle2 = Animator.StringToHash("Idle2");
    private static readonly int Walk1 = Animator.StringToHash("Walk1");
    private static readonly int Walk2 = Animator.StringToHash("Walk2");
    private static readonly int Walk3 = Animator.StringToHash("Walk3");
    private static readonly int Jog = Animator.StringToHash("Jog");

    private Waypoint playersWaypoint;
    private PedestrianAISensors senses;

    bool canBeViolated = true;
    bool isRiding = false;
    private int walkPersonality = 0;
    private int idlePersonality = 0;


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

        walkPersonality = Random.Range(0, 5);
        idlePersonality = Random.Range(0, 3);

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

        baseSpeed = personalityToWalkSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case NPCState.STOPPED:
                if (state == currentState) break;
                animator.CrossFade(personalityToIdleAnimation(), 0f);
                currentState = state;
                break;

            case NPCState.WAITING:
                if (state == currentState) break;
                animator.CrossFade(personalityToIdleAnimation(), 0f);
                currentState = state;
                break;

            case NPCState.WALKING:
                if (state == currentState) break;
                animator.CrossFade(personalityToWalkAnimation(),0f);
                NPCWalking();
                currentState = state;
                break;

            case NPCState.INGRESS:
                currentState = state;

                if (controller.destinationInfo.reachedDestination)
                {
                    animator.CrossFade(personalityToIdleAnimation(), 0f);
                    state = NPCState.RIDING;
                    isRiding = true;
                    onPedestrianIngress.Raise(this, gameObject);
                }
                break;

            case NPCState.RIDING:
                currentState = state;

                break;

            case NPCState.EGRESS:
                currentState = state;

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

    public void changeStateToIdle()
    {
        state = NPCState.STOPPED;
    }

    public void changeStateToWalk()
    {
        state = NPCState.WALKING;
    }

    public bool isWaiting()
    {
        switch(state)
        {
            case NPCState.WAITING:
                return true;

            default:
                return false;
        }
    }

    private int personalityToIdleAnimation()
    {
        switch(idlePersonality)
        {
            case 0:
                return Idle0;
                

            case 1:
                return Idle1;
                

            case 2:
                return Idle2;
                

        }

        return Idle0;
    }

    private int personalityToWalkAnimation()
    {
        switch (walkPersonality)
        {
            case 0:
                return Walk1;
                

            case 1:
                return Walk2;
                

            case 2:
                return Walk3;
                

            case 4:
                return Jog;
                
        }

        return Walk1;
    }

    private float personalityToWalkSpeed()
    {
        switch (walkPersonality)
        {
            case 0:
                return 1.8f;


            case 1:
                return 1.2f;


            case 2:
                return 0.7f;

            case 4:
                return 3f;

        }

        return 0.8f;
    }

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
        animator.CrossFade(personalityToWalkAnimation(), 0f);

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
        
        animator.CrossFade(personalityToWalkAnimation(),0f);


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