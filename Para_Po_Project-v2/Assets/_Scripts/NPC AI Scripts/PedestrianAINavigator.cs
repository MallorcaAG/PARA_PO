using System.Collections;
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

    [Header("Game Event")]
    [SerializeField] private GameEvent onPedestrianIngress;
    [SerializeField] private GameEvent onPedestrianEgress;
    [SerializeField] private GameEvent onImpactWithPlayer;
    [SerializeField] private GameEvent onSuccessfulEgress;
    [SerializeField] private GameEvent onSFXPlay;
    [SerializeField] private GameEvent onParaPo;

    [Header("References")]
    [SerializeField] private AudioClip voiceline;

    private static readonly int Idle0 = Animator.StringToHash("Idle0");
    private static readonly int Idle1 = Animator.StringToHash("Idle1");
    private static readonly int Idle2 = Animator.StringToHash("Idle2");
    private static readonly int Walk1 = Animator.StringToHash("Walk1");
    private static readonly int Walk2 = Animator.StringToHash("Walk2");
    private static readonly int Walk3 = Animator.StringToHash("Walk3");
    private static readonly int Jog = Animator.StringToHash("Jog");

    private static Queue<PedestrianAINavigator> egressQueue = new Queue<PedestrianAINavigator>();
    private static bool isEgressing = false;

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
                        animator.CrossFade(personalityToIdleAnimation(), 0f);
                        SetState(NPCState.RIDING);
                        isRiding = true;
                        myRB.useGravity = false;
                        onPedestrianIngress.Raise(this, gameObject);
                        SetDestination(Vector3.zero);
                        controller.destinationInfo.reachedDestination = false;
                    }
                    else
                    {
                        SetState(NPCState.WAITING);
                        SetDestination(transform.position);
                        StartCoroutine(kys());
                    }
                }
                break;

            case NPCState.RIDING:
                currentState = state;
                break;

            case NPCState.EGRESS:
                currentState = state;
                if (controller.destinationInfo.reachedDestination)
                {
                    SetState(NPCState.WALKING);
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

    private void NPCWalking() => NPCTraversal();

    public void pivot(Vector3 sense, float maxDistance)
    {
        Vector3 pivotPos = transform.TransformPoint(sense * maxDistance);
        SetDestination(pivotPos);
    }

    public void GetOnVehicle(Component component, object landmarkPlayerIsIn)
    {


        if ((GameObject)landmarkPlayerIsIn != myLandmark || desiredLandmark == null) return;

        if (playerVehiclePassengerStatus.x >= playerVehiclePassengerStatus.y)
        {

            return;
        }

        if (state == NPCState.WAITING || state == NPCState.WALKING)
        {
            animator.CrossFade(personalityToWalkAnimation(), 0f);
            senses.enabled = false;
            SetState(NPCState.INGRESS);
            SetDestination(playersWaypoint.GetPosition());
            canBeViolated = false;
        }
        else
        {
            Debug.Log("Attempted to board while in invalid state: " + state);
        }
    }

    public void LandmarkReached(Component sender, object landmarkPlayerIsIn)
    {
        if (desiredLandmark == null || (GameObject)landmarkPlayerIsIn != desiredLandmark) return;

        if (isRiding)
        {
            egressQueue.Enqueue(this);
            TryStartEgressQueue();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private static void TryStartEgressQueue()
    {
        if (!isEgressing && egressQueue.Count > 0)
        {
            PedestrianAINavigator nextPedestrian = egressQueue.Peek();
            if (nextPedestrian != null && nextPedestrian.desiredLandmark != null)
            {
                nextPedestrian.StartCoroutine(nextPedestrian.EgressWithDelayCoroutine());
            }
            else
            {
                egressQueue.Dequeue();
                TryStartEgressQueue();
            }
        }
    }

    private IEnumerator EgressWithDelayCoroutine()
    {
        isEgressing = true;

        yield return new WaitForSeconds(Random.Range(0.15f, 0.25f));

        if (desiredLandmark == null)
        {
            egressQueue.Dequeue();
            isEgressing = false;
            TryStartEgressQueue();
            yield break;
        }

        SetState(NPCState.EGRESS);
        onSFXPlay?.Raise(this, voiceline);
        onParaPo?.Raise(this, 0);
    }

    public void GetOffVehicle(Component sender, object landmarkPlayerIsIn)
    {
        if (state != NPCState.EGRESS) return;

        gameObject.GetComponent<NPCDistanceToPlayer>().excempted = false;
        animator.CrossFade(personalityToWalkAnimation(), 0f);

        transform.parent = null;
        Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
        transform.position = new Vector3(
            playersWaypoint.transform.position.x,
            playersWaypoint.transform.position.y + 0.15f,
            playersWaypoint.transform.position.z
        ) + offset;

        myRB.useGravity = true;
        onPedestrianEgress?.Raise(this, gameObject);

        currentWaypoint = playersWaypoint ?? currentWaypoint;
        currentWaypoint = !changeDirection ?
            currentWaypoint.nextWaypoint ?? currentWaypoint :
            currentWaypoint.previousWaypoint ?? currentWaypoint;

        isRiding = false;
        SetDestination(currentWaypoint.GetPosition());

        if (egressQueue.Count > 0 && egressQueue.Peek() == this)
            egressQueue.Dequeue();

        isEgressing = false;
        TryStartEgressQueue();
    }

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

    private void SetState(NPCState newState)
    {

        state = newState;
    }

    public void changeStateToIdle() => SetState(NPCState.STOPPED);
    public void changeStateToWalk() => SetState(NPCState.WALKING);
    public bool isWaiting() => state == NPCState.WAITING;
}