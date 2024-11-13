using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianAISensors : AISensors
{
    [SerializeField] protected Transform leftSense, rightSense;

    private PedestrianAINavigator myNav;
    protected bool LsensorDetected;
    protected bool RsensorDetected;

    int peddylaneLayer;
    RaycastHit hit, hit2, hit3;

    // Start is called before the first frame update
    void Start()
    {
        myNav = GetComponent<PedestrianAINavigator>();

        peddylaneLayer = LayerMask.NameToLayer("PedestrianTrafficSignal");
    }

    // Update is called once per frame
    void Update()
    {
        sensorDetected = Physics.Raycast(sensorPosition.position, sensorPosition.forward, out hit, maxDistance, includeLayers);
        LsensorDetected = Physics.Raycast(leftSense.position, leftSense.forward, out hit2, maxDistance, includeLayers);
        RsensorDetected = Physics.Raycast(rightSense.position, rightSense.forward, out hit3, maxDistance, includeLayers);

        try
        {
            if (hit.transform.gameObject.layer == peddylaneLayer)
            {
                myNav.Stop();
            }
            if (hit2.transform.gameObject.layer == peddylaneLayer)
            {
                myNav.Stop();
            }
            if (hit3.transform.gameObject.layer == peddylaneLayer)
            {
                myNav.Stop();
            }
        }
        catch { }
        

        if (sensorDetected)
        {

            if (LsensorDetected && RsensorDetected)
            {
                myNav.Stop();

            }
            else if(LsensorDetected)
            {
                myNav.pivot(-leftSense.forward, maxDistance);
            }
            else if(RsensorDetected)
            {
                myNav.pivot(-rightSense.forward, maxDistance);
            }
        }
        else
        {
            myNav.Go();
        }
    }

    private void OnDrawGizmos()
    {
        

        

        //Debugging Section
        if (!showCastsDebug)
        {
            return;
        }

        if (sensorDetected)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(sensorPosition.position, sensorPosition.forward * hit.distance);
            //Gizmos.DrawWireCube(sensorPosition.position + sensorPosition.forward * hit.distance, size);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(sensorPosition.position, sensorPosition.forward * maxDistance);
            //Gizmos.DrawWireCube(sensorPosition.position + sensorPosition.forward * maxDistance, size);
        }

        if (LsensorDetected)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(leftSense.position, leftSense.forward * hit.distance);
            //Gizmos.DrawWireCube(sensorPosition.position + sensorPosition.forward * hit.distance, size);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(leftSense.position, leftSense.forward * maxDistance);
            //Gizmos.DrawWireCube(sensorPosition.position + sensorPosition.forward * maxDistance, size);
        }

        if (RsensorDetected)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(rightSense.position, rightSense.forward * hit.distance);
            //Gizmos.DrawWireCube(sensorPosition.position + sensorPosition.forward * hit.distance, size);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(rightSense.position, rightSense.forward * maxDistance);
            //Gizmos.DrawWireCube(sensorPosition.position + sensorPosition.forward * maxDistance, size);
        }
    }

}
