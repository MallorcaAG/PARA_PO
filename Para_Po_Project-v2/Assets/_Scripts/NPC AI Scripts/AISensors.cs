using UnityEngine;

public class AISensors : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] protected Transform sensorPosition;
    [SerializeField] protected float maxDistance = 10f;
    [SerializeField] protected LayerMask includeLayers;
    [Header("Debug")]
    [SerializeField] protected bool showCastsDebug = true;

    protected float holdDist;
    protected bool sensorDetected;
    

    #region Getter/Setter functions
    public bool getSensors()
    {
        return sensorDetected;
    }
    public float getDefaultMaxDist()
    {
        return holdDist;
    }
    public void setMaxDistance(float n)
    {
        maxDistance = n;
    }
    protected void saveDefaultMaxDist()
    {
        holdDist = maxDistance;
    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        holdDist = maxDistance;
    }

    private void Update()
    {
        
    }

    protected void raycasting()
    {
        RaycastHit hit;

        Vector3 halfExtent = sensorPosition.lossyScale / 2;
        Vector3 size = sensorPosition.lossyScale;

        sensorDetected = Physics.BoxCast(sensorPosition.position, halfExtent, sensorPosition.forward, out hit, sensorPosition.rotation, maxDistance, includeLayers);

        //Debug.Log(gameObject.name + " Sensor detected" + sensorDetected);
    }

    /*private void OnDrawGizmos()
    {
        

        //Debugging Section
        if(!showCastsDebug)
        {
            return;
        }

        if(sensorDetected)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(sensorPosition.position, sensorPosition.forward * hit.distance);
            Gizmos.DrawWireCube(sensorPosition.position + sensorPosition.forward * hit.distance, size);

            //Debug.LogWarning("Colliding with: "+hit.rigidbody.gameObject.name);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(sensorPosition.position, sensorPosition.forward * maxDistance);
            Gizmos.DrawWireCube(sensorPosition.position + sensorPosition.forward * maxDistance, size);
        }
    }*/
}
