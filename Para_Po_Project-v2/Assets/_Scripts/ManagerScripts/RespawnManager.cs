using UnityEngine;

public class RespawnManager : MonoBehaviour
{

    [SerializeField] private Transform playerStartPosition;
    [SerializeField] private GameEvent onPlayerRespawn;

    [SerializeField] private VehicleManager vehicleManager;
    [Space]
    [Header("Trike Specific Settings")]
    [SerializeField] private bool isTricycle = false;
    [Header("\tREQUIRED if isTrike is true")]
    [SerializeField] private Rigidbody sphereRB;
    private Landmark lastCheckpoint;
    private float targetTime = 2f;

    private void Start()
    {
        if (vehicleManager == null)
            vehicleManager = GetComponent<VehicleManager>();

        if (playerStartPosition == null)
            playerStartPosition = GameObject.Find("PlayerStartingPoint").transform;
    }

    private void Update() 
    {
        GetPlayerInput();
    }

    private void GetPlayerInput()
    {
        if(Timer())
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                targetTime = 2f;
                if(isTricycle)
                {
                    InitiateTricycleRespawn();
                }
                else
                {
                    InitiateRespawn();
                }
            }
        }
        
    }

    private void InitiateRespawn()
    {
        onPlayerRespawn.Raise(this, 0);

        if(vehicleManager.getCheckpoint() == null)
        {
            transform.position = playerStartPosition.position + Vector3.up;
            transform.rotation = playerStartPosition.rotation;
            ClearArea(transform.position, 5f);
            return;
        }

        lastCheckpoint = vehicleManager.getCheckpoint();
        lastCheckpoint.ClearArea();
        GameObject spawnpoint = lastCheckpoint.getSpawnpoints()[0];

        transform.position = spawnpoint.transform.position + Vector3.up;    
        transform.rotation = spawnpoint.transform.rotation;
    }
    private void InitiateTricycleRespawn()
    {
        onPlayerRespawn.Raise(this, 0);

        if (vehicleManager.getCheckpoint() == null)
        {
            sphereRB.transform.position = playerStartPosition.position + Vector3.up;
            sphereRB.transform.rotation = playerStartPosition.rotation;
            ClearArea(sphereRB.transform.position, 5f);
            return;
        }

        lastCheckpoint = vehicleManager.getCheckpoint();
        lastCheckpoint.ClearArea();
        GameObject spawnpoint = lastCheckpoint.getSpawnpoints()[0];

        Debug.LogWarning("checkpoint spawnpoint:\n" + spawnpoint.transform.position);

        sphereRB.transform.position = spawnpoint.transform.position + Vector3.up;
        sphereRB.transform.rotation = spawnpoint.transform.rotation;
    }

    public void ClearArea(Vector3 tf, float rad)
    {
        Collider[] hitColliders = Physics.OverlapSphere(tf, rad);
        foreach (Collider hitCollider in hitColliders)
        {
            GameObject obj = hitCollider.gameObject;

            if (obj.CompareTag("Pedestrians"))
            {
                obj.TryGetComponent<PedestrianAINavigator>(out PedestrianAINavigator ai);
                Debug.Log(obj.name + ": AHHHH IM GETTING ERADICATED\nbleeeehhhh *dying noises*");
                ai.fastKYS();
            }
            else if (obj.CompareTag("Vehicles"))
            {
                obj.TryGetComponent<VehicleAINavigator>(out VehicleAINavigator ai2);
                Debug.Log(obj.name + ": AHHHH IM GETTING ERADICATED\nbleeeehhhh *dying noises*");
                ai2.fastKYS();
            }
        }

    }

    private bool Timer()
    {
        targetTime -= Time.deltaTime;

        if (targetTime <= 0.0f)
        {
            return true;
        }

        return false;
    }
}
