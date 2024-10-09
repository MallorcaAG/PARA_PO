using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleAISensors : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform sensorPosition;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private LayerMask includeLayers;
    [Header("Debug")]
    [SerializeField] private bool showCastsDebug = true;

    private bool sensorDetected;
    private VehicleAINavigator myNav;

    #region Getter/Setter functions
    public bool getSensors()
    {
        return sensorDetected;
    }

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        myNav = GetComponent<VehicleAINavigator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!sensorDetected)
        {
            myNav.Gas();
        }
        else
        {
            myNav.Brakes();
        }
    }



    private void OnDrawGizmos()
    {
        RaycastHit hit;

        Vector3 halfExtent = sensorPosition.lossyScale / 2;
        Vector3 size = sensorPosition.lossyScale;

        sensorDetected = Physics.BoxCast(sensorPosition.position, halfExtent, sensorPosition.forward, out hit, sensorPosition.rotation, maxDistance, includeLayers);







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
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(sensorPosition.position, sensorPosition.forward * maxDistance);
            Gizmos.DrawWireCube(sensorPosition.position + sensorPosition.forward * maxDistance, size);
        }
    }
}
