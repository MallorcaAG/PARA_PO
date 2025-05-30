using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] protected NPCCount npcs;
    [SerializeField] protected GameObject[] prefabCollection;
    [SerializeField] protected GameObject mySpawnedObj;

    protected Waypoint myWaypoint;



    public GameObject[] getPrefab()
    {
        return prefabCollection;
    }
    public void setMyWaypoint(Waypoint waypoint)
    {
        myWaypoint = waypoint;
    }


}
