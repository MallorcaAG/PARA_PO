using UnityEngine;

public class TrafficLightLaneManager : MonoBehaviour
{
    [SerializeField] private GameObject[] myLeftLane;
    [SerializeField] private GameObject[] myStraightThroughLane;
    [SerializeField] private GameObject[] myRightLane;

    public void LeftTurnGo()
    {
        if(myLeftLane == null)
        {
            return;
        }
        
        foreach(GameObject go in myLeftLane)
        {
            go.SetActive(false);
        }
        
    }

    public void RightTurnGo()
    {
        if (myRightLane == null)
        {
            return;
        }

        foreach (GameObject go in myRightLane)
        {
            go.SetActive(false);
        }
    }

    public void StraightThroughGo()
    {
        if (myStraightThroughLane == null)
        {
            return;
        }

        foreach (GameObject go in myStraightThroughLane)
        {
            go.SetActive(false);
        }
    }

    public void LeftTurnStop()
    {
        if (myLeftLane == null)
        {
            return ;
        }

        foreach (GameObject go in myLeftLane)
        {
            go.SetActive(true);
        }
    }

    public void RightTurnStop()
    {
        if (myRightLane == null)
        {
            return;    
        }

        foreach (GameObject go in myRightLane)
        {
            go.SetActive(true);
        }
    }

    public void StraightThroughStop()
    {
        if (myStraightThroughLane == null)
        {
            return;   
        }

        foreach (GameObject go in myStraightThroughLane)
        {
            go.SetActive(true);
        }
    }
}
