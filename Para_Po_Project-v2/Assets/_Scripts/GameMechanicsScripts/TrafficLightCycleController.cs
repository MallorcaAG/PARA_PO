using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json.Bson;
using UnityEngine;

public class TrafficLightCycleController : MonoBehaviour
{
    [Header("Directions")]
    [SerializeField] private TrafficLightLaneManager north, south, east, west;
    [Header("Red Light Violation Trigger")]
    [SerializeField] private GameObject isPlayerRedObj;
    [Header("Phases")]
    [SerializeField] private List<TrafficLightPhases> phases;
    [Header("Lights")]
    [SerializeField] private GameObject northStraightRed;
    [SerializeField] private GameObject northStraightYellow;
    [SerializeField] private GameObject northStraightGreen;
    [SerializeField] private GameObject northLeftRed;
    [SerializeField] private GameObject northLeftYellow;
    [SerializeField] private GameObject northLeftGreen;
    [SerializeField] private GameObject southStraightRed;
    [SerializeField] private GameObject southStraightYellow;
    [SerializeField] private GameObject southStraightGreen;
    [SerializeField] private GameObject southLeftRed;
    [SerializeField] private GameObject southLeftYellow;
    [SerializeField] private GameObject southLeftGreen;
    [SerializeField] private GameObject eastStraightRed;
    [SerializeField] private GameObject eastStraightYellow;
    [SerializeField] private GameObject eastStraightGreen;
    [SerializeField] private GameObject eastLeftRed;
    [SerializeField] private GameObject eastLeftYellow;
    [SerializeField] private GameObject eastLeftGreen;
    [SerializeField] private GameObject westStraightRed;
    [SerializeField] private GameObject westStraightYellow;
    [SerializeField] private GameObject westStraightGreen;
    [SerializeField] private GameObject westLeftRed;
    [SerializeField] private GameObject westLeftYellow;
    [SerializeField] private GameObject westLeftGreen;
    [Header("Settings")]
    [Range(0.1f,0.5f)][SerializeField] private float yellowLightTransition;

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
            //MAMAMIA ITSA SPAGHETTI TIME

        if (phases[i].isPlayerRed)
        {
            isPlayerRedObj.SetActive(true);
        }
        else
        {
            isPlayerRedObj.SetActive(false);
        }

        //North
        if (north != null)
        {
            if (phases[i].northLeft)
            {
                north.LeftTurnGo();

                if (targetTime <= (phases[i].phaseDuration * yellowLightTransition))
                {
                    northLeftRed.SetActive(false);
                    northLeftYellow.SetActive(true);
                    northLeftGreen.SetActive(false);
                }
                else
                {
                    northLeftRed.SetActive(false);
                    northLeftYellow.SetActive(false);
                    northLeftGreen.SetActive(true);
                }

            }
            else
            {
                north.LeftTurnStop();

                northLeftRed.SetActive(true);
                northLeftYellow.SetActive(false);
                northLeftGreen.SetActive(false);
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

                if (targetTime <= (phases[i].phaseDuration * yellowLightTransition))
                {
                    northStraightRed.SetActive(false);
                    northStraightYellow.SetActive(true);
                    northStraightGreen.SetActive(false);
                }
                else
                {
                    northStraightRed.SetActive(false);
                    northStraightYellow.SetActive(false);
                    northStraightGreen.SetActive(true);
                }
            }
            else
            {
                north.StraightThroughStop();

                northStraightRed.SetActive(true);
                northStraightYellow.SetActive(false);
                northStraightGreen.SetActive(false);
            }
        }

        //South
        if (south != null)
        {
            if (phases[i].southLeft)
            {
                south.LeftTurnGo();

                if (targetTime <= (phases[i].phaseDuration * yellowLightTransition))
                {
                    southLeftRed.SetActive(false);
                    southLeftYellow.SetActive(true);
                    southLeftGreen.SetActive(false);
                }
                else
                {
                    southLeftRed.SetActive(false);
                    southLeftYellow.SetActive(false);
                    southLeftGreen.SetActive(true);
                }
            }
            else
            {
                south.LeftTurnStop();

                southLeftRed.SetActive(true);
                southLeftYellow.SetActive(false);
                southLeftGreen.SetActive(false);
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

                if (targetTime <= (phases[i].phaseDuration * yellowLightTransition))
                {
                    southStraightRed.SetActive(false);
                    southStraightYellow.SetActive(true);
                    southStraightGreen.SetActive(false);
                }
                else
                {
                    southStraightRed.SetActive(false);
                    southStraightYellow.SetActive(false);
                    southStraightGreen.SetActive(true);
                }
            }
            else
            {
                south.StraightThroughStop();

                southStraightRed.SetActive(true);
                southStraightYellow.SetActive(false);
                southStraightGreen.SetActive(false);
            }
        }

        //East
        if (east != null)
        {
            if (phases[i].eastLeft)
            {
                east.LeftTurnGo();

                if (targetTime <= (phases[i].phaseDuration * yellowLightTransition))
                {
                    eastLeftRed.SetActive(false);
                    eastLeftYellow.SetActive(true);
                    eastLeftGreen.SetActive(false);
                }
                else
                {
                    eastLeftRed.SetActive(false);
                    eastLeftYellow.SetActive(false);
                    eastLeftGreen.SetActive(true);
                }
            }
            else
            {
                east.LeftTurnStop();

                eastLeftRed.SetActive(true);
                eastLeftYellow.SetActive(false);
                eastLeftGreen.SetActive(false);
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

                if (targetTime <= (phases[i].phaseDuration * yellowLightTransition))
                {
                    eastStraightRed.SetActive(false);
                    eastStraightYellow.SetActive(true);
                    eastStraightGreen.SetActive(false);
                }
                else
                {
                    eastStraightRed.SetActive(false);
                    eastStraightYellow.SetActive(false);
                    eastStraightGreen.SetActive(true);
                }
            }
            else
            {
                east.StraightThroughStop();

                eastStraightRed.SetActive(true);
                eastStraightYellow.SetActive(false);
                eastStraightGreen.SetActive(false);
            }
        }

        //West  
        if (west != null)
        {
            if (phases[i].westLeft)
            {
                west.LeftTurnGo();

                if (targetTime <= (phases[i].phaseDuration * yellowLightTransition))
                {
                    westLeftRed.SetActive(false);
                    westLeftYellow.SetActive(true);
                    westLeftGreen.SetActive(false);
                }
                else
                {
                    westLeftRed.SetActive(false);
                    westLeftYellow.SetActive(false);
                    westLeftGreen.SetActive(true);
                }
            }
            else
            {
                west.LeftTurnStop();

                westLeftRed.SetActive(true);
                westLeftYellow.SetActive(false);
                westLeftGreen.SetActive(false);
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

                if (targetTime <= (phases[i].phaseDuration * yellowLightTransition))
                {
                    westStraightRed.SetActive(false);
                    westStraightYellow.SetActive(true);
                    westStraightGreen.SetActive(false);
                }
                else
                {
                    westStraightRed.SetActive(false);
                    westStraightYellow.SetActive(false);
                    westStraightGreen.SetActive(true);
                }
            }
            else
            {
                west.StraightThroughStop();

                westStraightRed.SetActive(true);
                westStraightYellow.SetActive(false);
                westStraightGreen.SetActive(false);
            }
        }
    }
}
