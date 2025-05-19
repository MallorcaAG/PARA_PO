﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianAINavigator : WaypointNavigator
{
    private enum NPCState { STOPPED, WAITING, WALKING, RIDING, INGRESS, EGRESS }

    [Header("Behavior")]
    [SerializeField] private NPCState state, currentState;
    [SerializeField] private GameObject myLandmark;
    [SerializeField] private GameObject desiredLandmark;
    [SerializeField] private Animator animator;
    [SerializeField] private float deathFromImpactWithPlayerTimer = 5f;
    [SerializeField] private float egressDelay = 1.0f;

    [Header("Game Event")]
    [SerializeField] private GameEvent onPedestrianIngress;
    [SerializeField] private GameEvent onPedestrianEgress;
    [SerializeField] private GameEvent onImpactWithPlayer;
    [SerializeField] private GameEvent onSuccessfulEgress;
    [SerializeField] private GameEvent onSFXPlay;
    [SerializeField] private GameEvent onParaPo;
    [SerializeField] private GameEvent onMaxPassengerCapacityReached;
    [SerializeField] private GameEvent onAddPoints;

    [Header("References")]
    [SerializeField] private AudioClip voiceline;
    [SerializeField] private GameObject myUIIndicator;

    private static readonly int Idle0 = Animator.StringToHash("Idle0");
    private static readonly int Idle1 = Animator.StringToHash("Idle1");
    private static readonly int Idle2 = Animator.StringToHash("Idle2");
    private static readonly int Walk1 = Animator.StringToHash("Walk1");
    private static readonly int Walk2 = Animator.StringToHash("Walk2");
    private static readonly int Walk3 = Animator.StringToHash("Walk3");
    private static readonly int Jog = Animator.StringToHash("Jog");

    private Waypoint playersWaypoint;
    private PedestrianAISensors senses;
    private Rigidbody myRB;

    bool canBeViolated = true;
    bool isRiding = false;
    private int walkPersonality = 0;
    private int idlePersonality = 0;
    private bool allowIngress = false, dying = false;
    private NPCDistanceToPlayer destroyer;
    private Vector2 playerVehiclePassengerStatus;
    private int ingressAttempts = 0;

    // Queue management for egress handling
    private static Queue<PedestrianAINavigator> egressQueue = new Queue<PedestrianAINavigator>();
    private static bool isEgressInProgress = false;

    #region Setter/Getters
    public void setMyLandmark(GameObject newLandmark) => myLandmark = newLandmark;
    public void setDesiredLandmark(GameObject newLandmark) => desiredLandmark = newLandmark;
    public void setPlayersWaypointRef(Component component, object data)
    {
        GameObject obj = (GameObject)data;
        playersWaypoint = obj.GetComponent<Waypoint>();
    }
    public Waypoint getPlayersWaypoint() => playersWaypoint;
    public void setPlayerVehiclePassengerStatus(Component sender, object data) => playerVehiclePassengerStatus = (Vector2)data;
    public void AllowIngress(bool con) => allowIngress = con;
    public int getNPCState()
    {
        return currentState switch
        {
            NPCState.INGRESS => 0,
            NPCState.EGRESS => 1,
            _ => -1,
        };
    }
    public void setMyUIIndicatorReference(GameObject ui)
    {
        myUIIndicator = ui;
    }
    public GameObject MyUIIndicator { get { return myUIIndicator; } }
    #endregion

    #region Game loop functions
    void Start()
    {
        destroyer = gameObject.GetComponent<NPCDistanceToPlayer>();
        myRB = GetComponent<Rigidbody>();
        senses = GetComponent<PedestrianAISensors>();

        walkPersonality = Random.Range(0, 5);
        idlePersonality = Random.Range(0, 3);

        state = currentWaypoint == null ? NPCState.WAITING : NPCState.WALKING;
        SetDestination(currentWaypoint?.GetPosition() ?? transform.position);

        baseSpeed = personalityToWalkSpeed();

        setRigidbodyState(true);
        setColliderState(true);
    }

    void Update()
    {
        switch (state)
        {
            case NPCState.STOPPED:
            case NPCState.WAITING:
                if (state != currentState)
                {
                    animator.CrossFade(personalityToIdleAnimation(), 0f);
                    currentState = state;
                }
                break;

            case NPCState.WALKING:
                if (state != currentState)
                {
                    animator.CrossFade(personalityToWalkAnimation(), 0f);
                    NPCWalking();
                    currentState = state;
                }
                break;

            case NPCState.INGRESS:
                currentState = state;
                if (controller.destinationInfo.reachedDestination)
                {

                    if (allowIngress)
                    {
                        if (playerVehiclePassengerStatus.x >= playerVehiclePassengerStatus.y)
                        {
                            BarredEntryToVehicle(this, gameObject);
                            //onMaxPassengerCapacityReached.Raise(this, true);
                        }
                        else
                        {
                            //onMaxPassengerCapacityReached.Raise(this, false);
                            Debug.Log("Initiating Ingressing Proceedure: " + gameObject.name);
                            animator.CrossFade(personalityToIdleAnimation(), 0f);
                            SetState(NPCState.RIDING);
                            isRiding = true;
                            myRB.useGravity = false;
                            onPedestrianIngress.Raise(this, gameObject);
                            SetDestination(Vector3.zero);
                            controller.destinationInfo.reachedDestination = false;
                        }
                    }
                    else
                    {
                        //Try again
                        ingressAttempts++;
                        if (ingressAttempts <= 3)
                        {
                            Debug.Log("Reattempting to ingress");
                            senses.enabled = false;
                            SetDestination(playersWaypoint.GetPosition());
                            canBeViolated = false;
                        }
                        else
                        {
                            Debug.Log("Initializing \"I give up\"");
                            SetState(NPCState.WAITING);
                            SetDestination(transform.position);
                            StartCoroutine(kys());
                        }

                    }
                }
                break;

            case NPCState.RIDING:
                currentState = state;
                break;

            case NPCState.EGRESS:
                currentState = state;
                // Egress is handled by queue system; no per-frame logic needed here
                //Yes but it doesnt handle when they reach the sidewalk
                if (controller.destinationInfo.reachedDestination)
                {
                    SetState(NPCState.WALKING);
                    senses.enabled = true;

                    StartCoroutine(EnableCanBeViolatedDelayed(3f));

                    myLandmark = null;
                    desiredLandmark = null;
                    playersWaypoint = null;
                    //onSuccessfulEgress.Raise(this, gameObject);
                }
                break;
        }

        //Dont delete comment. Still testing something
        /*if (playerVehiclePassengerStatus.x >= playerVehiclePassengerStatus.y)
        {
            onMaxPassengerCapacityReached.Raise(this, true);
        }
        else
        {
            onMaxPassengerCapacityReached.Raise(this, false);
        }*/
    }

    private void NPCWalking() => NPCTraversal();

    public void pivot(Vector3 sense, float maxDistance)
    {
        Vector3 pivotPos = transform.TransformPoint(sense * maxDistance);
        SetDestination(pivotPos);
    }
    #endregion
    #region Animation Personality
    private int personalityToIdleAnimation() => idlePersonality switch
    {
        0 => Idle0,
        1 => Idle1,
        2 => Idle2,
        _ => Idle0
    };

    private int personalityToWalkAnimation() => walkPersonality switch
    {
        0 => Walk1,
        1 => Walk2,
        2 => Walk3,
        4 => Jog,
        _ => Walk1
    };

    private float personalityToWalkSpeed() => walkPersonality switch
    {
        0 => 1.8f,
        1 => 1.2f,
        2 => 0.7f,
        4 => 3f,
        _ => 0.8f
    };
    #endregion

    #region Ingress and Egress Functions
    public void GetOnVehicle(Component component, object landmarkPlayerIsIn)
    {
        if ((GameObject)landmarkPlayerIsIn != myLandmark || desiredLandmark == null) return;
        if (playerVehiclePassengerStatus.x >= playerVehiclePassengerStatus.y)
        {
            //Debug.Log("Vehicle is full so " + gameObject.name + " cant get on");

            //onMaxPassengerCapacityReached.Raise(this, true);

            return;
        }


        //onMaxPassengerCapacityReached.Raise(this, false);

        if (state == NPCState.WAITING || state == NPCState.WALKING)
        {
            //Debug.Log("Getting on Vehicle: "+gameObject.name);

            animator.CrossFade(personalityToWalkAnimation(), 0f);
            senses.enabled = false;
            SetState(NPCState.INGRESS);
            SetDestination(playersWaypoint.GetPosition());
            canBeViolated = false;
        }
        else
        {
            Debug.Log(gameObject.name + ": Attempted to board while in invalid state: " + state);
        }
    }

    public void BarredEntryToVehicle(Component sender, object data)
    {
        GameObject ped = (GameObject)data;

        if (ped == this.gameObject)
        {
            Debug.LogWarning("I have been barred entry");
            SetState(NPCState.WAITING);
            canBeViolated = false;
            isRiding = false;
            allowIngress = false;
            myRB.useGravity = true;
            SetDestination(transform.position);
        }
    }
    public void LandmarkReached(Component sender, object landmarkPlayerIsIn)
    {
        if (desiredLandmark == null || (GameObject)landmarkPlayerIsIn != desiredLandmark) return;

        if (isRiding)
        {
            SetState(NPCState.EGRESS);

            // Ensure currentWaypoint is set before egress
            if (currentWaypoint == null && playersWaypoint != null)
            {
                currentWaypoint = playersWaypoint;
            }

            if (!egressQueue.Contains(this))
            {
                egressQueue.Enqueue(this);
            }

            onSFXPlay?.Raise(this, voiceline);
            onParaPo?.Raise(this, 0);
        }
        else
        {
            StartCoroutine(kys());
        }
    }


    public void GetOffVehicle(Component sender, object landmarkPlayerIsIn)
    {
        if (state != NPCState.EGRESS) return;

        //Debug.Log("Egress in progress = " + isEgressInProgress);
        GameObject landmark = (GameObject)landmarkPlayerIsIn;

        if (!isEgressInProgress)
        {
            ProcessNextEgress(landmark);
        }
    }


    private void ProcessNextEgress(GameObject landmark)
    {
        if (egressQueue.Count == 0)
        {
            Debug.LogWarning("Egress Queue is empty\n");
            isEgressInProgress = false;
            return;
        }

        isEgressInProgress = true;
        PedestrianAINavigator nextPedestrian = egressQueue.Peek();

        if (nextPedestrian.currentWaypoint == null)
        {
            if (nextPedestrian.playersWaypoint != null)
            {
                nextPedestrian.currentWaypoint = nextPedestrian.playersWaypoint;
                Debug.Log($"{nextPedestrian.name}: currentWaypoint was null, assigned playersWaypoint '{nextPedestrian.currentWaypoint.name}' as currentWaypoint.");
            }
            else
            {
                Debug.LogError($"{nextPedestrian.name}: playersWaypoint is null. Cannot assign currentWaypoint. Skipping.");
                egressQueue.Dequeue();
                isEgressInProgress = false;
                return;
            }
        }
        else
        {
            Debug.Log($"{nextPedestrian.name}: currentWaypoint is '{nextPedestrian.currentWaypoint.name}'.");
        }

        Waypoint target = !nextPedestrian.changeDirection ?
                          nextPedestrian.currentWaypoint.nextWaypoint :
                          nextPedestrian.currentWaypoint.previousWaypoint;

        if (target == null)
        {
            Debug.LogWarning($"{nextPedestrian.name}: No connected waypoint from currentWaypoint '{nextPedestrian.currentWaypoint.name}'. Finding nearest waypoint instead.");

            // Find nearest waypoint to the pedestrian's current position
            target = FindNearestWaypoint(nextPedestrian.transform.position);

            if (target == null)
            {
                Debug.LogError($"{nextPedestrian.name}: No nearest waypoint found. Skipping egress.");
                egressQueue.Dequeue();
                isEgressInProgress = false;
                return;
            }
            else
            {
                Debug.Log($"{nextPedestrian.name}: Nearest waypoint found '{target.name}'.");
            }
        }
        else
        {
            Debug.Log($"{nextPedestrian.name}: Moving from currentWaypoint '{nextPedestrian.currentWaypoint.name}' to target waypoint '{target.name}'.");
        }

        nextPedestrian.currentWaypoint = target;

        Debug.Log($"{nextPedestrian.name}: Updated currentWaypoint to '{nextPedestrian.currentWaypoint.name}'. Beginning ExecuteEgress.");
        nextPedestrian.ExecuteEgress(landmark);
    }

    private Waypoint FindNearestWaypoint(Vector3 position)
    {
        Waypoint[] allWaypoints = FindObjectsOfType<Waypoint>();
        Waypoint nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (var waypoint in allWaypoints)
        {
            float dist = Vector3.Distance(position, waypoint.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = waypoint;
            }
        }

        return nearest;
    }




    // Execute the egress process for pedestrian in front of Queue
    private void ExecuteEgress(GameObject landmark)
    {


        // Raise event to notify egress has started for this pedestrian
        onPedestrianEgress?.Raise(this, gameObject);

        // Remove excemption to despawning in relation to player distance
        gameObject.GetComponent<NPCDistanceToPlayer>().excempted = false;

        // Animation change
        animator.CrossFade(personalityToWalkAnimation(), 0f);

        // Reappear passenger back onto GameWorld
        transform.parent = null;
        // ! Is offset still necessary if theres already a delay on their egress?
        //Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f)); 
        transform.position = new Vector3(
            playersWaypoint.transform.position.x,
            playersWaypoint.transform.position.y + 0.15f,
            playersWaypoint.transform.position.z
        ) /*+ offset*/;

        Debug.Log(gameObject.name + ": (PlayerWaypoint)" + playersWaypoint.name);
        Debug.Log(gameObject.name + ": (CurrentWaypoint)" + currentWaypoint.name);

        //Physics change
        myRB.useGravity = true;

        isRiding = false;
        SetDestination(currentWaypoint.GetPosition());

        // Start coroutine to finish egress after delay, then process next queue member
        StartCoroutine(FinishEgressAfterDelay(egressDelay, landmark));
    }

    // Delay to avoid egress overlap and allow clean exit animations
    private IEnumerator FinishEgressAfterDelay(float delay, GameObject landmark)
    {
        //Wait for delay
        yield return new WaitForSeconds(delay);

        // Remove self from queue once finished egress
        if (egressQueue.Count > 0 && egressQueue.Peek() == this)
        {
            egressQueue.Dequeue();
        }

        // Check and continue processing next pedestrian for egress if any
        if (egressQueue.Count > 0)
        {
            ProcessNextEgress(landmark);
        }
        else
        {
            isEgressInProgress = false;
        }

        // Notify general successful egress (optional game event)
        if (landmark == desiredLandmark)
        {
            onSuccessfulEgress.Raise(this, gameObject);
        }
        else
        {
            //Only give 1 point
            onAddPoints.Raise(this, 1f);
        }
        //onMaxPassengerCapacityReached.Raise(this, false);
    }
    #endregion

    #region Ragdoll and Collisions
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player") && canBeViolated)
        {
            onImpactWithPlayer.Raise(this, gameObject);
            EnableRagdoll();
            StartCoroutine(kys());
        }
    }

    private void EnableRagdoll()
    {
        if (animator != null) animator.enabled = false;

        if (MyUIIndicator != null)
        {
            myUIIndicator.SetActive(false);
        }

        DisableNavigation();
        setRigidbodyState(false);
        setColliderState(true);
        ZeroOutRigidbodyVelocity();
        transform.parent = null;

        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.AddForce(Vector3.back * 2f, ForceMode.Impulse);
        }
    }
    private IEnumerator EnableCanBeViolatedDelayed(float delay)
    {
        float countdown = delay;
        while (countdown > 0f)
        {
            Debug.Log($"canBeViolated will be set in {Mathf.CeilToInt(countdown)} second(s)...");
            yield return new WaitForSeconds(1f);
            countdown -= 1f;
        }

        canBeViolated = true;
        Debug.Log("canBeViolated is now TRUE.");
    }
    private IEnumerator kys()
    {
        yield return new WaitForSeconds(deathFromImpactWithPlayerTimer);
        dying = true;
        destroyer.kys();
    }

    private void setRigidbodyState(bool state)
    {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>()) rb.isKinematic = state;
    }

    private void setColliderState(bool state)
    {
        foreach (Collider col in GetComponentsInChildren<Collider>()) col.enabled = state;
    }

    private void ZeroOutRigidbodyVelocity()
    {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void DisableNavigation()
    {
        controller.destinationInfo.reachedDestination = true;
        controller.enabled = false;
        SetDestination(transform.position);
    }
    #endregion

    #region State Changes
    private void SetState(NPCState newState)
    {
        state = newState;
    }

    public void changeStateToIdle() => SetState(NPCState.STOPPED);
    public void changeStateToWalk() => SetState(NPCState.WALKING);
    public bool isWaiting() => state == NPCState.WAITING;
    #endregion
}