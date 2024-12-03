using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;

public class LandmarkManager : Singleton<LandmarkManager>
{
    [SerializeField] private GameObject[] landmarks;
    [SerializeField] private GameObject[] npc;
    [SerializeField] private GameObject uiIndicator;
    [Range(1,60)][SerializeField] private int spawnQuantity;
    

    private void Awake()
    {
        for (int i = spawnQuantity; i > 0; i--)
        {
            RandomizeSpawnLoc();
        }

    }

    void RandomizeSpawnLoc()
    {
        //Debug.Log("SPAWNING");

        int start = Random.Range(1, landmarks.Length - 2); //Test on a larger scale
        int end = Random.Range(start + 1, landmarks.Length);

        GameObject[] points = landmarks[start].GetComponent<Landmark>().getSpawnpoints();
        int startPos = Random.Range(0, points.Length);

        GameObject npcObj = Instantiate(npc[Random.Range(0,npc.Length)], points[startPos].transform.position, Quaternion.identity); 
        Instantiate(uiIndicator,npcObj.transform,false);
        PedestrianAINavigator npcAI = npcObj.GetComponent<PedestrianAINavigator>();
        npcAI.setMyLandmark(landmarks[start]);
        npcAI.setDesiredLandmark(landmarks[end]);
    }

}
