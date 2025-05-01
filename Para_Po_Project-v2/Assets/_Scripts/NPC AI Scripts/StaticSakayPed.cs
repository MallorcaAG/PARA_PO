using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticSakayPed : MonoBehaviour
{
    [Header("I make pre-spawned SakayPeds NPC AI in the editor\n from spawnable runtime NPCAI prefabs\n" +
        "\tHOW TO USE:\n" +
        "\t1. Attach script to NPCAI prefab \n" +
        "\t2. Assign PedestrianAINavigator's DesiredLandmark\n" +
        "\t3. PLAY\n\n(Ignore this) V V V")]
    [SerializeField] private PedestrianAINavigator myNav;

    // Start is called before the first frame update
    void Start()
    {
        myNav = GetComponent<PedestrianAINavigator>();
        myNav.setMyLandmark(gameObject);
        myNav.setCurrentWaypoint(myNav.getPlayersWaypoint());
        myNav.GetOnVehicle(this, gameObject);
    }
}
