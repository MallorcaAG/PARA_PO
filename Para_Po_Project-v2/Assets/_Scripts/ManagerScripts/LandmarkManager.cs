using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandmarkManager : Singleton<LandmarkManager>
{
    [SerializeField] private GameObject[] landmarks;
    [SerializeField] private GameObject[] npc;
    [SerializeField] private GameObject uiIndicator;
    [Range(1, 60)] [SerializeField] private int spawnQuantity;

    private void Awake()
    {
        if (landmarks.Length < 2)
        {
            Debug.LogError("Not enough landmarks to spawn NPCs. At least 2 are required.");
            return;
        }

        for (int i = 0; i < spawnQuantity; i++)
        {
            RandomizeSpawnLoc();
        }
    }

    void RandomizeSpawnLoc()
    {
        if (landmarks.Length < 2) return;

        int start = Random.Range(0, landmarks.Length - 1);
        int end = Random.Range(start + 1, landmarks.Length);

        // Safety check for valid spawn points
        GameObject[] points = landmarks[start].GetComponent<Landmark>().getSpawnpoints();
        if (points == null || points.Length == 0)
        {
            Debug.LogWarning($"No spawn points found for landmark: {landmarks[start].name}");
            return;
        }

        int startPos = Random.Range(0, points.Length);

        GameObject npcObj = Instantiate(
            npc[Random.Range(0, npc.Length)],
            points[startPos].transform.position,
            Quaternion.identity
        );

        Instantiate(uiIndicator, npcObj.transform, false);

        PedestrianAINavigator npcAI = npcObj.GetComponent<PedestrianAINavigator>();
        npcAI.setMyLandmark(landmarks[start]);
        npcAI.setDesiredLandmark(landmarks[end]);
    }
}
