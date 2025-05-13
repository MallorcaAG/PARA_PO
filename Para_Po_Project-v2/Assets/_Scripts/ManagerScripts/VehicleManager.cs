using PlasticGui.EventTracking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{

    [Header("Game Events")]
    [SerializeField] private GameEvent shareMyWaypoint;
    [SerializeField] private GameEvent onPassengerCountChange;
    [SerializeField] private GameEvent playerVehiclePassengerStatus;
    [Header("Variables")]
    [SerializeField] private int routeCheckpoint;
    [SerializeField] private int maxPassengers = 1;
    [SerializeField] private GameObject VFXPrefab;
    [SerializeField] private GameObject myWaypoint;
    /*    [SerializeField] private List<GameObject> mySeats;*/
    [SerializeField] private List<GameObject> myPassengers;

    private Landmark checkpoint;

    public Landmark getCheckpoint()
    {
        return checkpoint;
    }


    private void Start()
    {
        SendMyWaypoint();
        sendPlayerVehiclePassengerStatus();

    }

    private void Update()
    {
        sendPlayerVehiclePassengerStatus();
    }

    public void SendMyWaypoint()
    {
        shareMyWaypoint.Raise(this, myWaypoint);
    }
    public void sendPlayerVehiclePassengerStatus()
    {
        Vector2 vals = new Vector2();
        vals.x = myPassengers.Count;
        vals.y = maxPassengers;

        playerVehiclePassengerStatus.Raise(this, vals);
    }
    public void PedestrianIngress(Component sender, object data)
    {
        GameObject obj = (GameObject)data;

        // Ensure vehicle is not full
        if (myPassengers.Count < maxPassengers)
        {
            // INSTANTIATE POOFING VFX OR CALL DIFFERENT GAME EVENT TO INSTANTIATE THE VFX
            GameObject vfx = Instantiate(VFXPrefab, myWaypoint.transform);
            Destroy(vfx, 4f);

            myPassengers.Add(obj);
            onPassengerCountChange.Raise(this, myPassengers.Count);
            Debug.Log("Passenger Count: " + myPassengers.Count);

            obj.transform.SetParent(myWaypoint.transform);
            obj.transform.localPosition = new Vector3(0, obj.transform.position.y - 100, 0);
        }
    }

    public void PedestrianEgress(Component sender, object data)
    {
        GameObject obj = (GameObject)data;

        // Ensure the pedestrian is egressing correctly
        if (myPassengers.Contains(obj))
        {
            // INSTANTIATE POOFING VFX OR CALL DIFFERENT GAME EVENT TO INSTANTIATE THE VFX
            GameObject vfx = Instantiate(VFXPrefab, myWaypoint.transform);
            Destroy(vfx, 4f);

            myPassengers.Remove(obj);
            onPassengerCountChange.Raise(this, myPassengers.Count);
            Debug.Log("Passenger Count: " + myPassengers.Count);

            obj.transform.parent = null;
            obj.transform.position = new Vector3(myWaypoint.transform.position.x, myWaypoint.transform.position.y + 0.1f, myWaypoint.transform.position.z); // Adjusted position
        }
    }

    public void IncrementRouteTravelled(Component sender, object data)
    {
        Landmark obj = ((GameObject)data).GetComponent<Landmark>();

        if(!obj.PlayerPassedByBefore())
        {
            checkpoint = obj;
        }
    }
}
