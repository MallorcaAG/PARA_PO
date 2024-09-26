using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    [Header("Game Events")]
    [SerializeField] private GameEvent shareMyWaypoint;
    [Space]
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
        obj.transform.SetParent(myWaypoint.transform);
        obj.transform.localPosition = new Vector3(0, obj.transform.position.y, 0);
        obj.SetActive(false); //Problematic cause we need the script to still be active for Egress functions. FIX LATER
    }
}
