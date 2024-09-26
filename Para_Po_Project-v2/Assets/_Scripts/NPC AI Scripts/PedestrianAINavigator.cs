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
    [SerializeField] private Waypoint currentWaypoint;
    [SerializeField] private bool changeDirection = false;
    [Header("Behavior")]
    [SerializeField] private NPCState state;
    [SerializeField] private GameObject myLandmark;
    [SerializeField] private GameObject desiredLandmark;
    [Header("Game Event")]
    [SerializeField] private GameEvent onPedestrianIngress;

    private Waypoint playersWaypoint;
    private CharacterNav controller;

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
        controller = GetComponent<CharacterNav>();

        if(currentWaypoint == null)
        {
            state = NPCState.WAITING;

            controller.SetDestination(gameObject.transform.position);
        }
        else if(currentWaypoint != null)
        {
            state = NPCState.WALKING;

            controller.SetDestination(currentWaypoint.GetPosition());
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case NPCState.WAITING:

                break;

            case NPCState.WALKING:
                NPCWalking();
                break;

            case NPCState.INGRESS:
                if(controller.destinationInfo.reachedDestination)
                {
                    //INSTANTIATE POOFING VFX OR CALL ANOTHER DIFFERENT GAME EVENT TO INSTANTIATE THE VFX

                    state= NPCState.RIDING;

                    onPedestrianIngress.Raise(this, this.gameObject);
                }
                break;

            case NPCState.RIDING:

                break;

            case NPCState.EGRESS:

                break;

            
        }
    }
    #endregion

    private void NPCWalking()
    {
        if (controller.destinationInfo.reachedDestination)
        {
            if (!changeDirection)
            {
                currentWaypoint = currentWaypoint.nextWaypoint;
            }
            else
            {
                currentWaypoint = currentWaypoint.previousWaypoint;
            }

            controller.SetDestination(currentWaypoint.GetPosition());
        }
    }

    public void GetOnVehicle(Component component, object landmarkPlayerIsIn)
    {
        if ((GameObject)landmarkPlayerIsIn != myLandmark)
        {
            Debug.Log("Oh whoops not me kys");
            return;
        }
        

        state = NPCState.INGRESS;
        
        controller.SetDestination(playersWaypoint.GetPosition());
    }

    private void GetOffVehicle()
    {
        
    }
}
