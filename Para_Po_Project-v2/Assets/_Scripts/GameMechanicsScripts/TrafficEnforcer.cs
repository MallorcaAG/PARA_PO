using Codice.CM.SEIDInfo;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TrafficEnforcer : MonoBehaviour
{
    [Header("Violations Cost")]
        [Tooltip("BE SURE TO INCLUDE THE NEGATIVE SIGN")]
    [SerializeField] private float runningOverPedestrians = -150;
        [Tooltip("BE SURE TO INCLUDE THE NEGATIVE SIGN")]
    [SerializeField] private float hittingAnotherVehicle = -100;
        [Tooltip("BE SURE TO INCLUDE THE NEGATIVE SIGN")]
    [SerializeField] private float counterflowingOrDrivingOnSidewalk = -100;
        [Tooltip("BE SURE TO INCLUDE THE NEGATIVE SIGN")]
    [SerializeField] private float beatingRedLight = -50;
        [Tooltip("BE SURE TO INCLUDE THE NEGATIVE SIGN")]
    [SerializeField] private float crashingIntoBuilding = -150;
        [Tooltip("BE SURE TO INCLUDE THE NEGATIVE SIGN")]
    [SerializeField] private float obstructingTrafficOrStalling = -50;
        [Tooltip("BE SURE TO INCLUDE THE NEGATIVE SIGN")]
    [SerializeField] private float speeding = -50;
    [Header("Immunity")]
    [SerializeField] private float immunityCooldown = 3f;
    [SerializeField] private bool immune = false;
    [Header("Game Events")]
    [SerializeField] private GameEvent onTrafficViolationCommitted;
    [Header("References")]
    [SerializeField] private GameObject player;

    private float targetTime;

    private void Start()
    {
        targetTime = immunityCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if(immune)
        {
            immune = !Timer();  //Once timer finishes, set immune to false
        }
  
    }

    public void TrafficViolationCommitted(Component sender, object data)
    {
        if(!immune)
        {
            Debug.LogWarning("VIOLATION COMMITTED");
            onTrafficViolationCommitted.Raise(this, PenaltyBasedOnViolationType(sender, data));

            immune = true;
        }
    }

    private float PenaltyBasedOnViolationType(Component sender, object data)
    {
        //REORDER THESE FROM WORST TO MILD VIOLATION

        if(sender.TryGetComponent<PedestrianAINavigator>(out PedestrianAINavigator peds))
        {
            Debug.LogWarning("Violation Type: Ran over a pedestrian");
            return runningOverPedestrians;
        }
        else if (sender.TryGetComponent<Buildings>(out Buildings property))
        {
            Debug.LogWarning("Violation Type: property damage");
            return crashingIntoBuilding;
        }
        else if (sender.TryGetComponent<LaneReader>(out LaneReader flow))
        {
            Debug.LogWarning("Violation Type: Counter Flowing Or Driving On Sidewalk");
            return counterflowingOrDrivingOnSidewalk;
        }

        return 0;
    }

    private bool Timer()
    {
        targetTime -= Time.deltaTime;

        if(targetTime <= 0.0f)
        {
            targetTime = immunityCooldown;
            return true;
        }

        return false;
    }
}
