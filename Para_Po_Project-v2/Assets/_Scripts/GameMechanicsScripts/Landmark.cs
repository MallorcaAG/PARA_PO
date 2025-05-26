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
    [Header("References")]
    [SerializeField] private SphereCollider myCollider;

    private Rigidbody player;
    bool isMoving;
    bool dataSent = false;
    bool playerPassed = false;
    bool clearArea = false;
    float radius;
    Vector3 pos;
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

    private void Start()
    {
        radius = myCollider.radius;
        pos = transform.position;
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
        Collider[] hitColliders = Physics.OverlapSphere(pos, radius * 2);
        foreach (Collider hitCollider in hitColliders)
        {
            GameObject obj = hitCollider.gameObject;

            if (obj.CompareTag("Pedestrians"))
            {
                obj.TryGetComponent<PedestrianAINavigator>(out PedestrianAINavigator ai);
                Debug.Log(obj.name + ": AHHHH IM GETTING ERADICATED\nbleeeehhhh *dying noises*");
                if(ai != null)
                {
                    ai.fastKYS();
                }
            }
            else if (obj.CompareTag("Vehicles"))
            {
                obj.TryGetComponent<VehicleAINavigator>(out VehicleAINavigator ai2);
                Debug.Log(obj.name + ": AHHHH IM GETTING ERADICATED\nbleeeehhhh *dying noises*");
                if(ai2 != null)
                {
                    ai2.fastKYS();
                }
            }
        }

    }

    

    /*private IEnumerator clearAreaCoroutine()
    {
        

        clearArea = true;

        yield return new WaitForSeconds(1f);

        clearArea = false;
    }*/
}
