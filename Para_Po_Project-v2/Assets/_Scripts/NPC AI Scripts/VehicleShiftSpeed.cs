using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core;
using UnityEngine;

public enum Shift
{
    UP, DOWN
}

public class VehicleShiftSpeed : AISensors
{
    [SerializeField] private Shift shift = Shift.DOWN;
    [SerializeField] private float value = 2f;
    [SerializeField] private bool disableSensor = false;

    private RandomMovementSpeed movement;
    private bool speedSent = false;


    // Update is called once per frame
    void Update()
    {
        //Raycast
        RaycastHit hit;

        Ray ray = new Ray(transform.position + new Vector3(0, -0.2f, 0), transform.up);

        sensorDetected = Physics.Raycast(ray, out hit, maxDistance, includeLayers);

        /*Debug.Log(sensorDetected.ToString());*/

        //Call event once
        if(!sensorDetected)
        {
            speedSent = false;
            return;
        }
        else if(sensorDetected && hit.transform.gameObject.TryGetComponent<RandomMovementSpeed>(out RandomMovementSpeed ai))
        {
            if (speedSent)
            {
                return;
            }

            speedShifter(hit, ai);
            speedSent = true;
        } 

    }

    private void speedShifter(RaycastHit hit, RandomMovementSpeed ai)
    {
        movement = ai;

        //Shift Speeds
        if (shift == Shift.DOWN)
        {
            movement.reduceSpeed(value);
            if (disableSensor)
            {
                movement.disableSensor();
            }
        }
        else
        {
            movement.increaseSpeed();
        }
    }

    private void OnDrawGizmos()
    {

        //Debug.Log(hit.transform.gameObject.ToString());

        //Debugging Section
        if (!showCastsDebug)
        {
            return;
        }

        if (sensorDetected)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.up * maxDistance);
            
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.up * maxDistance);
            
        }
    }

}
