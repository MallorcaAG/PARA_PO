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
    [SerializeField] private float hittingSignPostTrafficObject = -100;
    [Tooltip("BE SURE TO INCLUDE THE NEGATIVE SIGN")]
    [SerializeField] private float crashingIntoBuilding = -150;
        [Tooltip("BE SURE TO INCLUDE THE NEGATIVE SIGN")]
    [SerializeField] private float obstructingTrafficStallingOrAFK = -50;
        [Tooltip("BE SURE TO INCLUDE THE NEGATIVE SIGN")]
    [SerializeField] private float speeding = -50;
        [Tooltip("BE SURE TO INCLUDE THE NEGATIVE SIGN")]
    [SerializeField] private float notUrFaultBonus = 50;
    [Header("Immunity")]
    [SerializeField] private float immunityCooldown = 3f;
    
    [SerializeField] private bool immune = false;
    [Header("Game Events")]
    [SerializeField] private GameEvent onTrafficViolationCommitted;
    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private float speedLimit = 16f;
    [Range(0f, 5f)][SerializeField] private float speedingGracePeriod = 3f;
    [Range(10f,60f)][SerializeField] private float obstructionChargeCooldown = 20f;


    private float targetTime, targetTime2, targetTime3;
    private bool justStarted = true;

    private void Start()
    {
        targetTime = immunityCooldown;

        resetTimer2();
        resetTimer3();
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
        else if (sender.TryGetComponent<TrafficObject>(out TrafficObject cone))
        {
            Debug.LogWarning("Violation Type: property damage");
            return hittingSignPostTrafficObject;
        }
        else if (sender.TryGetComponent<LaneReader>(out LaneReader flow))
        {
            Debug.LogWarning("Violation Type: Counter Flowing Or Driving On Sidewalk");
            return counterflowingOrDrivingOnSidewalk;
        }
        else if (sender.TryGetComponent<VehicleAINavigator>(out VehicleAINavigator car))
        {
            
            switch ((int)data)
            {
                case 0:
                    Debug.Log("YOU HAVE BEEN VIOLATED: Got hit by another vehicle\nDW ITS NOT UR FAULT BOZO");
                    return notUrFaultBonus;
                case 1:
                    Debug.LogWarning("Violation Type: Hit another vehicle");
                    return hittingAnotherVehicle;
                default:
                    return 0;

            }
        }
        else if (sender.TryGetComponent<TrafficLightViolation>(out TrafficLightViolation red))
        {
            Debug.LogWarning("Violation Type: Traffic Light Violation");
            return beatingRedLight;
        }
        else if (sender.TryGetComponent<TrafficEnforcer>(out TrafficEnforcer mmda))
        {

            switch ((int)data)
            {
                case 0:
                    Debug.Log("Violation Type: Speeding");
                    return speeding;

                case 1:
                    Debug.Log("Violation Type: Obstructing Traffic/Stalling/AFK");
                    return obstructingTrafficStallingOrAFK;

                default:
                    return 0;

            }
        }

        return 0;
    }

    public void checkPlayerSpeed(Component sender, object data)
    {

        /*Debug.Log((string)data + " " + data.GetType());*/

        float speed = float.Parse((string)data);

        

        if (speed >= speedLimit)
        {
            if(Timer2())
            {
                TrafficViolationCommitted(this, 0);
            }
        }
        else if (speed > 0.01f)
        {
            resetTimer2();
            resetTimer3();

            justStarted = false;
        }
        
        if (speed <= 0.01f && !justStarted)
        {
            if (Timer3())
            {
                TrafficViolationCommitted(this, 1);
            }
        }
        
        
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

    private bool Timer2()
    {
        targetTime2 -= 1f;

        if (targetTime2 <= 0.0f)
        {
            resetTimer2();
            return true;
        }

        return false;
    }

    private void resetTimer2()
    {
        targetTime2 = speedingGracePeriod;
    }

    private bool Timer3()
    {
        targetTime3 -= 1f;

        if (targetTime3 <= 0.0f)
        {
            resetTimer3();
            return true;
        }

        return false;
    }

    private void resetTimer3()
    {
        targetTime3 = obstructionChargeCooldown;
    }
}
