using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandmarkManager : MonoBehaviour
{
    [SerializeField] private GameObject[] landmarks;
    [SerializeField] private GameObject npc;

    private void Awake()
    {
        GameObject npcObj = Instantiate(npc, landmarks[0].GetComponent<Landmark>().getSpawnpoints()[0].transform.position, Quaternion.identity);
        PedestrianAINavigator npcAI = npcObj.GetComponent<PedestrianAINavigator>();
        npcAI.setMyLandmark(landmarks[0]);
        npcAI.setDesiredLandmark(landmarks[3]);
    }

}
