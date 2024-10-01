using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianAINavigator : MonoBehaviour
{
    private enum NPCState
    {
        WAITING, WALKING, RIDING, INGRESS, EGRESS
    };

    [Header("Pathfinding")]
    [SerializeField] public Waypoint currentWaypoint;
    [SerializeField] private bool changeDirection = false;

    [Header("Behavior")]
    [SerializeField] private NPCState state;
    [SerializeField] private GameObject myLandmark;
    [SerializeField] private GameObject desiredLandmark;

    [Header("Game Event")]
    [SerializeField] private GameEvent onPedestrianIngress;
    [SerializeField] private GameEvent onPedestrianEgress;

    private Waypoint playersWaypoint;
    private CharacterNav controller;

    int direction;

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

    public CharacterNav getController()
    {
        return controller;
    }
    #endregion

    #region Runtime Functions
    // Start is called before the first frame update
    void Start()
    {
        direction = Mathf.RoundToInt(Random.Range(0f, 1f)); 
        controller = GetComponent<CharacterNav>();

        if (currentWaypoint == null)
        {
            state = NPCState.WAITING;
            controller.SetDestination(transform.position);
        }
        else
        {
            state = NPCState.WALKING;
            controller.SetDestination(currentWaypoint.GetPosition());
        }
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
                    onPedestrianIngress.Raise(this, gameObject);
                }
                break;

            case NPCState.RIDING:
                break;

            case NPCState.EGRESS:
                if (controller.destinationInfo.reachedDestination)
                {
                    state = NPCState.WALKING;
                    myLandmark = null;
                    desiredLandmark = null;
                    playersWaypoint = null;
                }
                break;
        }
    }
    #endregion

    private void NPCWalking()
    {
        if (controller.destinationInfo.reachedDestination)
        {
            bool shouldBranch = false;

            if(currentWaypoint.branches != null && currentWaypoint.branches.Count > 0)
            {
                shouldBranch = Random.Range(0f, 1f) <= currentWaypoint.branchRatio ? true : false;
            }

            if (shouldBranch)
            {
                currentWaypoint = currentWaypoint.branches[Random.Range(0, currentWaypoint.branches.Count - 1)];
            }
            else
            {
                if (direction == 0)
                {
                    if (currentWaypoint.nextWaypoint != null)
                    {
                        currentWaypoint = currentWaypoint.nextWaypoint;
                    }
                    else
                    {
                        currentWaypoint = currentWaypoint.previousWaypoint;
                        direction = 1;
                    }
                }
                else if (direction == 1)
                {
                    if (currentWaypoint.previousWaypoint != null)
                    {
                        currentWaypoint = currentWaypoint.previousWaypoint;
                    }
                    else
                    {
                        currentWaypoint = currentWaypoint.nextWaypoint;
                        direction = 0;

                    }
                }
            }
            controller.SetDestination(currentWaypoint.GetPosition());
        }
    }

    public void GetOnVehicle(Component component, object landmarkPlayerIsIn)
    {
        if ((GameObject)landmarkPlayerIsIn != myLandmark)
        {
            return;
        }

        state = NPCState.INGRESS;
        controller.SetDestination(playersWaypoint.GetPosition());
    }

    public void LandmarkReached(Component sender, object landmarkPlayerIsIn)
    {
        if ((GameObject)landmarkPlayerIsIn != desiredLandmark)
        {
            return;
        }

        state = NPCState.EGRESS;
        Debug.Log("<b>PARA PO!</b>");
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

        controller.SetDestination(currentWaypoint.GetPosition());
    }
}