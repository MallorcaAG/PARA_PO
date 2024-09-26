using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Landmark : MonoBehaviour
{
    [Header("Game Events")]
    [SerializeField] private GameEvent onPlayerEnterLandmark;
    [SerializeField] private GameEvent onPlayerExitLandmark;
    [SerializeField] private GameEvent onPlayerStopAtLandmark;
    [Header("Spawn Points")]
    [SerializeField] private GameObject[] spawnPoints;
    [SerializeField] private GameObject npc;

    private Rigidbody player;
    bool isMoving;
    bool dataSent = false;

    private void Awake()
    {
        GameObject npcObj = Instantiate(npc, spawnPoints[0].transform.position, Quaternion.identity);
        PedestrianAINavigator npcAI = npcObj.GetComponent<PedestrianAINavigator>();
        npcAI.setMyLandmark(this.gameObject);
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
            }
            else
            {
                isMoving = false;

                if (!dataSent)
                {
                    onPlayerStopAtLandmark.Raise(this, this.gameObject);
                    Debug.Log("Data sent");
                    dataSent = true;
                }
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(player != null)
        {
            return;
        }

        if(other.gameObject.CompareTag("Player"))
        {
            player = other.gameObject.GetComponent<Rigidbody>();

            onPlayerEnterLandmark.Raise(this, this.gameObject);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = null;

            onPlayerExitLandmark.Raise(this, this.gameObject);
        }
    }
}
