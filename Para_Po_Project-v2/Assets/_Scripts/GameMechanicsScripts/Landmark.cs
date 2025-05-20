using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using static Codice.Client.Common.EventTracking.TrackFeatureUseEvent.Features.DesktopGUI.Filters;

public class Landmark : MonoBehaviour
{
    [Header("Game Events")]
    [SerializeField] private GameEvent onPlayerEnterLandmark;
    [SerializeField] private GameEvent onPlayerExitLandmark;
    [SerializeField] private GameEvent onPlayerStopAtLandmark;
    [Header("Spawn Points")]
    [SerializeField] private GameObject[] spawnPoints;

    private List<PedestrianAINavigator> peddyInBounds = new List<PedestrianAINavigator>();
    private List<VehicleAINavigator> vehicleInBounds = new List<VehicleAINavigator>();
    private Rigidbody player;
    bool isMoving;
    bool dataSent = false;
    bool playerPassed = false;
    bool clearArea = false;
    public GameObject[] getSpawnpoints()
    {
        return spawnPoints;
    }
    public bool PlayerPassedByBefore()
    {
        return playerPassed; 
    }
    public GameObject[] getSpawnPoints()
    {
        return spawnPoints;
    }


    private void LateUpdate()
    {
        if (player == null)
        {
            return;
        }

        if (player != null)
        {
            if (player.velocity.magnitude > 0.1f)
            {
                isMoving = true;
                //Debug.Log("Player is moving: "+ player.velocity.magnitude);

                if (dataSent)
                {
                    dataSent = false;
                }
            }
            else
            {
                isMoving = false;
                //Debug.Log("Player is still: " + player.velocity.magnitude);

                if (!dataSent)
                {
                    onPlayerStopAtLandmark.Raise(this, this.gameObject);
                    //Debug.Log("Data sent");
                    dataSent = true;
                }
            }

            
            

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject);
        GameObject obj = other.gameObject;

        if(player != null)
        {
            return;
        }

        if(obj.CompareTag("Player"))
        {
            player = obj.GetComponent<Rigidbody>();

            onPlayerEnterLandmark.Raise(this, this.gameObject);
        }
        else if (obj.TryGetComponent<PedestrianAINavigator>(out PedestrianAINavigator ai))
        {
            peddyInBounds.Add(ai);
        }
        else if (obj.TryGetComponent<VehicleAINavigator>(out VehicleAINavigator ai2))
        {
            vehicleInBounds.Add(ai2);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject obj = other.gameObject;

        if (obj.CompareTag("Player"))
        {
            player = null;

            onPlayerExitLandmark.Raise(this, this.gameObject);

            playerPassed = true;
        }
        else if (obj.TryGetComponent<PedestrianAINavigator>(out PedestrianAINavigator ai))
        {
            peddyInBounds.Remove(ai);
        }
        else if (obj.TryGetComponent<VehicleAINavigator>(out VehicleAINavigator ai2))
        {
            vehicleInBounds.Remove(ai2);
        }
    }

    /*private void OnTriggerStay(Collider other)
    {
        if(clearArea)
        {
            if(other.gameObject.CompareTag("Pedestrians") || other.gameObject.CompareTag("Vehicles"))
            {
                Debug.Log(other.gameObject.name + ": AHHHH IM GETTING ERADICATED\nbleeeehhhh *dying noises*");
                Destroy(other.gameObject);
            }
        }
    }*/

    public void ClearArea()
    {
        foreach(PedestrianAINavigator ai in peddyInBounds)
        {
            Debug.Log(ai.gameObject.name + ": AHHHH IM GETTING ERADICATED\nbleeeehhhh *dying noises*");
            ai.fastKYS();
        }

        foreach(VehicleAINavigator ai in vehicleInBounds)
        {
            Debug.Log(ai.gameObject.name + ": AHHHH IM GETTING ERADICATED\nbleeeehhhh *dying noises*");
            ai.fastKYS();
        }

        //StartCoroutine(clearAreaCoroutine());
    }

    /*private IEnumerator clearAreaCoroutine()
    {
        

        clearArea = true;

        yield return new WaitForSeconds(1f);

        clearArea = false;
    }*/
}
