using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{

    [Header("Game Events")]
    [SerializeField] private GameEvent shareMyWaypoint;
    [Header("Variables")]
    [SerializeField] private int routeCheckpoint;
    [SerializeField] private GameObject myWaypoint;
    /*    [SerializeField] private List<GameObject> mySeats;*/
    [SerializeField] private List<GameObject> myPassengers;

    private void Start()
    {
        shareMyWaypoint.Raise(this, myWaypoint);
    }

    public void PedestrianIngress(Component sender, object data)
    {
        GameObject obj = (GameObject)data;
        //INSTANTIATE POOFING VFX OR CALL DIFFERENT GAME EVENT TO INSTANTIATE THE VFX
        obj.transform.SetParent(myWaypoint.transform);
        obj.transform.localPosition = new Vector3(0, obj.transform.position.y - 100, 0);
        //Turn off colliders when implemented

    }

    public void PedestrianEgress(Component sender, object data)
    {
        GameObject obj = (GameObject)data;
        //INSTANTIATE POOFING VFX OR CALL DIFFERENT GAME EVENT TO INSTANTIATE THE VFX
        obj.transform.localPosition = new Vector3(0, obj.transform.position.y + 100, 0);
        obj.transform.parent = null;
        //Turn off colliders when implemented

    }

    public void IncrementRouteTravelled(Component sender, object data)
    {
        Landmark obj = ((GameObject)data).GetComponent<Landmark>();

        if(!obj.PlayerPassedByBefore())
        {
            routeCheckpoint++;
        }
    }
}
