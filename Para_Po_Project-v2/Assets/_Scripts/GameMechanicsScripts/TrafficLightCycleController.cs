using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json.Bson;
using UnityEngine;

public class TrafficLightCycleController : MonoBehaviour
{
    [Header("Directions")]
    [SerializeField] private TrafficLightLaneManager north, south, east, west;
    [Header("Phases")]
    [SerializeField] private List<TrafficLightPhases> phases;

    float targetTime;
    int i = 0;

    private void Start()
    {
        i = 0;
        runCurrentPhase();
    }

    // Update is called once per frame
    void Update()
    {
        if(Timer())
            runCurrentPhase();
    }


    private bool Timer()
    {
        targetTime -= Time.deltaTime;

        if (targetTime <= 0.0f)
        {
            nextPhase();
            targetTime = phases[i].phaseDuration;
            return true;
        }

        return false;
    }

    private void nextPhase()
    {
        if(i < phases.Count - 1)
        {
            i++;
            return;
        }

        i = 0;
    }

    private void runCurrentPhase()
    {
        //HELP THIS IS SPAGETTI TwT

        //North
        if (north != null)
        {
            if (phases[i].northLeft)
            {
                north.LeftTurnGo();
            }
            else
            {
                north.LeftTurnStop();
            }

            if (phases[i].northRight)
            {
                north.RightTurnGo();
            }
            else
            {
                north.RightTurnStop();
            }

            if (phases[i].northStraight)
            {
                north.StraightThroughGo();
            }
            else
            {
                north.StraightThroughStop();
            }
        }

        //South
        if (south != null)
        {
            if (phases[i].southLeft)
            {
                south.LeftTurnGo();
            }
            else
            {
                south.LeftTurnStop();
            }

            if (phases[i].southRight)
            {
                south.RightTurnGo();
            }
            else
            {
                south.RightTurnStop();
            }

            if (phases[i].southStraight)
            {
                south.StraightThroughGo();
            }
            else
            {
                south.StraightThroughStop();
            }
        }

        //East
        if (east != null)
        {
            if (phases[i].eastLeft)
            {
                east.LeftTurnGo();
            }
            else
            {
                east.LeftTurnStop();
            }

            if (phases[i].eastRight)
            {
                east.RightTurnGo();
            }
            else
            {
                east.RightTurnStop();
            }

            if (phases[i].eastStraight)
            {
                east.StraightThroughGo();
            }
            else
            {
                east.StraightThroughStop();
            }
        }

        //West  
        if (west != null)
        {
            if (phases[i].westLeft)
            {
                west.LeftTurnGo();
            }
            else
            {
                west.LeftTurnStop();
            }

            if (phases[i].westRight)
            {
                west.RightTurnGo();
            }
            else
            {
                west.RightTurnStop();
            }

            if (phases[i].westStraight)
            {
                west.StraightThroughGo();
            }
            else
            {
                west.StraightThroughStop();
            }
        }
    }
}
