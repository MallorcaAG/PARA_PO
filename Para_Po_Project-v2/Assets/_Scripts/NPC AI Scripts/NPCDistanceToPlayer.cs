using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDistanceToPlayer : MonoBehaviour
{
    [SerializeField] private NPCCount npcs;
    [SerializeField] private float range;
    public bool primeDestruction;

    private Transform player;
    private float distance;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(5f);

        primeDestruction = true;
    }

    private void Update()
    {
        distance = Vector3.Distance(player.position, transform.position);

        if(primeDestruction)
        {
            if(distance >= range)
            {
                if(gameObject.TryGetComponent<PedestrianAINavigator>(out PedestrianAINavigator peds))
                {
                    npcs.subtractPedestrianNPC();
                }
                else if(gameObject.TryGetComponent<VehicleAINavigator>(out VehicleAINavigator car))
                {
                    npcs.subtractVehicleNPC();
                }

                //Turn this off because omae wa mou shindeiru
                primeDestruction = false;

                Destroy(gameObject);
            }
        }
    }
}
