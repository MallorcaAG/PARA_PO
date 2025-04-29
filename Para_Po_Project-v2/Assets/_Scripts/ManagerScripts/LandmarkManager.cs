using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandmarkManager : Singleton<LandmarkManager>
{
    [SerializeField] private GameObject[] landmarks;
    [SerializeField] private GameObject[] npc;
    [SerializeField] private GameObject uiIndicator;
    [Range(1, 60)] [SerializeField] private int spawnQuantity;
    
    [SerializeField, Min(0)] private int maxNPCsPerLandmark = 3;

    private const float spawnRadius = 2.5f;
    private const int maxAttempts = 10;

    private void Awake()
    {
        if (landmarks.Length < 2)
        {
            Debug.LogError("Not enough landmarks to spawn NPCs. At least 2 are required.");
            return;
        }

        // Randomly spawn NPCs per landmark based on inspector range
        foreach (GameObject landmark in landmarks)
        {
            int npcCount = Random.Range(0, maxNPCsPerLandmark + 1); // inclusive
            for (int i = 0; i < npcCount; i++)
            {
                SpawnNPCAtLandmark(landmark);
            }
        }
    }

    void SpawnNPCAtLandmark(GameObject landmark)
    {
        GameObject[] points = landmark.GetComponent<Landmark>().getSpawnpoints();

        if (points == null || points.Length == 0)
        {
            Debug.LogWarning($"No spawn points found for landmark: {landmark.name}");
            return;
        }

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            int spawnIndex = Random.Range(0, points.Length);
            Vector3 basePosition = points[spawnIndex].transform.position;
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = basePosition + new Vector3(randomOffset.x, 0, randomOffset.y);

            if (!IsNearWaypoint(spawnPosition))
                continue;

            GameObject npcObj = Instantiate(
                npc[Random.Range(0, npc.Length)],
                spawnPosition,
                Quaternion.identity
            );

            npcObj.GetComponent<NPCDistanceToPlayer>().excempted = true;
            Instantiate(uiIndicator, npcObj.transform, false);

            GameObject destinationLandmark = GetRandomDifferentLandmark(landmark);
            PedestrianAINavigator npcAI = npcObj.GetComponent<PedestrianAINavigator>();
            npcAI.setMyLandmark(landmark);
            npcAI.setDesiredLandmark(destinationLandmark);
            return;
        }

        Debug.LogWarning($"Failed to spawn NPC near waypoints at {landmark.name}");
    }

    void RandomizeSpawnLoc()
    {
        for (int attempts = 0; attempts < maxAttempts; attempts++)
        {
            int start = Random.Range(0, landmarks.Length - 1);
            int end = Random.Range(start + 1, landmarks.Length);

            GameObject startLandmark = landmarks[start];
            GameObject endLandmark = landmarks[end];

            GameObject[] points = startLandmark.GetComponent<Landmark>().getSpawnpoints();
            if (points == null || points.Length == 0) continue;

            int spawnIndex = Random.Range(0, points.Length);
            Vector3 basePosition = points[spawnIndex].transform.position;
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = basePosition + new Vector3(randomOffset.x, 0, randomOffset.y);

            if (!IsNearWaypoint(spawnPosition)) continue;

            GameObject npcObj = Instantiate(
                npc[Random.Range(0, npc.Length)],
                spawnPosition,
                Quaternion.identity
            );

            npcObj.GetComponent<NPCDistanceToPlayer>().excempted = true;
            Instantiate(uiIndicator, npcObj.transform, false);

            PedestrianAINavigator npcAI = npcObj.GetComponent<PedestrianAINavigator>();
            npcAI.setMyLandmark(startLandmark);
            npcAI.setDesiredLandmark(endLandmark);
            return;
        }

        Debug.LogWarning("Random NPC spawn failed after multiple attempts.");
    }

    bool IsNearWaypoint(Vector3 position)
    {
        Collider[] hits = Physics.OverlapSphere(position, 5f); // 5 units radius
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Waypoint")) return true;
        }
        return false;
    }

    GameObject GetRandomDifferentLandmark(GameObject exclude)
    {
        List<GameObject> options = new List<GameObject>(landmarks);
        options.Remove(exclude);
        return options[Random.Range(0, options.Count)];
    }
}
