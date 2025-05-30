using System.Collections;
using UnityEngine;

public class NPCDistanceToPlayer : MonoBehaviour
{
    [SerializeField] private NPCCount npcs;
    [SerializeField] private float range;
    public bool primeDestruction, excempted = false;

    private Transform player;
    private float distance;

    public void setNPCCount(NPCCount obj)
    {
        this.npcs = obj;
    }

    public void setRange(float r)
    {
        this.range = r;
    }

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
        if (excempted)
            return;

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

    public void kys() //ONLY USED WHEN CONTACT WITH PLAYER IS MADE
    {
        npcs.subtractVehicleNPC();

        primeDestruction = false;

        Destroy(gameObject);
    }
}
